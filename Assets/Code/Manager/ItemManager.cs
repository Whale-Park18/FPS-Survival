using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.Item.Kit;
using WhalePark18.Manager;

namespace WhalePark18.Item
{
    /// <summary>
    /// �����ؾ� �� ������ ������ ���� ����ü
    /// </summary>
    [System.Serializable]
    public struct ItemInfo
    {
        public GameObject itemPrefab;
        [Range(1, 100), Tooltip("������ Ȯ��(%)")]
        public int weight;
    }

    /// <summary>
    /// ���ӿ��� �����Ǵ� ��� �������� �����ϴ� ������
    /// </summary>
    /// <remarks>
    /// ���� ���� �ϼ��� ���� ������ƮǮ�� ������� �ʴ� ��������
    /// ���� ����, ���� ������ƮǮ�� LogManager�� �̿��� �α׸� �����
    /// �������� ������ ��
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
        /// ItemInfo�� ������ ����ġ�� ���� Ȯ���� �������� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <returns>������ ������</returns>
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