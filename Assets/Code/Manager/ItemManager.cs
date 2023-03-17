using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.Item.Kit;
using WhalePark18.Manager;

namespace WhalePark18.Item
{
    /// <summary>
    /// 생성해야 할 아이템 정보를 담은 구조체
    /// </summary>
    [System.Serializable]
    public struct ItemInfo
    {
        public GameObject itemPrefab;
        [Range(1, 100), Tooltip("아이템 확률(%)")]
        public int weight;
    }

    /// <summary>
    /// 게임에서 생성되는 모든 아이템을 관리하는 관리자
    /// </summary>
    /// <remarks>
    /// 게임 데모 완성을 위해 오브젝트풀을 사용하지 않는 버전으로
    /// 개발 진행, 추후 오브젝트풀과 LogManager를 이용한 로그를 남기는
    /// 버전으로 진행할 것
    /// </remarks>
    public class ItemManager : MonoSingleton<ItemManager>
    {
        [SerializeField]
        private  ItemInfo[] itemList;

        private void Awake()
        {
            CreateInstance();
        }

        /// <summary>
        /// ItemInfo에 설정된 가중치에 의한 확률로 아이템을 반환하는 메소드
        /// </summary>
        /// <returns>생성될 아이템</returns>
        public GameObject GetRandomItem()
        {
            float total = 0;
        
            foreach(ItemInfo element in itemList)
            {
                total += element.weight;
            }
        
            float randomPoint = Random.value * total;
        
            for(int i = 0; i < itemList.Length; i++)
            {
                if(randomPoint < itemList[i].weight)
                {
                    return itemList[i].itemPrefab;
                }
                else
                {
                    randomPoint -= itemList[i].weight;
                }
            }
        
            return itemList[itemList.Length - 1].itemPrefab;
        }
    }
}