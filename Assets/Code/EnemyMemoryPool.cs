using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    [SerializeField]
    private Transform   target;                             // ���� ��ǥ(�÷��̾�)
    [SerializeField]
    private GameObject  enemySpawnPointPrefab;              // ���� �����ϱ� �� ���� ���� ��ġ�� �˷��ִ� ������
    [SerializeField]
    private GameObject  enemyPrefab;                        // �����Ǵ� �� ������
    [SerializeField]
    private float       enemySpawnTime = 1f;                // �� ���� �ֱ�
    [SerializeField]
    private float       enemySpawnLatency = 1f;             // Ÿ�� ���� �� ���� �����ϱ���� ��� �ð�

    private MemoryPool  spawnPointMemoryPool;               // �� ���� ��ġ�� �˷��ִ� ������Ʈ ����, Ȱ��/��Ȱ�� ����
    private MemoryPool  enemyMemoryPool;                    // �� ����, Ȱ��/��Ȱ�� ����

    private int         numberOfEnemiesSpawnedAtOnce = 1;   // ���ÿ� �����Ǵ� ���� ����
    private Vector2Int  mapSize = new Vector2Int(100, 100); // �� ũ��

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        StartCoroutine("SpawnTile");
    }

    private IEnumerator SpawnTile()
    {
        int currentNumber = 0;
        int maximumNumber = 50;

        while(true)
        {
            /// ���ÿ� numberOfEnemiesSpawnedAtOnce ���ڸ�ŭ ���� �����ǵ��� �ݺ��� ���
            for(int i = 0; i < numberOfEnemiesSpawnedAtOnce; i++)
            {
                GameObject item = spawnPointMemoryPool.ActivePoolItem();

                item.transform.position = new Vector3(Random.Range(-mapSize.x * 0.49f, mapSize.x * 0.49f), 1,
                                                      Random.Range(-mapSize.y * 0.49f, mapSize.y * 0.49f));

                StartCoroutine("SpawnEnemy", item);
            }

            currentNumber++;

            if(currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberOfEnemiesSpawnedAtOnce++;
            }

            yield return new WaitForSeconds(enemySpawnTime);
        }
    }

    private IEnumerator SpawnEnemy(GameObject porint)
    {
        yield return new WaitForSeconds(enemySpawnLatency);

        /// �� ������Ʈ�� �����ϰ�, ���� ��ġ�� point ��ġ�� ����
        GameObject item = enemyMemoryPool.ActivePoolItem();
        item.transform.position = new Vector3(porint.transform.position.x, 0,
                                              porint.transform.position.z);

        item.GetComponent<EnemyFSM>().Setup(target, this);

        // Ÿ�� ������Ʈ(point) ��Ȱ��ȭ
        spawnPointMemoryPool.DeactivePoolItem(porint);
    }

    public void DeactiveEnemy(GameObject enemy)
    {
        enemyMemoryPool.DeactivePoolItem(enemy);
    }
}
