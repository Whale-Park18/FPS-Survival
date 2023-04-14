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
            public int      maxSpawnCount;          // �ִ� ��ȯ ��
            public int      currentSpawnCount;      // ���� ��ȯ ��
            public int      numberOfSpawnAtOnce;    // �� ���� ��ȯ�Ǵ� ��
            public float    spawnCycleTime;         // �� ��ȯ �ֱ�
            public float    spawnDelayTime;         // Ÿ�� ���� ��, ��ȥ�Ǳ���� ��� �ð�
        }

        /****************************************
         * EnemyManager Base
         ****************************************/
        [SerializeField]
        private EnemyObjectPool[]           enemyObjectPools;
        [SerializeField]
        private EnemySpawnTileObjectPool[]  enemySpawnTileObjectPools;
        private Transform                   target;                             // Enemy ���� ��ǥ
        private Queue<Coroutine>            spawnEnemyBackgroundHandleQueue;    // SpawnEnemyBackground�� �ڷ�ƾ �ڵ���
                                                                                // �����ϴ� ť

        [Header("Enemy Spawn")]
        [SerializeField, Tooltip("Normal, Elite, Boss ��")]
        private EnemySpawnInfo[] enemySpawnInfos;

        [Header("Overlap Position")]
        [SerializeField, Tooltip("�ߺ� Ȯ�ο� ���� ����")]
        private float overlapHeight;
        [SerializeField, Tooltip("�ߺ� Ȯ�ο� ���� ������")]
        private float overlapRadius;
        [SerializeField, Tooltip("Pillar ���ӿ�����Ʈ�� ���̾�")]
        private LayerMask pillarLayerMask;
        [SerializeField, Tooltip("Pillar�� ������ ��, ȸ���� ������")]
        private float radiusOfAvoid;

        [Header("Spawn Range")]
        [SerializeField, Tooltip("�� ũ��")]
        private Vector2 mapSize;

        /****************************************
         * Kill Info
         ****************************************/
        private KillCountInfo killCountInfo;

        /****************************************
         * ������Ƽ
         ****************************************/
        public KillCountInfo KillCountInfo => killCountInfo;

        /// <summary>
        /// EnemyManager�� �ν��Ͻ�ȭ �� ����, ȣ��Ǵ� �ʱ�ȣ �޼ҵ�
        /// </summary>
        private void Awake()
        {
            /// 1. �̱��� �۾�
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }

            /// 2. ������Ʈ �� ���� �ʱ�ȭ
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
            /// 1. ��ȯ�� ��� �� ������ƮǮ�� ��ȯ
            foreach (var objectPool in enemyObjectPools)
                objectPool.Reset();

            /// 2. �߻�� ��� ����ü ������Ʈ Ǯ�� ��ȯ
            /// (���� �ð��� ������ �� ��, ���ص��� �� ����)
        }

        /// <summary>
        /// ���������� Enemy�� ��ȯ�ؾ� �� ��, ����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="spawnEnemyClass">��ȯ�� ���� ���</param>
        public void SpawnEnemyBackground(EnemyClass spawnEnemyClass)
        {
            spawnEnemyBackgroundHandleQueue.Enqueue(StartCoroutine(OnSpawnEnemyBackground(spawnEnemyClass)));
        }

        /// <summary>
        /// ���������� Enemy�� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <param name="spawnEnemyClass">��ȯ�� ���� ���</param>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator OnSpawnEnemyBackground(EnemyClass spawnEnemyClass)
        {
            WaitForSeconds waitSpawnEnemyCycleTime = new WaitForSeconds(enemySpawnInfos[(int)spawnEnemyClass].spawnCycleTime);

            while (true)
            {
                /// spawnEnemyClass�� ��ȯ ������ ���� ������ Ȯ���Ѵ�.
                /// 1. spawnCount < numberOfSpawnAtOnce
                /// 2. currentSpawnCount < maxSpawnCount
                /// (��, �� ���� ��� ������ �� ���� ��ȯ�Ѵ�.)
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
        /// SpawnEnemyBackground �۵��� ���ߴ� �޼ҵ�
        /// (��, �۵��� ������� �����.)
        /// </summary>
        public void StopSpawnEnemyBackground()
        {
            StopCoroutine(spawnEnemyBackgroundHandleQueue.Dequeue());
        }

        /// <summary>
        /// Ʈ���ſ� ���� Enemy�� ��ȯ�Ǿ� �� ��, ����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="spawnEnemyClass">��ȯ�� ���� ���</param>
        public void SpawnEnemyEvent(EnemyClass spawnEnemyClass)
        {
            SpawnTile(spawnEnemyClass);
        }

        /// <summary>
        /// �� ��ȯ�� �̸� �˷��ִ� Ÿ��(EnemySpawnTile)�� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <param name="spawnEnemyClass">��ȯ�� ���� ���</param>
        /// <returns>�ڷ�ƾ</returns>
        private void SpawnTile(EnemyClass spawnEnemyClass)
        {
            EnemySpawnPoint spawnPoint = enemySpawnTileObjectPools[(int)spawnEnemyClass].GetObject(GetRandomSpawnPoint());
            StartCoroutine(SpawnEnemy(spawnPoint, spawnEnemyClass));
        }

        /// <summary>
        /// ������ ��ȯ ��ġ�� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <returns>������ ��ȯ ��ġ</returns>
        private Vector3 GetRandomSpawnPoint()
        {
            /// ���� ���� �߽ɿ� �ֱ� ������ �� ũ���� �ݰ��� ���밪�� �ȴ�.
            /// �� ũ���� �ݰ� ��ġ�� ���� ��ȯ�Ǹ� �� ���� �ɸ��� ������ ���� -1�� ũ�Ⱑ ���밪�� �ȴ�.
            float absoluteX = mapSize.x * 0.5f - 1;
            float absoluteZ = mapSize.y * 0.5f - 1;
            float randomX = Random.Range(-absoluteX, absoluteX);
            float randomZ = Random.Range(-absoluteZ, absoluteZ);
            Vector3 randomSpawnPosition = new Vector3(randomX, 1, randomZ);

            /// ���Ƿ� ������ ��ġ�� ��հ� ��ģ�ٸ�
            /// ��ġ�� �ʴ� ��ġ�� ����� ��ġ�� �����Ѵ�.
            if (IsOverlapOnPillar(randomSpawnPosition))
                randomSpawnPosition = CalculateAvoidOverlapPosition(randomSpawnPosition);

            return randomSpawnPosition;
        }

        /// <summary>
        /// ��տ� ��ġ���� Ȯ���ϴ� �޼ҵ�
        /// </summary>
        /// <param name="position">���� ���� ��ġ</param>
        /// <returns></returns>
        private bool IsOverlapOnPillar(Vector3 position)
        {
            /// interactionObjectMask ���̾��ũ�� ��ġ�� ������Ʈ ������ 1�̻��̸�
            /// ��տ� ��ġ�� ���� �ǹ��Ѵ�.
            Vector3 point0 = position;
            Vector3 point1 = position + Vector3.up * overlapHeight;
            int overlapCount = Physics.OverlapCapsuleNonAlloc(point0, point1, overlapRadius, null, pillarLayerMask);

            return overlapCount > 0;
        }

        /// <summary>
        /// �����ġ�� ��ġ�� �ʴ� ��ġ�� ����ϴ� �޼ҵ�
        /// </summary>
        /// <returns>position����, radius ������ ��ŭ�� ���� ��ġ</returns>
        private Vector3 CalculateAvoidOverlapPosition(Vector3 position)
        {
            /// ������ ����
            float angle = Random.Range(0f, 360f);

            /// Sin, Cos�� ���� R=1�� ��, ��ǥ ���� ���� ��
            /// radiusOfAvoid ��������ŭ ���� �̵��� offset ���͸� ���Ѵ�.
            float x = Mathf.Sin(angle);
            float z = Mathf.Cos(angle);
            Vector3 offset = new Vector3(x, 0, z) * radiusOfAvoid;

            /// ���� ��ġ���� offset ��ŭ �̵���Ų ��ġ�� ��ȯ�Ѵ�.
            return position + offset;
        }

        /// <summary>
        /// Enemy�� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <param name="spawnPoint">�� ������ �̸� �˷��ִ� Ÿ��</param>
        /// <param name="spawnEnemyClass">��ȯ�� ���� ���</param>
        /// <returns></returns>
        private IEnumerator SpawnEnemy(EnemySpawnPoint spawnPoint, EnemyClass spawnEnemyClass)
        {
            /// 1. �� ��ȯ ���ð� ��ŭ ���
            WaitForSeconds waitSpawnEnemyDelayTime = new WaitForSeconds(enemySpawnInfos[(int)spawnEnemyClass].spawnDelayTime);
            yield return waitSpawnEnemyDelayTime;

            /// 2. spawnPoint ��ġ�� �� ��ȯ
            Vector3 spawnPosition = spawnPoint.transform.position;
            EnemyBase enemy = enemyObjectPools[(int)spawnEnemyClass].GetObject(spawnPosition);

            /// 3. enemySpawnTile ������ƮǮ�� ��ȯ
            enemySpawnTileObjectPools[(int)spawnEnemyClass].ReturnObject(spawnPoint);
        }

        /// <summary>
        /// �� ��ȯ �������̽�
        /// </summary>
        /// <param name="returnEnemy"></param>
        public void ReturnEnemy(EnemyBase returnEnemy)
        {
            /// 1. ���� ��޿� �°� ��ȯ�ϰ� óġ �� ������ �����Ѵ�.
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

            /// 2. Normal ��� óġ �� >= ��ǥ óġ�� �̶�� ������ �����Ѵ�.
            if (killCountInfo.normal >= GameManager.Instance.GoalNumberOfKill)
                GameManager.Instance.GameOver();

            /// 3. killCount�� 10�� ����� ������
            if (killCountInfo.normal % 10 == 0)
            {
                enemySpawnInfos[(int)EnemyClass.Normal].numberOfSpawnAtOnce++;
                SpawnEnemyEvent(EnemyClass.Elite);
            }
        }
    }
}