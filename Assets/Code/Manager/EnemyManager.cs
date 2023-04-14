using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WhalePark18.Character.Enemy;
using WhalePark18.ObjectPool;

namespace WhalePark18.Manager
{
    public enum EnemyClass { Normal = 0, Elite, Boss }

    public struct KillCountInfo
    {
        public int normal;
        public int elite;
        public int boss;
    }

    public class EnemyManager : MonoSingleton<EnemyManager>
    {
        [System.Serializable]
        private struct EnemySpawnInfo
        {
            public int      maxSpawnCount;          // 최대 소환 수
            public int      currentSpawnCount;      // 현재 소환 수
            public int      numberOfSpawnAtOnce;    // 한 번에 소환되는 수
            public float    spawnCycleTime;         // 적 소환 주기
            public float    spawnDelayTime;         // 타일 생성 후, 소혼되기까지 대기 시간
        }

        /****************************************
         * EnemyManager Base
         ****************************************/
        [SerializeField]
        private EnemyObjectPool[]           enemyObjectPools;
        [SerializeField]
        private EnemySpawnTileObjectPool[]  enemySpawnTileObjectPools;
        private Transform                   target;                             // Enemy 공격 목표
        private Queue<Coroutine>            spawnEnemyBackgroundHandleQueue;    // SpawnEnemyBackground에 코루틴 핸들을
                                                                                // 관리하는 큐

        [Header("Enemy Spawn")]
        [SerializeField, Tooltip("Normal, Elite, Boss 순")]
        private EnemySpawnInfo[] enemySpawnInfos;

        [Header("Overlap Position")]
        [SerializeField, Tooltip("중복 확인에 사용될 높이")]
        private float overlapHeight;
        [SerializeField, Tooltip("중복 확인에 사용될 반지름")]
        private float overlapRadius;
        [SerializeField, Tooltip("Pillar 게임오브젝트의 레이어")]
        private LayerMask pillarLayerMask;
        [SerializeField, Tooltip("Pillar과 겹쳤을 때, 회피할 반지름")]
        private float radiusOfAvoid;

        [Header("Spawn Range")]
        [SerializeField, Tooltip("맵 크기")]
        private Vector2 mapSize;

        /****************************************
         * Kill Info
         ****************************************/
        private KillCountInfo killCountInfo;

        /****************************************
         * 프로퍼티
         ****************************************/
        public KillCountInfo KillCountInfo => killCountInfo;

        /// <summary>
        /// EnemyManager가 인스턴스화 된 직후, 호출되는 초기호 메소드
        /// </summary>
        private void Awake()
        {
            /// 1. 싱글톤 작업
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }

            /// 2. 컴포넌트 및 변수 초기화
            //enemySpawnInfos = new EnemySpawnInfo[3];
            killCountInfo = new KillCountInfo();
            spawnEnemyBackgroundHandleQueue = new Queue<Coroutine>();
        }

        public void Setup(Transform target)
        {
            this.target = target;

            for(int i = 0; i < enemyObjectPools.Length; i++)
            {
                enemyObjectPools[i].Target = target;
            }
        }

        public void Reset()
        {
            /// 1. 소환된 모든 적 오브젝트풀에 반환
            foreach (var objectPool in enemyObjectPools)
                objectPool.Reset();

            /// 2. 발사된 모든 투사체 오브젝트 풀에 반환
            /// (지속 시간을 생각해 볼 때, 안해도될 것 같음)
        }

        /// <summary>
        /// 지속적으로 Enemy를 소환해야 할 때, 사용하는 메소드
        /// </summary>
        /// <param name="spawnEnemyClass">소환할 적의 등급</param>
        public void SpawnEnemyBackground(EnemyClass spawnEnemyClass)
        {
            spawnEnemyBackgroundHandleQueue.Enqueue(StartCoroutine(OnSpawnEnemyBackground(spawnEnemyClass)));
        }

