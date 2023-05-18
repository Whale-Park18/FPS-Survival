using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WhalePark18.Character.Player;

namespace WhalePark18.UI.Window.Game
{
    public enum Trait
    {
        HpUp, HpTickUp,
        ShieldUp, ShieldTickUp,
        AttackDamageUp, AttackNumberOfPiercingUp,
        ItemEfficiencyUp
    }

    /// <summary>
    /// 캐릭터 특성 창 클래스
    /// </summary>
    public class WindowTrait : WindowBase
    {
        [Header("Player Status")]
        [SerializeField]
        private PlayerStatus status;

        [Header("Select Trait Panel")]
        [SerializeField]
        private Button[] buttonSelectTraitGroup;    // 특성 선택 버튼 배열

        [Header("Explation Panel")]
        [SerializeField]
        private TextMeshProUGUI textTitle;          // 특성 이름을 표시하는 텍스트
        [SerializeField]
        private TextMeshProUGUI textExplation;      // 특성의 효과를 설명하는 텍스트
        [SerializeField]
        private TextMeshProUGUI textCoinUsage;      // 코인 사용량을 표시하는 텍스트
        [SerializeField]
        private TextMeshProUGUI textLevel;          // 특성의 레벨을 표시하는 텍스트
        [SerializeField]
        private Button buttonEnhance;               // 강화 버튼

        private Trait selectTrait;                  // 특성창에서 선택된 특성

        private void Start()
        {
            ButtonBinding();
        }

        private void OnDisable()
        {
            // #DEBUG
            //Reset();
        }

        /// <summary>
        ///UGUI 버튼과 이벤트를 바인딩하는 메소드
        /// </summary>
        private void ButtonBinding()
        {
            /// 체력 관련 버튼 이벤트 바인딩
            buttonSelectTraitGroup[(int)Trait.HpUp].onClick.AddListener(UpdateExplationOfHpSizeUp);
            buttonSelectTraitGroup[(int)Trait.HpTickUp].onClick.AddListener(UpdateExplationOfHpTickUp);

            /// 보호막 관련 버튼 이벤트 바인딩
            buttonSelectTraitGroup[(int)Trait.ShieldUp].onClick.AddListener(UpdateExplationOfShieldSizeUp);
            buttonSelectTraitGroup[(int)Trait.ShieldTickUp].onClick.AddListener(UpdateExplationOfShieldTickUp);

            /// 공격 관련 버튼 이벤트 바인딩
            buttonSelectTraitGroup[(int)Trait.AttackDamageUp].onClick.AddListener(UpdateExplationOfAttackDamageUp);
            buttonSelectTraitGroup[(int)Trait.AttackNumberOfPiercingUp].onClick.AddListener(UpdateExplationOfAttackNumberOfPiercingUp);

            /// 아이템 관련 버튼 이벤트 바인딩
            buttonSelectTraitGroup[(int)Trait.ItemEfficiencyUp].onClick.AddListener(UpdateExplationOfItemEfficiencyUp);

            /// 강화 관련 버튼 이벤트 바인딩
            buttonEnhance.onClick.AddListener(OnClickEnhance);
        }

