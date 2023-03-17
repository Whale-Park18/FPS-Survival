using TMPro;
using UnityEngine;
using UnityEngine.UI;

using System.Reflection;

namespace WhalePark18.Character.Player
{
    public enum Traits { None = -1, HPUp, HPTickUp, ShieldUp, ShieldTickUP, AttackDamageUp, AttackPiercingUp, ItemEfficiencyUp}

    public class PlayerTraitsButton : MonoBehaviour
    {
        [Header("Traits")]
        private int[] currentTraitsLevel;                   // 현재 속성 레벨
        [SerializeField]
        private int maxTraitsLevel = 5;                     // 최대 속성 레벨
        [SerializeField] 
        private int hpUpCoefficient = 20;                   // 체력 증가 계수
        [SerializeField]
        private int hpTickUpCoefficient = 1;                // 체력 회복력 계수
        [SerializeField]
        private int shieldUpCoefficient = 20;               // 보호막 증가 계수
        [SerializeField]
        private float shieldTickUpCoefficient = 0.8f;       // 보호막 회복력 증가 계수
        [SerializeField]
        private float attackDamageUpCoefficient = 0.2f;     // 공격력 증가 계수
        [SerializeField]
        private int attackPiercingUpCoefficient = 1;        // 관통력 증가 계수
        [SerializeField]
        private float itemEfficiencyUpCoefficient = 0.1f;   // 아이템 효율 증가 계수

        [Header("Button")]
        [SerializeField]
        private Button[] buttonElements;                    // 속성 버튼들

        [Header("Detail")]
        [SerializeField]
        private TextMeshProUGUI textDetailTitle;            // 디테일 제목 텍스트
        [SerializeField]
        private TextMeshProUGUI textDetailContent;          // 디테일 콘텐츠 텍스트
        [SerializeField]
        private TextMeshProUGUI textDetailLevel;            // 디테일 레벨 텍스트
        [SerializeField]
        private Button buttonEnhance;                       // 강화 버튼

        [Header("Components")]
        [SerializeField]
        private PlayerStatus status;

        private Traits currentTraits;

        private void Awake()
        {
            ButtonBinding();
            currentTraitsLevel = new int[buttonElements.Length];
        }

        /// <summary>
        /// 버튼과 이벤트 바인딩 메소드
        /// </summary>
        private void ButtonBinding()
        {
            /// 체력 속성 버튼 이벤트 바인딩
            buttonElements[(int)Traits.HPUp].onClick.AddListener(OnClickHPUp);
            buttonElements[(int)Traits.HPTickUp].onClick.AddListener(OnClickHPResilienceUp);

            /// 보호막 속성 버튼 이벤트 바인딩
            buttonElements[(int)Traits.ShieldUp].onClick.AddListener(OnClickShieldUp);
            buttonElements[(int)Traits.ShieldTickUP].onClick.AddListener(OnClickShieldResilienceUp);

            /// 공격 속성 버튼 이벤트 바인딩
            buttonElements[(int)Traits.AttackDamageUp].onClick.AddListener(OnClickDamageUp);
            buttonElements[(int)Traits.AttackPiercingUp].onClick.AddListener(OnClickPiercingUp);

            /// 아이템 속성 버튼 이벤트 바인딩
            buttonElements[(int)Traits.ItemEfficiencyUp].onClick.AddListener(OnClickItemEfficiencyUp);

            /// 강화 버튼 이벤트 바인딩
            buttonEnhance.onClick.AddListener(OnClickEnhance);
        }

