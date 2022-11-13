using System.Collections.Generic;
using UnityEngine;

public class MemoryPool : MonoBehaviour
{
    /// <summary>
    /// 메모리 풀로 관리되는 오브젝트 정보
    /// </summary>
    private class PoolItem
    {
        public bool         isActive;           // "gameObject"의 활성화/비활성화 정보
        public GameObject   gameObject;         // 화면에 보이는 실제 게임오브젝트
    }

    private int increaseCount = 5;              // 오브젝특가 부족할 떄 Instantiate()로 추가되는 오브젝트 개수
    private int maxCount;                       // 현재 리스트에 둥록되어 있는 오브젝트 개수
    private int activeCount;                    // 현재 게임에 사용되고 있는(활성화된) 오브젝트 개수

    private GameObject      poolObject;         // 오브젝트 풀링에서 관리하는 게임 오브젝트 프리팹
    private List<PoolItem>  poolItemList;       // 관리되는 모든 오브젝트를 저장하는 리스트

    public int MaxCount     => maxCount;        // 외부에서 현재 리스트에 등록되어 있는 오브젝트 개수 확인을 위한 프로퍼티
    public int ActiveCount  => activeCount;     // 외부에서 현재 활성화 되어 있는 오브젝트 개수 확인을 위한 프로퍼티

    private Vector3 tempPosition = new Vector3(48, 1, 48);  // 오브젝트가 임시로 보관되는 위치

    public MemoryPool(GameObject poolObject)
    {
        maxCount        = 0;
        activeCount     = 0;
        this.poolObject = poolObject;

        poolItemList    = new List<PoolItem>();

        InstantiateObjects();
    }

    /// <summary>
    /// increaseCount 단위로 오브젝트 생성
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
    /// 현재 관리중인 모든 오브젝트를 삭제
    /// </summary>
    /// <remarks>
    /// Destroy() 메소드를 이용해 오브젝트를 삭제
    /// 씬이 바뀌거나 게임이 종료될 때 한번만 수행하여
    /// 모든 게임오브젝트를 한번에 삭제한다.
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
    /// 현재 비활성화 상태의 오브젝트 중 하나를 활성화로 만들어 반환
    /// </summary>
    /// <returns>활성화된 오브젝트</returns>
    public GameObject ActivePoolItem()
    {
        if(poolItemList == null) return null;

        /// 현재 생성해서 관리하는 모든 오브젝트 개수와 현재 활성화 상태인 오브젝트 개수 비교
        /// 모든 오브젝트가 활성화 상태이면 새로운 오브젝트 필요
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
    /// 현재 사용이 완료된 오브젝트를 비활성화 상태로 설정
    /// </summary>
    /// <param name="removeObject">비활성화 상태로 전환할 게임 오브젝트</param>
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
    /// 현재 활성화 상태인 모든 게임 오브젝트를 비활성화 상태로 전환
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
