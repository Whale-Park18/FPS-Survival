using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WhalePark18.Character.Enemy;
using WhalePark18.ObjectPool;

namespace WhalePark18.Manager
{
    public class EnemyManager : MonoSingleton<EnemyManager>
    {
        private EnemyObjectPool enemyObjectPool;
        private EnemySpawnTileObjectPool enemySpawnTileObjectPool;

        [Header("Enemy Spawn")]
        private int currentEnemyCount;  // 현재 소환된 적의 수
        [SerializeField, Tooltip("소환된 적의 최대 수")]
        private int maxEnemyCount;
        [SerializeField, Tooltip("한 번에 소환되는 적의 수")]
        private int numberOfEnemiesSpawnedAtOnce;
        [SerializeField, Tooltip("적 소환 주기")]
        private float enemySpawnCycleTime;
        [SerializeField, Tooltip("타일 생성 후, 적이 소환하기까지 대기 시간")]
        private float enemySpawnDelayTime;

        [Header("Overlap Position")]
        [SerializeField, Tooltip("enemy 캡슐 콜라이더의 높이")]
        private float enemyHeight;
        [SerializeField, Tooltip("enemy 캡슐 콜라이더의 반지름")]
        private float enemyRadius;
        [SerializeField, Tooltip("Pillar 게임오브젝트의 레이어")]
        private LayerMask pillarLayerMask;
        [SerializeField, Tooltip("Pillar과 겹쳤을 때, 회피할 반지름")]
        private float radiusOfAvoid;

        [Header("Spawn Range")]
        [SerializeField, Tooltip("맵 크기")]
        private Vector2 mapSize;

        private Transform target;   // Enemy 공격 목표
        private int killCount;      // Enemy 처치수

        public Transform Target
        {
            set => target = value;
            get => target;
        }

        public int KillCount => killCount;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }

            OnSInitialized();
        }

        private void OnSInitialized()
        {
            enemyObjectPool = GetComponentInChildren<EnemyObjectPool>();
            enemySpawnTileObjectPool = GetComponentInChildren<EnemySpawnTileObjectPool>();
        }

        public void Run()
        {
            print("Enemy Manager Run");
            StartCoroutine(SpawnTile());
        }

        /// <summary>
        /// 적 소환시 미리 알려주는 타일(EnemySpawnTile)을 소환하는 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator SpawnTile()
        {
            while(true)
            {
                /// 동시에 numberOfEnemiesSpawnedAtOnce 숫자만큼 적을 소환한다.
                /// 단, maxEnemyCount를 넘을 수는 없다.
                for (int i = 0; i < numberOfEnemiesSpawnedAtOnce && currentEnemyCount < maxEnemyCount; i++)
                {
                    currentEnemyCount++;
                    EnemySpawnPoint spawnPoint = enemySpawnTileObjectPool.GetObject(GetRandomSpawnPoint());
                    StartCoroutine(SpawnEnemy(spawnPoint));
                }

                /// 적 소환 주기 시간까지 대기
                yield return new WaitForSeconds(enemySpawnCycleTime);
            }

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

            WhalePark18.Debug.Log(DebugCategory.Debug, "Spawn Tile Position",
                "Enemy{0} SpawnPosition: {1}", currentEnemyCount, randomSpawnPosition);

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
            Vector3 point1 = position + Vector3.up * enemyHeight;
            int overlapCount = Physics.OverlapCapsuleNonAlloc(point0, point1, enemyRadius, null, pillarLayerMask);

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
        /// 적을 소환하는 메소드
        /// </summary>
        /// <param name="enemySpawnPoint">적 생성시 미리 알려주는 타일</param>
        /// <returns></returns>
        private IEnumerator SpawnEnemy(EnemySpawnPoint enemySpawnPoint)
        {
            /// 적 소환 대기시간 만큼 대기
            yield return new WaitForSeconds(enemySpawnDelayTime);

            /// enemySpawnPoint 위치에 적 소환
            Vector3 spawnPosition = enemySpawnPoint.transform.position;
            //spawnPosition.y = 0;
            WhalePark18.Debug.Log(DebugCategory.Debug, "Spawn Enemy Position",
                "Enemy{0} SpawnPosition: {1}", currentEnemyCount, spawnPosition);

            EnemyFSM enemyFSM = enemyObjectPool.GetObject(spawnPosition);
            enemyFSM.Setup(target);

            /// enemySpawnTile 오브젝트풀에 반환
            enemySpawnTileObjectPool.ReturnObject(enemySpawnPoint);
        }

        /// <summary>
        /// 적 반환 인터페이스
        /// </summary>
        /// <param name="returnEnemyFSM"></param>
        public void ReturnEnemy(EnemyFSM returnEnemyFSM)
        {
            /// 오브젝트를 반환하고 현재 적 수와 처치수를 갱신한다.
            enemyObjectPool.ReturnObject(returnEnemyFSM);
            currentEnemyCount--;
            killCount++;

            if (KillCount == GameManager.Instance.KillGoals)
                GameManager.Instance.GameOver();

            /// killCount가 10의 배수일 때마다
            /// 한번에 소환되는 적의 수를 증가시킨다.
            if (killCount % 10 == 0)
                numberOfEnemiesSpawnedAtOnce++;
        }
    }
}