        /// <summary>
        /// 체력 증가 버튼 이벤트 메소드
        /// </summary>
        public void OnClickHPUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.HPUp);
        }

        /// <summary>
        /// 체력 회복력 버튼 이벤트 메소드
        /// </summary>
        public void OnClickHPResilienceUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.HPTickUp);
        }

        /// <summary>
        /// 보호막 증가 버튼 이벤트 메소드
        /// </summary>
        public void OnClickShieldUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.ShieldUp);
        }

        /// <summary>
        /// 보호막 회복력 증가 버튼 이벤트 메소드
        /// </summary>
        public void OnClickShieldResilienceUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.ShieldTickUP);
        }

        /// <summary>
        /// 데미지 증가 버튼 이벤트 메소드
        /// </summary>
        public void OnClickDamageUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.AttackDamageUp);
        }

        /// <summary>
        /// 관통력 증가 버튼 이벤트 메소드
        /// </summary>
        public void OnClickPiercingUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.AttackPiercingUp);
        }

        /// <summary>
        /// 아이템 효율 증가 버튼 이벤트 메소드
        /// </summary>
        public void OnClickItemEfficiencyUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.ItemEfficiencyUp);
        }

        /// <summary>
        /// 상세 정보 패널 업데이트 메소드
        /// </summary>
        /// <param name="updateTraits"></param>
        private void UpdateDetailPanel(Traits updateTraits)
        {
            //currentTraits = updateTraits;
            //
            //switch (currentTraits)
            //{
            //    case Traits.HPUp:
            //        textDetailTitle.text = "체력 증가";
            //        textDetailContent.text = "체력이 " + status.Hp.CalculateIncrementForTraitLevel(currentTraitsLevel[(int)Traits.HPUp] + 1) + " 증가합니다.";
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.HPUp] + "/" + maxTraitsLevel;
            //        break;
            //
            //    case Traits.HPTickUp:
            //        textDetailTitle.text = "체력 회복력 증가";
            //        textDetailContent.text = "체력 회복력이 " + CalculateHPTickUPIncrease(currentTraitsLevel[(int)Traits.HPTickUp] + 1) + " 증가합니다.";
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.HPTickUp] + "/" + maxTraitsLevel;
            //        break;
            //
            //    case Traits.ShieldUp:
            //        textDetailTitle.text = "보호막 증가";
            //        textDetailContent.text = "보호막이 " + CalculateShieldUpIncrease(currentTraitsLevel[(int)Traits.ShieldUp] + 1) + " 증가합니다.";
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.ShieldUp] + "/" + maxTraitsLevel;
            //        break;
            //
            //    case Traits.ShieldTickUP:
            //        textDetailTitle.text = "보호막 회복력 증가";
            //        textDetailContent.text = "보호막 회복력이 " + CalculateShieldTickUpIncrease(currentTraitsLevel[(int)Traits.ShieldTickUP] + 1) * 100 + "% 증가합니다.";
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.ShieldTickUP] + "/" + maxTraitsLevel;
            //        break;
            //
            //    case Traits.AttackDamageUp:
            //        textDetailTitle.text = "공격력 증가";
            //        textDetailContent.text = "공격력이 " + CalculateAttackDamageUpIncrease(currentTraitsLevel[(int)Traits.AttackDamageUp] + 1) * 100 + "% 증가합니다."; 
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.AttackDamageUp] + "/" + maxTraitsLevel;
            //        break;
            //
            //    case Traits.AttackPiercingUp:
            //        textDetailTitle.text = "관통력 증가";
            //        textDetailContent.text = "관통력이 " + CalculateAttackPiercingUpIncrease(currentTraitsLevel[(int)Traits.AttackPiercingUp] + 1) + " 증가합니다.";
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.AttackPiercingUp] + "/" + maxTraitsLevel;
            //        break;
            //
            //    case Traits.ItemEfficiencyUp:
            //        textDetailTitle.text = "아이템 효율 증가";
            //        textDetailContent.text = "아이템 효율이 " + CalculateItemEfficiencyUp(currentTraitsLevel[(int)Traits.ItemEfficiencyUp] + 1) * 100 + "% 증가합니다.";
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.ItemEfficiencyUp] + "/" + maxTraitsLevel;
            //        break;
            //}
        }

        /// <summary>
        /// 강화 버튼 이벤트 메소드
        /// </summary>
        public void OnClickEnhance()
        {
            print(MethodBase.GetCurrentMethod().Name);

            if (currentTraitsLevel[(int)currentTraits] >= maxTraitsLevel || status.DecreaseCoin(100) == false)
                return;

            switch(currentTraits)
            {
                case Traits.HPUp:
                    print("체력 증가");
                    status.Hp.TraitLevelUp();
                    break;

                case Traits.HPTickUp:
                    print("체력 회복력 증가");
                    status.HpTick.TraitLevelUp();
                    break;

                case Traits.ShieldUp:
                    print("보호막 증가");
                    status.ShieldTick.TraitLevelUp();
                    break;

                case Traits.ShieldTickUP:
                    print("보호막 회복력 증가");
                    status.ShieldTick.TraitLevelUp();
                    break;

                case Traits.AttackDamageUp:
                    print("공격력 증가");
                    status.AttackDamage.TraitLevelUp();
                    break;

                case Traits.AttackPiercingUp:
                    print("관통력 증가");
                    status.AttackNunberOfPiercing.TraitLevelUp();
                    break;

                case Traits.ItemEfficiencyUp:
                    print("아이템 효율 증가");
                    status.ItemEfficiency.TraitLevelUp();
                    break;
            }

            UpdateDetailPanel(currentTraits);
        }
    }
}