        /// <summary>
        /// 지속적으로 Enemy를 소환하는 메소드
        /// </summary>
        /// <param name="spawnEnemyClass">소환할 적의 등급</param>
        /// <returns>코루틴</returns>
        private IEnumerator OnSpawnEnemyBackground(EnemyClass spawnEnemyClass)
        {
            WaitForSeconds waitSpawnEnemyCycleTime = new WaitForSeconds(enemySpawnInfos[(int)spawnEnemyClass].spawnCycleTime);

            while (true)
            {
                /// spawnEnemyClass의 소환 정보를 통해 조건을 확인한다.
                /// 1. spawnCount < numberOfSpawnAtOnce
                /// 2. currentSpawnCount < maxSpawnCount
                /// (단, 두 조건 모두 만족할 때 적을 소환한다.)
                for (int spawnCount = 0; spawnCount < enemySpawnInfos[(int)spawnEnemyClass].numberOfSpawnAtOnce && enemySpawnInfos[(int)spawnEnemyClass].currentSpawnCount < enemySpawnInfos[(int)spawnEnemyClass].maxSpawnCount; spawnCount++)
                {
                    if (spawnEnemyClass == EnemyClass.Normal)
                        enemySpawnInfos[(int)EnemyClass.Normal].currentSpawnCount++;

                    SpawnTile(spawnEnemyClass);
                }

                yield return waitSpawnEnemyCycleTime;
            }
        }

        /// <summary>
        /// SpawnEnemyBackground 작동을 멈추는 메소드
        /// (단, 작동한 순서대로 멈춘다.)
        /// </summary>
        public void StopSpawnEnemyBackground()
        {
            StopCoroutine(spawnEnemyBackgroundHandleQueue.Dequeue());
        }

        /// <summary>
        /// 트리거에 의해 Enemy이 소환되야 할 때, 사용하는 메소드
        /// </summary>
        /// <param name="spawnEnemyClass">소환할 적의 등급</param>
        public void SpawnEnemyEvent(EnemyClass spawnEnemyClass)
        {
            SpawnTile(spawnEnemyClass);
        }

        /// <summary>
        /// 적 소환시 미리 알려주는 타일(EnemySpawnTile)을 소환하는 메소드
        /// </summary>
        /// <param name="spawnEnemyClass">소환할 적의 등급</param>
        /// <returns>코루틴</returns>
        private void SpawnTile(EnemyClass spawnEnemyClass)
        {
            EnemySpawnPoint spawnPoint = enemySpawnTileObjectPools[(int)spawnEnemyClass].GetObject(GetRandomSpawnPoint());
            StartCoroutine(SpawnEnemy(spawnPoint, spawnEnemyClass));
        }

        /// <summary>
        /// 무작위 소환 위치를 반환하는 메소드
        /// </summary>
        /// <returns>무작위 소환 위치</returns>
        private Vector3 GetRandomSpawnPoint()
        {
            /// 맵은 월드 중심에 있기 때문에 맵 크기의 반값이 절대값이 된다.
            /// 맵 크기의 반값 위치에 적이 소환되면 맵 끝에 걸리기 때문에 절반 -1의 크기가 절대값이 된다.
            float absoluteX = mapSize.x * 0.5f - 1;
            float absoluteZ = mapSize.y * 0.5f - 1;
            float randomX = Random.Range(-absoluteX, absoluteX);
            float randomZ = Random.Range(-absoluteZ, absoluteZ);
            Vector3 randomSpawnPosition = new Vector3(randomX, 1, randomZ);

            /// 임의로 설정된 위치가 기둥과 겹친다면
            /// 겹치지 않는 위치를 계산해 위치로 설정한다.
            if (IsOverlapOnPillar(randomSpawnPosition))
                randomSpawnPosition = CalculateAvoidOverlapPosition(randomSpawnPosition);

            return randomSpawnPosition;
        }

