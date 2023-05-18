using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WhalePark18.Item.Kit;
using WhalePark18.Manager;

namespace WhalePark18.Item
{
    public enum ItemCategory { None = -1, Survial, Heal, Active, }

    [System.Serializable, Tooltip("������ Ȯ�� ����")]
    public struct ItemProbabilityInfo
    {
        public ItemCategory category;
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
        [Header("Item Info")]
        [SerializeField]
        private List<ItemProbabilityInfo> itemProbabilityInfoList;

        private List<List<GameObject>> listByItemCategory;  // ��� ������ ����Ʈ�� �����ϴ� ��Ʈ ����Ʈ
        [SerializeField]
        private List<GameObject> survivalItemList;          // ���� ���� ������ ����Ʈ
        [SerializeField]
        private List<GameObject> healItemList;              // �� ���� ������ ����Ʈ
        [SerializeField]
        private List<GameObject> activeItemList;            // Ȱ��ȭ ������ ����Ʈ

        [Header("DEBUG")]
        public TMP_InputField inputSimulationCount;
        public Button buttonExecute;
        public List<TextMeshProUGUI> textResultList;

        protected override void Awake()
        {
            /// 1. �̱��� �۾�
            base.Awake();

            /// ������ ����Ʈ���� �����ϱ� ���� listByItemCategory ������ ����
            listByItemCategory = new List<List<GameObject>>();
            listByItemCategory.Add(survivalItemList);
            listByItemCategory.Add(healItemList);
            listByItemCategory.Add(activeItemList);

            // #DEBUG
            //buttonExecute.onClick.AddListener(OnClickDebugSimulation);
        }

        /// <summary>
        /// ItemInfo�� ������ ����ġ�� ���� Ȯ���� �������� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <returns>������ ������</returns>
        public GameObject GetRandomItem()
        {
            float total = 0;
        
            foreach(var element in itemProbabilityInfoList)
            {
                total += element.weight;
            }
        
            float randomPoint = Random.value * total;

            for(int i = 0; i < itemProbabilityInfoList.Count; i++)
            {
                if(randomPoint < itemProbabilityInfoList[i].weight)
                {
                    return ReturnItem(itemProbabilityInfoList[i].category);
                }
                else
                {
                    randomPoint -= itemProbabilityInfoList[i].weight;
                }
            }
        
            return ReturnItem(itemProbabilityInfoList[itemProbabilityInfoList.Count - 1].category);
        }

        private GameObject ReturnItem(ItemCategory category)
        {
            LogManager.ConsoleDebugLog("ItemManagerReturnItem()", $"category: {category}");

            /// category�� ItemCategory.None�� ��� null�� ��ȯ������
            /// �ƴ� ��� category�� ���� ������ �� �ϳ��� ������ ��ȯ.
            if (category.Equals(ItemCategory.None))
            {
                return null;
            }
            else
            {
                int randomIndex = Random.Range(0, listByItemCategory[(int)category].Count - 1);
                return listByItemCategory[(int)category][randomIndex];
            }
        }

        /// <summary>
        /// �ùķ��̼ǿ� �޼ҵ�
        /// </summary>
        public void OnClickDebugSimulation()
        {
            LogManager.ConsoleDebugLog("ItemManager", "OnClickDebugSimulation");

            int[] simulationResult = new int[4];
            

            for(int count = 0; count < int.Parse(inputSimulationCount.text); count++)
            {
                float total = 0;
                foreach (var element in itemProbabilityInfoList)
                {
                    total += element.weight;
                }

                float randomPoint = Random.value * total;
                for (int i = 0; i < itemProbabilityInfoList.Count; i++)
                {
                    if (randomPoint < itemProbabilityInfoList[i].weight)
                    {
                        LogManager.ConsoleDebugLog("ItemManager", $"category: {itemProbabilityInfoList[i].category}");
                        simulationResult[(int)itemProbabilityInfoList[i].category]++;
                        break;
                    }
                    else
                    {
                        randomPoint -= itemProbabilityInfoList[i].weight;
                    }
                }

                //simulationResult[itemProbabilityInfoList.Count - 1]++;
            }

            DebugEditSimulationResult(
                simulationResult[(int)ItemCategory.None + 1],
                simulationResult[(int)ItemCategory.Survial + 1],
                simulationResult[(int)ItemCategory.Heal + 1],
                simulationResult[(int)ItemCategory.Active + 1]
            );
        }

        /// <summary>
        /// �ùķ��̼� ��� Ȯ�ο� �޼ҵ�
        /// </summary>
        private void DebugEditSimulationResult(int none, int survival, int heal, int active)
        {
            textResultList[(int)ItemCategory.None + 1].text = none.ToString();
            textResultList[(int)ItemCategory.Survial + 1].text = survival.ToString();
            textResultList[(int)ItemCategory.Heal + 1].text = heal.ToString();
            textResultList[(int)ItemCategory.Active + 1].text = active.ToString();
        }
    }
}