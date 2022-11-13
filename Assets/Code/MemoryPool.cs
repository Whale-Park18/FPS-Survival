using System.Collections.Generic;
using UnityEngine;

public class MemoryPool : MonoBehaviour
{
    /// <summary>
    /// �޸� Ǯ�� �����Ǵ� ������Ʈ ����
    /// </summary>
    private class PoolItem
    {
        public bool         isActive;           // "gameObject"�� Ȱ��ȭ/��Ȱ��ȭ ����
        public GameObject   gameObject;         // ȭ�鿡 ���̴� ���� ���ӿ�����Ʈ
    }

    private int increaseCount = 5;              // ������Ư�� ������ �� Instantiate()�� �߰��Ǵ� ������Ʈ ����
    private int maxCount;                       // ���� ����Ʈ�� �շϵǾ� �ִ� ������Ʈ ����
    private int activeCount;                    // ���� ���ӿ� ���ǰ� �ִ�(Ȱ��ȭ��) ������Ʈ ����

    private GameObject      poolObject;         // ������Ʈ Ǯ������ �����ϴ� ���� ������Ʈ ������
    private List<PoolItem>  poolItemList;       // �����Ǵ� ��� ������Ʈ�� �����ϴ� ����Ʈ

    public int MaxCount     => maxCount;        // �ܺο��� ���� ����Ʈ�� ��ϵǾ� �ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ
    public int ActiveCount  => activeCount;     // �ܺο��� ���� Ȱ��ȭ �Ǿ� �ִ� ������Ʈ ���� Ȯ���� ���� ������Ƽ

    private Vector3 tempPosition = new Vector3(48, 1, 48);  // ������Ʈ�� �ӽ÷� �����Ǵ� ��ġ

    public MemoryPool(GameObject poolObject)
    {
        maxCount        = 0;
        activeCount     = 0;
        this.poolObject = poolObject;

        poolItemList    = new List<PoolItem>();

        InstantiateObjects();
    }

    /// <summary>
    /// increaseCount ������ ������Ʈ ����
    /// </summary>
    public void InstantiateObjects()
    {
        maxCount += increaseCount;

        for(int i=0;i<increaseCount; i++)
        {
            PoolItem poolItem = new PoolItem();

            poolItem.isActive = false;
            poolItem.gameObject = GameObject.Instantiate(poolObject);
            poolItem.gameObject.transform.position = tempPosition;
            poolItem.gameObject.SetActive(false);

            poolItemList.Add(poolItem);
        }
    }

    /// <summary>
    /// ���� �������� ��� ������Ʈ�� ����
    /// </summary>
    /// <remarks>
    /// Destroy() �޼ҵ带 �̿��� ������Ʈ�� ����
    /// ���� �ٲ�ų� ������ ����� �� �ѹ��� �����Ͽ�
    /// ��� ���ӿ�����Ʈ�� �ѹ��� �����Ѵ�.
    /// </remarks>
    public void DestoryObjects()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; i++) 
        {
            GameObject.Destroy(poolItemList[i].gameObject);
        }

        poolItemList.Clear();
    }

    /// <summary>
    /// ���� ��Ȱ��ȭ ������ ������Ʈ �� �ϳ��� Ȱ��ȭ�� ����� ��ȯ
    /// </summary>
    /// <returns>Ȱ��ȭ�� ������Ʈ</returns>
    public GameObject ActivePoolItem()
    {
        if(poolItemList == null) return null;

        /// ���� �����ؼ� �����ϴ� ��� ������Ʈ ������ ���� Ȱ��ȭ ������ ������Ʈ ���� ��
        /// ��� ������Ʈ�� Ȱ��ȭ �����̸� ���ο� ������Ʈ �ʿ�
        if(maxCount == activeCount)
        {
            InstantiateObjects();
        }

        int count = poolItemList.Count;
        for (int i = 0; i < count; i++) 
        {
            PoolItem poolItem = poolItemList[i];

            if(poolItem.isActive == false)
            {
                activeCount++;

                poolItem.isActive = true;
                poolItem.gameObject.SetActive(true);

                return poolItem.gameObject;
            }
        }

        return null;
    }

    /// <summary>
    /// ���� ����� �Ϸ�� ������Ʈ�� ��Ȱ��ȭ ���·� ����
    /// </summary>
    /// <param name="removeObject">��Ȱ��ȭ ���·� ��ȯ�� ���� ������Ʈ</param>
    public void DeactivePoolItem(GameObject removeObject)
    {
        if (poolItemList == null || removeObject == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; i++)
        {
            PoolItem poolItem = poolItemList[i];

            if(poolItem.gameObject == removeObject)
            {
                activeCount--;

                poolItem.gameObject.transform.position = tempPosition;
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return;
            }
        }
    }

    /// <summary>
    /// ���� Ȱ��ȭ ������ ��� ���� ������Ʈ�� ��Ȱ��ȭ ���·� ��ȯ
    /// </summary>
    public void DeactiveAllPoolItems()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for(int i = 0; i < count; i++)
        {
            PoolItem poolItem = poolItemList[i];

            if(poolItem.gameObject != null && poolItem.isActive)
            {
                poolItem.gameObject.transform.position = tempPosition;
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }

        activeCount = 0;
    }
}