        /// <summary>
        /// 기둥에 겹치는지 확인하는 메소드
        /// </summary>
        /// <param name="position">임의 스폰 위치</param>
        /// <returns></returns>
        private bool IsOverlapOnPillar(Vector3 position)
        {
            /// interactionObjectMask 레이어마스크에 겹치는 오브젝트 개수가 1이상이면
            /// 기둥에 겹치는 것을 의미한다.
            Vector3 point0 = position;
            Vector3 point1 = position + Vector3.up * overlapHeight;
            int overlapCount = Physics.OverlapCapsuleNonAlloc(point0, point1, overlapRadius, null, pillarLayerMask);

            return overlapCount > 0;
        }

        /// <summary>
        /// 기둥위치에 겹치지 않는 위치를 계산하는 메소드
        /// </summary>
        /// <returns>position기준, radius 반지름 만큼의 램덤 위치</returns>
        private Vector3 CalculateAvoidOverlapPosition(Vector3 position)
        {
            /// 임의의 각도
            float angle = Random.Range(0f, 360f);

            /// Sin, Cos를 통해 R=1일 때, 좌표 값을 구한 후
            /// radiusOfAvoid 반지름만큼 곱해 이동할 offset 벡터를 구한다.
            float x = Mathf.Sin(angle);
            float z = Mathf.Cos(angle);
            Vector3 offset = new Vector3(x, 0, z) * radiusOfAvoid;

            /// 현재 위치에서 offset 만큼 이동시킨 위치를 반환한다.
            return position + offset;
        }

        /// <summary>
        /// Enemy를 소환하는 메소드
        /// </summary>
        /// <param name="spawnPoint">적 생성시 미리 알려주는 타일</param>
        /// <param name="spawnEnemyClass">소환할 적의 등급</param>
        /// <returns></returns>
        private IEnumerator SpawnEnemy(EnemySpawnPoint spawnPoint, EnemyClass spawnEnemyClass)
        {
            /// 1. 적 소환 대기시간 만큼 대기
            WaitForSeconds waitSpawnEnemyDelayTime = new WaitForSeconds(enemySpawnInfos[(int)spawnEnemyClass].spawnDelayTime);
            yield return waitSpawnEnemyDelayTime;

            /// 2. spawnPoint 위치에 적 소환
            Vector3 spawnPosition = spawnPoint.transform.position;
            EnemyBase enemy = enemyObjectPools[(int)spawnEnemyClass].GetObject(spawnPosition);

            /// 3. enemySpawnTile 오브젝트풀에 반환
            enemySpawnTileObjectPools[(int)spawnEnemyClass].ReturnObject(spawnPoint);
        }

        /// <summary>
        /// 적 반환 인터페이스
        /// </summary>
        /// <param name="returnEnemy"></param>
        public void ReturnEnemy(EnemyBase returnEnemy)
        {
            /// 1. 적의 등급에 맞게 반환하고 처치 수 정보를 갱신한다.
            enemyObjectPools[(int)returnEnemy.Class].ReturnObject(returnEnemy);

            switch(returnEnemy.Class)
            {
                case EnemyClass.Normal:
                    killCountInfo.normal++;
                    break;

                case EnemyClass.Elite:
                    killCountInfo.elite++;
                    break;

                case EnemyClass.Boss:
                    killCountInfo.boss++;
                    break;
            }

            /// 2. Normal 등급 처치 수 >= 목표 처치수 이라면 게임을 종료한다.
            if (killCountInfo.normal >= GameManager.Instance.GoalNumberOfKill)
                GameManager.Instance.GameOver();

            /// 3. killCount가 10의 배수일 때마다
            if (killCountInfo.normal % 10 == 0)
            {
                enemySpawnInfos[(int)EnemyClass.Normal].numberOfSpawnAtOnce++;
                SpawnEnemyEvent(EnemyClass.Elite);
            }
        }
    }
}