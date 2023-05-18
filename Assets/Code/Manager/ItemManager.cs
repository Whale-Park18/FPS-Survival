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

    [System.Serializable, Tooltip("아이템 확률 정보")]
    public struct ItemProbabilityInfo
    {
        public ItemCategory category;
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
        [Header("Item Info")]
        [SerializeField]
        private List<ItemProbabilityInfo> itemProbabilityInfoList;

        private List<List<GameObject>> listByItemCategory;  // 모든 아이템 리스트를 관리하는 루트 리스트
        [SerializeField]
        private List<GameObject> survivalItemList;          // 생존 관련 아이템 리스트
        [SerializeField]
        private List<GameObject> healItemList;              // 힐 관련 아이템 리스트
        [SerializeField]
        private List<GameObject> activeItemList;            // 활성화 아이템 리스트

        [Header("DEBUG")]
        public TMP_InputField inputSimulationCount;
        public Button buttonExecute;
        public List<TextMeshProUGUI> textResultList;

        protected override void Awake()
        {
            /// 1. 싱글톤 작업
            base.Awake();

            /// 아이템 리스트들을 관리하기 쉽게 listByItemCategory 변수에 저장
            listByItemCategory = new List<List<GameObject>>();
            listByItemCategory.Add(survivalItemList);
            listByItemCategory.Add(healItemList);
            listByItemCategory.Add(activeItemList);

            // #DEBUG
            //buttonExecute.onClick.AddListener(OnClickDebugSimulation);
        }

        /// <summary>
        /// ItemInfo에 설정된 가중치에 의한 확률로 아이템을 반환하는 메소드
        /// </summary>
        /// <returns>생성될 아이템</returns>
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

            /// category가 ItemCategory.None일 경우 null을 반환하지만
            /// 아닐 경우 category의 속한 아이템 중 하나를 선택해 반환.
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
        /// 시뮬레이션용 메소드
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
        /// 시뮬레이션 결과 확인용 메소드
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