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
        private int[] currentTraitsLevel;                   // ���� �Ӽ� ����
        [SerializeField]
        private int maxTraitsLevel = 5;                     // �ִ� �Ӽ� ����
        [SerializeField] 
        private int hpUpCoefficient = 20;                   // ü�� ���� ���
        [SerializeField]
        private int hpTickUpCoefficient = 1;                // ü�� ȸ���� ���
        [SerializeField]
        private int shieldUpCoefficient = 20;               // ��ȣ�� ���� ���
        [SerializeField]
        private float shieldTickUpCoefficient = 0.8f;       // ��ȣ�� ȸ���� ���� ���
        [SerializeField]
        private float attackDamageUpCoefficient = 0.2f;     // ���ݷ� ���� ���
        [SerializeField]
        private int attackPiercingUpCoefficient = 1;        // ����� ���� ���
        [SerializeField]
        private float itemEfficiencyUpCoefficient = 0.1f;   // ������ ȿ�� ���� ���

        [Header("Button")]
        [SerializeField]
        private Button[] buttonElements;                    // �Ӽ� ��ư��

        [Header("Detail")]
        [SerializeField]
        private TextMeshProUGUI textDetailTitle;            // ������ ���� �ؽ�Ʈ
        [SerializeField]
        private TextMeshProUGUI textDetailContent;          // ������ ������ �ؽ�Ʈ
        [SerializeField]
        private TextMeshProUGUI textDetailLevel;            // ������ ���� �ؽ�Ʈ
        [SerializeField]
        private Button buttonEnhance;                       // ��ȭ ��ư

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
        /// ��ư�� �̺�Ʈ ���ε� �޼ҵ�
        /// </summary>
        private void ButtonBinding()
        {
            /// ü�� �Ӽ� ��ư �̺�Ʈ ���ε�
            buttonElements[(int)Traits.HPUp].onClick.AddListener(OnClickHPUp);
            buttonElements[(int)Traits.HPTickUp].onClick.AddListener(OnClickHPResilienceUp);

            /// ��ȣ�� �Ӽ� ��ư �̺�Ʈ ���ε�
            buttonElements[(int)Traits.ShieldUp].onClick.AddListener(OnClickShieldUp);
            buttonElements[(int)Traits.ShieldTickUP].onClick.AddListener(OnClickShieldResilienceUp);

            /// ���� �Ӽ� ��ư �̺�Ʈ ���ε�
            buttonElements[(int)Traits.AttackDamageUp].onClick.AddListener(OnClickDamageUp);
            buttonElements[(int)Traits.AttackPiercingUp].onClick.AddListener(OnClickPiercingUp);

            /// ������ �Ӽ� ��ư �̺�Ʈ ���ε�
            buttonElements[(int)Traits.ItemEfficiencyUp].onClick.AddListener(OnClickItemEfficiencyUp);

            /// ��ȭ ��ư �̺�Ʈ ���ε�
            buttonEnhance.onClick.AddListener(OnClickEnhance);
        }

        /// <summary>
        /// ü�� ���� ��ư �̺�Ʈ �޼ҵ�
        /// </summary>
        public void OnClickHPUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.HPUp);
        }

        /// <summary>
        /// ü�� ȸ���� ��ư �̺�Ʈ �޼ҵ�
        /// </summary>
        public void OnClickHPResilienceUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.HPTickUp);
        }

        /// <summary>
        /// ��ȣ�� ���� ��ư �̺�Ʈ �޼ҵ�
        /// </summary>
        public void OnClickShieldUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.ShieldUp);
        }

        /// <summary>
        /// ��ȣ�� ȸ���� ���� ��ư �̺�Ʈ �޼ҵ�
        /// </summary>
        public void OnClickShieldResilienceUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.ShieldTickUP);
        }

        /// <summary>
        /// ������ ���� ��ư �̺�Ʈ �޼ҵ�
        /// </summary>
        public void OnClickDamageUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.AttackDamageUp);
        }

        /// <summary>
        /// ����� ���� ��ư �̺�Ʈ �޼ҵ�
        /// </summary>
        public void OnClickPiercingUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.AttackPiercingUp);
        }

        /// <summary>
        /// ������ ȿ�� ���� ��ư �̺�Ʈ �޼ҵ�
        /// </summary>
        public void OnClickItemEfficiencyUp()
        {
            print(MethodBase.GetCurrentMethod().Name);
            UpdateDetailPanel(Traits.ItemEfficiencyUp);
        }

        /// <summary>
        /// �� ���� �г� ������Ʈ �޼ҵ�
        /// </summary>
        /// <param name="updateTraits"></param>
        private void UpdateDetailPanel(Traits updateTraits)
        {
            //currentTraits = updateTraits;
            //
            //switch (currentTraits)
            //{
            //    case Traits.HPUp:
            //        textDetailTitle.text = "ü�� ����";
            //        textDetailContent.text = "ü���� " + status.Hp.CalculateIncrementForTraitLevel(currentTraitsLevel[(int)Traits.HPUp] + 1) + " �����մϴ�.";
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.HPUp] + "/" + maxTraitsLevel;
            //        break;
            //
            //    case Traits.HPTickUp:
            //        textDetailTitle.text = "ü�� ȸ���� ����";
            //        textDetailContent.text = "ü�� ȸ������ " + CalculateHPTickUPIncrease(currentTraitsLevel[(int)Traits.HPTickUp] + 1) + " �����մϴ�.";
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.HPTickUp] + "/" + maxTraitsLevel;
            //        break;
            //
            //    case Traits.ShieldUp:
            //        textDetailTitle.text = "��ȣ�� ����";
            //        textDetailContent.text = "��ȣ���� " + CalculateShieldUpIncrease(currentTraitsLevel[(int)Traits.ShieldUp] + 1) + " �����մϴ�.";
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.ShieldUp] + "/" + maxTraitsLevel;
            //        break;
            //
            //    case Traits.ShieldTickUP:
            //        textDetailTitle.text = "��ȣ�� ȸ���� ����";
            //        textDetailContent.text = "��ȣ�� ȸ������ " + CalculateShieldTickUpIncrease(currentTraitsLevel[(int)Traits.ShieldTickUP] + 1) * 100 + "% �����մϴ�.";
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.ShieldTickUP] + "/" + maxTraitsLevel;
            //        break;
            //
            //    case Traits.AttackDamageUp:
            //        textDetailTitle.text = "���ݷ� ����";
            //        textDetailContent.text = "���ݷ��� " + CalculateAttackDamageUpIncrease(currentTraitsLevel[(int)Traits.AttackDamageUp] + 1) * 100 + "% �����մϴ�."; 
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.AttackDamageUp] + "/" + maxTraitsLevel;
            //        break;
            //
            //    case Traits.AttackPiercingUp:
            //        textDetailTitle.text = "����� ����";
            //        textDetailContent.text = "������� " + CalculateAttackPiercingUpIncrease(currentTraitsLevel[(int)Traits.AttackPiercingUp] + 1) + " �����մϴ�.";
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.AttackPiercingUp] + "/" + maxTraitsLevel;
            //        break;
            //
            //    case Traits.ItemEfficiencyUp:
            //        textDetailTitle.text = "������ ȿ�� ����";
            //        textDetailContent.text = "������ ȿ���� " + CalculateItemEfficiencyUp(currentTraitsLevel[(int)Traits.ItemEfficiencyUp] + 1) * 100 + "% �����մϴ�.";
            //        textDetailLevel.text = "LV. " + currentTraitsLevel[(int)Traits.ItemEfficiencyUp] + "/" + maxTraitsLevel;
            //        break;
            //}
        }

        /// <summary>
        /// ��ȭ ��ư �̺�Ʈ �޼ҵ�
        /// </summary>
        public void OnClickEnhance()
        {
            print(MethodBase.GetCurrentMethod().Name);

            if (currentTraitsLevel[(int)currentTraits] >= maxTraitsLevel || status.DecreaseCoin(100) == false)
                return;

            switch(currentTraits)
            {
                case Traits.HPUp:
                    print("ü�� ����");
                    status.Hp.TraitLevelUp();
                    break;

                case Traits.HPTickUp:
                    print("ü�� ȸ���� ����");
                    status.HpTick.TraitLevelUp();
                    break;

                case Traits.ShieldUp:
                    print("��ȣ�� ����");
                    status.ShieldTick.TraitLevelUp();
                    break;

                case Traits.ShieldTickUP:
                    print("��ȣ�� ȸ���� ����");
                    status.ShieldTick.TraitLevelUp();
                    break;

                case Traits.AttackDamageUp:
                    print("���ݷ� ����");
                    status.AttackDamage.TraitLevelUp();
                    break;

                case Traits.AttackPiercingUp:
                    print("����� ����");
                    status.AttackNunberOfPiercing.TraitLevelUp();
                    break;

                case Traits.ItemEfficiencyUp:
                    print("������ ȿ�� ����");
                    status.ItemEfficiency.TraitLevelUp();
                    break;
            }

            UpdateDetailPanel(currentTraits);
        }
    }
}
