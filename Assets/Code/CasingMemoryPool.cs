using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject casingPrefab;    // ÅºÇÇ ¿ÀºêÁ§Æ® ÇÁ¸®·¦

    private MemoryPool memoryPool;      // ÅºÇÇ ¸Þ¸ð¸® Ç®

    private void Awake()
    {
        memoryPool = new MemoryPool(casingPrefab);
    }

    public void SpawnCasing(Vector3 position, Vector3 direction)
    {
        GameObject item = memoryPool.ActivePoolItem();
        item.transform.position = position;
        item.transform.rotation = Random.rotation;
        item.GetComponent<Casing>().Setup(memoryPool, direction);
    }
}
