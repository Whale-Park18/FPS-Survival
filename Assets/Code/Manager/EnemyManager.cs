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
        private int currentEnemyCount;  // ���� ��ȯ�� ���� ��
        [SerializeField, Tooltip("��ȯ�� ���� �ִ� ��")]
        private int maxEnemyCount;
        [SerializeField, Tooltip("�� ���� ��ȯ�Ǵ� ���� ��")]
        private int numberOfEnemiesSpawnedAtOnce;
        [SerializeField, Tooltip("�� ��ȯ �ֱ�")]
        private float enemySpawnCycleTime;
        [SerializeField, Tooltip("Ÿ�� ���� ��, ���� ��ȯ�ϱ���� ��� �ð�")]
        private float enemySpawnDelayTime;

        [Header("Overlap Position")]
        [SerializeField, Tooltip("enemy ĸ�� �ݶ��̴��� ����")]
        private float enemyHeight;
        [SerializeField, Tooltip("enemy ĸ�� �ݶ��̴��� ������")]
        private float enemyRadius;
        [SerializeField, Tooltip("Pillar ���ӿ�����Ʈ�� ���̾�")]
        private LayerMask pillarLayerMask;
        [SerializeField, Tooltip("Pillar�� ������ ��, ȸ���� ������")]
        private float radiusOfAvoid;

        [Header("Spawn Range")]
        [SerializeField, Tooltip("�� ũ��")]
        private Vector2 mapSize;

        private Transform target;   // Enemy ���� ��ǥ
        private int killCount;      // Enemy óġ��

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
        /// �� ��ȯ�� �̸� �˷��ִ� Ÿ��(EnemySpawnTile)�� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator SpawnTile()
        {
            while(true)
            {
                /// ���ÿ� numberOfEnemiesSpawnedAtOnce ���ڸ�ŭ ���� ��ȯ�Ѵ�.
                /// ��, maxEnemyCount�� ���� ���� ����.
                for (int i = 0; i < numberOfEnemiesSpawnedAtOnce && currentEnemyCount < maxEnemyCount; i++)
                {
                    currentEnemyCount++;
                    EnemySpawnPoint spawnPoint = enemySpawnTileObjectPool.GetObject(GetRandomSpawnPoint());
                    StartCoroutine(SpawnEnemy(spawnPoint));
                }

                /// �� ��ȯ �ֱ� �ð����� ���
                yield return new WaitForSeconds(enemySpawnCycleTime);
            }

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

            WhalePark18.Debug.Log(DebugCategory.Debug, "Spawn Tile Position",
                "Enemy{0} SpawnPosition: {1}", currentEnemyCount, randomSpawnPosition);

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
            Vector3 point1 = position + Vector3.up * enemyHeight;
            int overlapCount = Physics.OverlapCapsuleNonAlloc(point0, point1, enemyRadius, null, pillarLayerMask);

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
        /// ���� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <param name="enemySpawnPoint">�� ������ �̸� �˷��ִ� Ÿ��</param>
        /// <returns></returns>
        private IEnumerator SpawnEnemy(EnemySpawnPoint enemySpawnPoint)
        {
            /// �� ��ȯ ���ð� ��ŭ ���
            yield return new WaitForSeconds(enemySpawnDelayTime);

            /// enemySpawnPoint ��ġ�� �� ��ȯ
            Vector3 spawnPosition = enemySpawnPoint.transform.position;
            //spawnPosition.y = 0;
            WhalePark18.Debug.Log(DebugCategory.Debug, "Spawn Enemy Position",
                "Enemy{0} SpawnPosition: {1}", currentEnemyCount, spawnPosition);

            EnemyFSM enemyFSM = enemyObjectPool.GetObject(spawnPosition);
            enemyFSM.Setup(target);

            /// enemySpawnTile ������ƮǮ�� ��ȯ
            enemySpawnTileObjectPool.ReturnObject(enemySpawnPoint);
        }

        /// <summary>
        /// �� ��ȯ �������̽�
        /// </summary>
        /// <param name="returnEnemyFSM"></param>
        public void ReturnEnemy(EnemyFSM returnEnemyFSM)
        {
            /// ������Ʈ�� ��ȯ�ϰ� ���� �� ���� óġ���� �����Ѵ�.
            enemyObjectPool.ReturnObject(returnEnemyFSM);
            currentEnemyCount--;
            killCount++;

            if (KillCount == GameManager.Instance.KillGoals)
                GameManager.Instance.GameOver();

            /// killCount�� 10�� ����� ������
            /// �ѹ��� ��ȯ�Ǵ� ���� ���� ������Ų��.
            if (killCount % 10 == 0)
                numberOfEnemiesSpawnedAtOnce++;
        }
    }
}