        /// <summary>
        /// 체력 증가 특성을 눌렀을 때, 설명을 업데이트하는 메소드
        /// </summary>
        public void UpdateExplationOfHpSizeUp()
        {
            selectTrait = Trait.HpUp;

            textTitle.text = "체력 증가";
            textExplation.text = "체력이 " + status.Hp.CalculateIncrementForTraitLevel(status.Hp.currentTraitLevel) + " 증가합니다.";
            textLevel.text = "LV. " + status.Hp.currentTraitLevel + " / " + status.Hp.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// 체력 회복력 증가 특성을 눌렀을 때, 설명을 업데이트하는 메소드
        /// </summary>
        public void UpdateExplationOfHpTickUp()
        {
            selectTrait = Trait.HpTickUp;

            textTitle.text = "체력 자연 회복력 증가";
            textExplation.text = "체력 자연 회복력이 " + status.HpTick.CalculateIncrementForTraitLevel(status.HpTick.currentTraitLevel) + " 증가합니다.";
            textLevel.text = "LV. " + status.HpTick.currentTraitLevel + " / " + status.HpTick.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// 보호막 증가 특성을 눌렀을 때, 설명을 업데이트하는 메소드
        /// </summary>
        public void UpdateExplationOfShieldSizeUp()
        {
            selectTrait = Trait.ShieldUp;
            
            textTitle.text = "보호막 증가";
            textExplation.text = "보호막이 " + status.Shield.CalculateIncrementForTraitLevel(status.Shield.currentTraitLevel) + " 증가합니다.";
            textLevel.text = "LV. " + status.Shield.currentTraitLevel + " / " + status.Shield.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// 보호막 회복력 증가 특성을 눌렀을 때, 설명을 업데이트하는 메소드
        /// </summary>
        public void UpdateExplationOfShieldTickUp()
        {
            selectTrait = Trait.ShieldTickUp;

            textTitle.text = "보호막 자연 회복력 증가";
            textExplation.text = "보호막 자연 회복력이 " + status.ShieldTick.CalculateIncrementForTraitLevel(status.ShieldTick.currentTraitLevel) * 100 + "% 증가합니다.";
            textLevel.text = "LV. " + status.ShieldTick.currentTraitLevel + " / " + status.ShieldTick.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// 공격력 증가 특성을 눌렀을 때, 설명을 업데이트하는 메소드
        /// </summary>
        public void UpdateExplationOfAttackDamageUp()
        {
            selectTrait = Trait.AttackDamageUp;

            textTitle.text = "공격력 증가";
            textExplation.text = "공격력이 " + status.AttackDamage.CalculateIncrementForTraitLevel(status.AttackDamage.currentTraitLevel) * 100 + "% 증가합니다.";
            textLevel.text = "LV. " + status.AttackDamage.currentTraitLevel + " / " + status.AttackDamage.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// 공격 관통력 증가 특성을 눌렀을 때, 설명을 업데이트하는 메소드
        /// </summary>
        public void UpdateExplationOfAttackNumberOfPiercingUp()
        {
            selectTrait = Trait.AttackNumberOfPiercingUp;

            textTitle.text = "적 관통 증가";
            textExplation.text = "피격되는 적의 숫자가 " + status.AttackNunberOfPiercing.CalculateIncrementForTraitLevel(status.AttackNunberOfPiercing.currentTraitLevel) + " 증가합니다.";
            textLevel.text = "LV. " + status.AttackNunberOfPiercing.currentTraitLevel + " / " + status.AttackNunberOfPiercing.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// 아이템 효율 증가 특성을 눌렀을 때, 설명을 업데이트하는 메소드
        /// </summary>
        public void UpdateExplationOfItemEfficiencyUp()
        {
            selectTrait = Trait.ItemEfficiencyUp;

            textTitle.text = "아이템 효율 증가";
            textExplation.text = "아이템이 " + status.ItemEfficiency.CalculateIncrementForTraitLevel(status.ItemEfficiency.currentTraitLevel) * 100 + "% 증가합니다.";
            textLevel.text = "LV. " + status.ItemEfficiency.currentTraitLevel + " / " + status.ItemEfficiency.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// 강화 버튼 이벤트 메소드
        /// </summary>
        public void OnClickEnhance()
        {
            /// 
            if (IsTraitLevelUperMaxLevel() || status.DecreaseCoin(status.CalculateCoinUsage(selectTrait)) == false) return;

            switch (selectTrait)
            {
                case Trait.HpUp:
                    /// 현재 생명력이 최대 생명력과 같다면 강화됐을 때, 
                    /// 현재 생명력이 최대 생명력과 같기위해 isCurrentAbilityEqualMaxAbility를 활성화시킨다.
                    /// 특성 강화 후, isCurrentAbilityEqualMaxAbility를 다시 비활성화시킨다.
                    if (status.Hp.currentAbility == status.Hp.maxAbility) 
                        status.Hp.isCurrentAbilityEqualMaxAbility = true;
                   
                    status.Hp.TraitLevelUp();
                    UpdateExplationOfHpSizeUp();
                    status.onHPEvent.Invoke((int)status.Hp.currentAbility, (int)status.Hp.currentAbility);

                    status.Hp.isCurrentAbilityEqualMaxAbility = false;
                    break;

                case Trait.HpTickUp:
                    status.HpTick.TraitLevelUp();
                    UpdateExplationOfHpTickUp();
                    break;

                case Trait.ShieldUp:
                    /// 현재 생명력이 최대 생명력과 같다면 강화됐을 때, 
                    /// 현재 생명력이 최대 생명력과 같기위해 isCurrentAbilityEqualMaxAbility를 활성화시킨다.
                    /// 특성 강화 후, isCurrentAbilityEqualMaxAbility를 다시 비활성화시킨다.
                    if(status.Shield.currentAbility == status.Shield.maxAbility)
                        status.Shield.isCurrentAbilityEqualMaxAbility = true;

                    status.Shield.TraitLevelUp();
                    UpdateExplationOfShieldSizeUp();
                    status.onShieldEvent.Invoke((int)status.Shield.currentAbility);

                    status.Shield.isCurrentAbilityEqualMaxAbility = false;
                    break;

                case Trait.ShieldTickUp:
                    status.ShieldTick.TraitLevelUp();
                    UpdateExplationOfShieldTickUp();
                    break;

                case Trait.AttackDamageUp:
                    status.AttackDamage.TraitLevelUp();
                    UpdateExplationOfAttackDamageUp();
                    break;

                case Trait.AttackNumberOfPiercingUp:
                    status.AttackNunberOfPiercing.TraitLevelUp();
                    UpdateExplationOfAttackNumberOfPiercingUp();
                    break;

                case Trait.ItemEfficiencyUp:
                    status.ItemEfficiency.TraitLevelUp();
                    UpdateExplationOfItemEfficiencyUp();
                    break;
            }
        }

        /// <summary>
        /// 현재 특성이 최대치인지 확인하는 메소드
        /// </summary>
        /// <returns>현재 특성이 최대치인지</returns>
        private bool IsTraitLevelUperMaxLevel()
        {
            bool result = false;

            switch(selectTrait)
            {
                case Trait.HpUp:
                    result = status.Hp.currentTraitLevel >= status.Hp.maxTraitLevel;
                    break;

                case Trait.HpTickUp:
                    result = status.HpTick.currentTraitLevel >= status.HpTick.maxTraitLevel;
                    break;

                case Trait.ShieldUp:
                    result = status.Shield.currentTraitLevel >= status.Shield.maxTraitLevel;
                    break;

                case Trait.ShieldTickUp:
                    result = status.ShieldTick.currentTraitLevel >= status.ShieldTick.maxTraitLevel;
                    break;

                case Trait.AttackDamageUp:
                    result = status.AttackDamage.currentTraitLevel >= status.AttackDamage.maxTraitLevel;
                    break;

                case Trait.AttackNumberOfPiercingUp:
                    result = status.AttackNunberOfPiercing.currentTraitLevel >= status.AttackNunberOfPiercing.maxTraitLevel;
                    break;

                case Trait.ItemEfficiencyUp:
                    result = status.ItemEfficiency.currentTraitLevel >= status.ItemEfficiency.maxTraitLevel;
                    break;
            }

            return result;
        }
    }
}