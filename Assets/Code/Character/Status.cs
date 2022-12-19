using System.Collections;
using System.Reflection;
using UnityEngine;

namespace WhalePark18.Character
{

    [System.Serializable]
    public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

    [System.Serializable]
    public class CoinEvent : UnityEngine.Events.UnityEvent<int> { }

    public class Status : MonoBehaviour
    {
        [HideInInspector]
        public HPEvent onHPEvent = new HPEvent();
        [HideInInspector]
        public CoinEvent onCoinEvent = new CoinEvent();

        [Header("Coin")]
        [SerializeField]
        private int startCoin = 0;
        private int currentCoin;

        [Header("Walk, Run Speed")]
        [SerializeField]
        private float walkSpeed;
        [SerializeField]
        private float runSpeed;

        [Header("HP")]
        [SerializeField]
        private int startHP = 100;                      // ���� ���� HP
        private int currentHP;                          // ���� HP
        private int maxHP;                              // �ִ� HP

        [SerializeField]
        private int startHPRecoveryForSec = 0;          // ���� ���� �ʴ� HP ȸ����
        private int currentHPRecoveryForSec;            // ���� �ʴ� HP ȸ����

        [Header("Shield")]
        [SerializeField]
        private int startShield = 0;                    // ���� ���� �ǵ差
        private int currentShield;                      // ���� �ǵ差
        private int maxShield;                          // �ִ� �ǵ差

        [SerializeField]
        private float startShieldRecoveryForSec = 0f;   // ���� ���� �ʴ� �ǵ� ȸ�� �ۼ�Ʈ
        private float currentShieldRecoveryForSec;      // ���� �ʴ� �ǵ� ȸ�� �ۼ�Ʈ

        [Header("Attack")]
        [SerializeField]
        private float startDamagePercentIncrease = 1f;  // ���� ���� ���� % ������
        private float currentDamagePercentIncrease;     // ���� ���� % ������

        [SerializeField]
        private int startPiercing = 0;                  // ���� ���� �����
        private int currentPiercing;                    // ���� �����

        [SerializeField]
        private float startAttackSpeed = 1f;            // ���� ���� ���� �ӵ�
        private float currentAttackSpeed;               // ���� ���� �ӵ�

        [Header("Item")]
        private float startItemEfficiency = 1f;         // ���� ���� ������ ȿ��
        private float currentItemEfficiency;            // ���� ������ ȿ��

        /// <summary>
        /// Walk, Run Property
        /// </summary>
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;

        /// <summary>
        /// HP Property
        /// </summary>
        public int MaxHP => maxHP;
        public int CurrentHP => currentHP;

        /// <summary>
        /// Attack Property
        /// </summary>
        public float CurrentDamagePercentIncrease
        {
            set => currentDamagePercentIncrease = value;
            get => currentDamagePercentIncrease;
        }
        public int CurrentPiercing
        {
            set => currentPiercing = value;
            get => currentPiercing;
        }
        public float CurrentAttackSpeed
        {
            set => currentAttackSpeed = value;
            get => currentAttackSpeed;
        }

        /// <summary>
        /// Item Property
        /// </summary>
        public float CurrentItemEfficiency => currentItemEfficiency;

        private void Awake()
        {
            /// ����
            currentCoin = startCoin;

            /// �����
            maxHP = currentHP = startHP;
            currentHPRecoveryForSec = startHPRecoveryForSec;

            /// �ǵ�
            currentShield = startShield;
            currentShieldRecoveryForSec = startShieldRecoveryForSec;

            /// ����
            currentDamagePercentIncrease = startDamagePercentIncrease;
            currentPiercing = startPiercing;
            currentAttackSpeed = startAttackSpeed;

            /// ������
            currentItemEfficiency = startItemEfficiency;
        }

        /// <summary>
        /// ���� ���� �������̽�
        /// </summary>
        /// <param name="coin">ȹ�� ����</param>
        public void IncreaseCoin(int coin)
        {
            currentCoin += coin;
            onCoinEvent.Invoke(currentCoin);
        }

        /// <summary>
        /// ���� ���� �������̽�
        /// </summary>
        /// <param name="coin">���� ����</param>
        /// <returns>���� ���� ����</returns>
        public bool DecreaseCoin(int coin)
        {
            if (currentCoin - coin < 0)
                return false;

            currentCoin -= coin;
            onCoinEvent.Invoke(coin);

            return true;
        }

        /// <summary>
        /// ü�� ���� �������̽�
        /// </summary>
        /// <param name="damage">���ط�</param>
        /// <returns>��� ����</returns>
        public bool DecreaseHP(int damage)
        {
            int previousHP = currentHP;

            currentHP = currentHP - damage > 0 ? currentHP - damage : 0;

            onHPEvent.Invoke(previousHP, currentHP);

            if (currentHP == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// ü�� ���� �������̽�
        /// </summary>
        /// <param name="hp">ȸ����</param>
        public void IncreaseHP(int hp)
        {
            int previousHP = currentHP;

            currentHP = currentHP + hp > startHP ? startHP : currentHP + hp;

            onHPEvent.Invoke(previousHP, currentHP);
        }

        /// <summary>
        /// ���� �ӵ� ���� �������̽�
        /// </summary>
        /// <param name="increaseAttackSpeed">���� �ӵ� ������</param>
        public void IncreaseAttackSpeed(float increaseAttackSpeed)
        {
            Debug.LogFormat("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color>\n" +
                "������ * ������ ȿ�� %: " + (increaseAttackSpeed * currentItemEfficiency) * 100 + '\n' +
                "�ݿø� ��: " + Mathf.Round((increaseAttackSpeed * currentItemEfficiency) * 100) + '\n' +
                "�Ǽ���: " + Mathf.Round((increaseAttackSpeed * currentItemEfficiency) * 100) / 100);
            // increaseAttackSpeed * 100
            // Mathf.Round(increaseAttackSpeed * 100)
            // Mathf.Round(increaseAttackSpeed * 100) / 100

            /// Mathf.Round(): �Ҽ��� ù°�ڸ����� �ݿø�
            /// ������ ������ ȿ���� ������ ȿ�� ���ȿ� ���� �Ҽ��� ��°�ڸ��� �ݿø� �ؾ� ��
            /// ���� [������ ȿ�� * ������ ȿ�� ����] ���� 100�� ���� �Ҽ��� ��°�ڸ��� 
            /// �Ҽ��� ù°�ڸ��� �ٲٰ� �ݿø��� �� ��, 100�� ���� ������
            currentAttackSpeed += Mathf.Round((increaseAttackSpeed * currentItemEfficiency) * 100) / 100;
        }

        /// <summary>
        /// ���� �ӵ� ���� �������̽�
        /// </summary>
        /// <param name="disincreaseAttackSpeed">���� �ӵ� ���ҷ�</param>
        public void DisincreaseAttackSpeed(float disincreaseAttackSpeed)
        {
            Debug.LogFormat("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color>\n" +
                "������ * ������ ȿ�� %: " + (disincreaseAttackSpeed * currentItemEfficiency) * 100 + '\n' +
                "�ݿø� ��: " + Mathf.Round((disincreaseAttackSpeed * currentItemEfficiency) * 100) + '\n' +
                "�Ǽ���: " + Mathf.Round((disincreaseAttackSpeed * currentItemEfficiency) * 100) / 100);

            currentAttackSpeed -= Mathf.Round((disincreaseAttackSpeed * currentItemEfficiency) * 100) / 100;
        }
    }
}