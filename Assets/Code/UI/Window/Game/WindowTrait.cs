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
    /// ĳ���� Ư�� â Ŭ����
    /// </summary>
    public class WindowTrait : WindowBase
    {
        [Header("Player Status")]
        [SerializeField]
        private PlayerStatus status;

        [Header("Select Trait Panel")]
        [SerializeField]
        private Button[] buttonSelectTraitGroup;    // Ư�� ���� ��ư �迭

        [Header("Explation Panel")]
        [SerializeField]
        private TextMeshProUGUI textTitle;          // Ư�� �̸��� ǥ���ϴ� �ؽ�Ʈ
        [SerializeField]
        private TextMeshProUGUI textExplation;      // Ư���� ȿ���� �����ϴ� �ؽ�Ʈ
        [SerializeField]
        private TextMeshProUGUI textCoinUsage;      // ���� ��뷮�� ǥ���ϴ� �ؽ�Ʈ
        [SerializeField]
        private TextMeshProUGUI textLevel;          // Ư���� ������ ǥ���ϴ� �ؽ�Ʈ
        [SerializeField]
        private Button buttonEnhance;               // ��ȭ ��ư

        private Trait selectTrait;                  // Ư��â���� ���õ� Ư��

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
        ///UGUI ��ư�� �̺�Ʈ�� ���ε��ϴ� �޼ҵ�
        /// </summary>
        private void ButtonBinding()
        {
            /// ü�� ���� ��ư �̺�Ʈ ���ε�
            buttonSelectTraitGroup[(int)Trait.HpUp].onClick.AddListener(UpdateExplationOfHpSizeUp);
            buttonSelectTraitGroup[(int)Trait.HpTickUp].onClick.AddListener(UpdateExplationOfHpTickUp);

            /// ��ȣ�� ���� ��ư �̺�Ʈ ���ε�
            buttonSelectTraitGroup[(int)Trait.ShieldUp].onClick.AddListener(UpdateExplationOfShieldSizeUp);
            buttonSelectTraitGroup[(int)Trait.ShieldTickUp].onClick.AddListener(UpdateExplationOfShieldTickUp);

            /// ���� ���� ��ư �̺�Ʈ ���ε�
            buttonSelectTraitGroup[(int)Trait.AttackDamageUp].onClick.AddListener(UpdateExplationOfAttackDamageUp);
            buttonSelectTraitGroup[(int)Trait.AttackNumberOfPiercingUp].onClick.AddListener(UpdateExplationOfAttackNumberOfPiercingUp);

            /// ������ ���� ��ư �̺�Ʈ ���ε�
            buttonSelectTraitGroup[(int)Trait.ItemEfficiencyUp].onClick.AddListener(UpdateExplationOfItemEfficiencyUp);

            /// ��ȭ ���� ��ư �̺�Ʈ ���ε�
            buttonEnhance.onClick.AddListener(OnClickEnhance);
        }

        /// <summary>
        /// ü�� ���� Ư���� ������ ��, ������ ������Ʈ�ϴ� �޼ҵ�
        /// </summary>
        public void UpdateExplationOfHpSizeUp()
        {
            selectTrait = Trait.HpUp;

            textTitle.text = "ü�� ����";
            textExplation.text = "ü���� " + status.Hp.CalculateIncrementForTraitLevel(status.Hp.currentTraitLevel) + " �����մϴ�.";
            textLevel.text = "LV. " + status.Hp.currentTraitLevel + " / " + status.Hp.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// ü�� ȸ���� ���� Ư���� ������ ��, ������ ������Ʈ�ϴ� �޼ҵ�
        /// </summary>
        public void UpdateExplationOfHpTickUp()
        {
            selectTrait = Trait.HpTickUp;

            textTitle.text = "ü�� �ڿ� ȸ���� ����";
            textExplation.text = "ü�� �ڿ� ȸ������ " + status.HpTick.CalculateIncrementForTraitLevel(status.HpTick.currentTraitLevel) + " �����մϴ�.";
            textLevel.text = "LV. " + status.HpTick.currentTraitLevel + " / " + status.HpTick.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// ��ȣ�� ���� Ư���� ������ ��, ������ ������Ʈ�ϴ� �޼ҵ�
        /// </summary>
        public void UpdateExplationOfShieldSizeUp()
        {
            selectTrait = Trait.ShieldUp;
            
            textTitle.text = "��ȣ�� ����";
            textExplation.text = "��ȣ���� " + status.Shield.CalculateIncrementForTraitLevel(status.Shield.currentTraitLevel) + " �����մϴ�.";
            textLevel.text = "LV. " + status.Shield.currentTraitLevel + " / " + status.Shield.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// ��ȣ�� ȸ���� ���� Ư���� ������ ��, ������ ������Ʈ�ϴ� �޼ҵ�
        /// </summary>
        public void UpdateExplationOfShieldTickUp()
        {
            selectTrait = Trait.ShieldTickUp;

            textTitle.text = "��ȣ�� �ڿ� ȸ���� ����";
            textExplation.text = "��ȣ�� �ڿ� ȸ������ " + status.ShieldTick.CalculateIncrementForTraitLevel(status.ShieldTick.currentTraitLevel) * 100 + "% �����մϴ�.";
            textLevel.text = "LV. " + status.ShieldTick.currentTraitLevel + " / " + status.ShieldTick.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// ���ݷ� ���� Ư���� ������ ��, ������ ������Ʈ�ϴ� �޼ҵ�
        /// </summary>
        public void UpdateExplationOfAttackDamageUp()
        {
            selectTrait = Trait.AttackDamageUp;

            textTitle.text = "���ݷ� ����";
            textExplation.text = "���ݷ��� " + status.AttackDamage.CalculateIncrementForTraitLevel(status.AttackDamage.currentTraitLevel) * 100 + "% �����մϴ�.";
            textLevel.text = "LV. " + status.AttackDamage.currentTraitLevel + " / " + status.AttackDamage.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// ���� ����� ���� Ư���� ������ ��, ������ ������Ʈ�ϴ� �޼ҵ�
        /// </summary>
        public void UpdateExplationOfAttackNumberOfPiercingUp()
        {
            selectTrait = Trait.AttackNumberOfPiercingUp;

            textTitle.text = "�� ���� ����";
            textExplation.text = "�ǰݵǴ� ���� ���ڰ� " + status.AttackNunberOfPiercing.CalculateIncrementForTraitLevel(status.AttackNunberOfPiercing.currentTraitLevel) + " �����մϴ�.";
            textLevel.text = "LV. " + status.AttackNunberOfPiercing.currentTraitLevel + " / " + status.AttackNunberOfPiercing.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// ������ ȿ�� ���� Ư���� ������ ��, ������ ������Ʈ�ϴ� �޼ҵ�
        /// </summary>
        public void UpdateExplationOfItemEfficiencyUp()
        {
            selectTrait = Trait.ItemEfficiencyUp;

            textTitle.text = "������ ȿ�� ����";
            textExplation.text = "�������� " + status.ItemEfficiency.CalculateIncrementForTraitLevel(status.ItemEfficiency.currentTraitLevel) * 100 + "% �����մϴ�.";
            textLevel.text = "LV. " + status.ItemEfficiency.currentTraitLevel + " / " + status.ItemEfficiency.maxTraitLevel;
            textCoinUsage.text = status.CalculateCoinUsage(selectTrait).ToString();
        }

        /// <summary>
        /// ��ȭ ��ư �̺�Ʈ �޼ҵ�
        /// </summary>
        public void OnClickEnhance()
        {
            /// 
            if (IsTraitLevelUperMaxLevel() || status.DecreaseCoin(status.CalculateCoinUsage(selectTrait)) == false) return;

            switch (selectTrait)
            {
                case Trait.HpUp:
                    /// ���� ������� �ִ� ����°� ���ٸ� ��ȭ���� ��, 
                    /// ���� ������� �ִ� ����°� �������� isCurrentAbilityEqualMaxAbility�� Ȱ��ȭ��Ų��.
                    /// Ư�� ��ȭ ��, isCurrentAbilityEqualMaxAbility�� �ٽ� ��Ȱ��ȭ��Ų��.
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
                    /// ���� ������� �ִ� ����°� ���ٸ� ��ȭ���� ��, 
                    /// ���� ������� �ִ� ����°� �������� isCurrentAbilityEqualMaxAbility�� Ȱ��ȭ��Ų��.
                    /// Ư�� ��ȭ ��, isCurrentAbilityEqualMaxAbility�� �ٽ� ��Ȱ��ȭ��Ų��.
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
        /// ���� Ư���� �ִ�ġ���� Ȯ���ϴ� �޼ҵ�
        /// </summary>
        /// <returns>���� Ư���� �ִ�ġ����</returns>
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