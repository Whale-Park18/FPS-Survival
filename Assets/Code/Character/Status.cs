using System.Collections;
using System.Reflection;
using UnityEngine;

namespace WhalePark18.Character
{

    [System.Serializable]
    public class CoinEvent : UnityEngine.Events.UnityEvent<int> { }

    [System.Serializable]
    public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

    [System.Serializable]
    public class ShieldEvent : UnityEngine.Events.UnityEvent<int> { }

    public class Status : MonoBehaviour
    {
        [HideInInspector]
        public CoinEvent onCoinEvent = new CoinEvent();
        [HideInInspector]
        public HPEvent onHPEvent = new HPEvent();
        [HideInInspector]
        public ShieldEvent onShieldEvent = new ShieldEvent();

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
        private float delayHPResilience = 10;           // HP ȸ�� ���� �ð�
        [SerializeField]
        private int startHPResilienceForSec = 0;        // ���� ���� �ʴ� HP ȸ����
        private int currentHPResilienceForSec;          // ���� �ʴ� HP ȸ����

        [Header("Shield")]
        [SerializeField]
        private int startShield = 0;                    // ���� ���� �ǵ差
        private int currentShield;                      // ���� �ǵ差
        private int maxShield;                          // �ִ� �ǵ差

        [SerializeField]
        private float delayShieldResilience;            // �ǵ� ȸ�� ���� �ð�
        [SerializeField]
        private float startShieldResilienceForSec = 0f; // ���� ���� �ʴ� �ǵ� ȸ�� �ۼ�Ʈ
        private float currentShieldResilienceForSec;    // ���� �ʴ� �ǵ� ȸ�� �ۼ�Ʈ

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
        /// Coin Property
        /// </summary>
        public int CurrentCoin => currentCoin;

        /// <summary>
        /// HP Property
        /// </summary>
        public int MaxHP => maxHP;
        public int CurrentHP => currentHP;

        /// <summary>
        /// �ǵ� Property
        /// </summary>
        public int MaxShield => maxShield;
        public int CurrentShield => currentShield;
        public bool shieldActive => maxShield > 0;

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
            currentHPResilienceForSec = startHPResilienceForSec;

            /// �ǵ�
            maxShield = currentShield = startShield;
            currentShieldResilienceForSec = startShieldResilienceForSec;

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
        /// <param name="hp">ȸ����</param>
        public void IncreaseHP(int hp)
        {
            int previousHP = currentHP;

            currentHP = currentHP + hp > maxHP ? maxHP : currentHP + hp;

            onHPEvent.Invoke(previousHP, currentHP);
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
        /// HP ȸ���� Ȱ��ȭ �������̽�
        /// </summary>
        public void StartHPResilience()
        {
            print("StartHPResilience");
            StartCoroutine("OnHPResilience");
        }

        /// <summary>
        /// HP ȸ���� ��Ȱ��ȭ �������̽�
        /// </summary>
        public void StopHPResilience()
        {
            print("StopHPResilience");
            StopCoroutine("OnHPResilience");
        }

        /// <summary>
        /// HP ȸ���� �޼ҵ�
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnHPResilience()
        {
            print("OnHPResilience(): Wait Delay");
            yield return new WaitForSeconds(delayHPResilience);
            print("OnHPResilience(): complated Delay");

            while (currentHP < maxHP)
            {
                print(currentHP + " / " + maxHP);
                IncreaseHP(1);
                yield return new WaitForSeconds(1);
            }
        }

        /// <summary>
        /// �ǵ� ���� �������̽�
        /// </summary>
        /// <param name="shield">ȸ����</param>
        public void IncreaseShield(int shield)
        {
            int previousShield = currentShield;

            currentShield = currentShield + shield > maxShield ? maxShield : currentShield + shield;

            // TODO: onShieldEvent ��ü ����
            onHPEvent.Invoke(previousShield, currentHP);
        }

        /// <summary>
        /// �ǵ� ���� �������̽�
        /// </summary>
        /// <param name="damage">���ط�</param>
        /// <returns>��� ����</returns>
        public bool DecreaseShield(int damage)
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

        public void StartShieldResilience()
        {
            StartCoroutine("OnShieldResilience");
        }

        public void StopShieldResilience()
        {
            StopCoroutine("OnShieldResilience");
        }

        private IEnumerator OnShieldResilience()
        {
            yield return new WaitForSeconds(delayShieldResilience);

            while (currentShield < maxShield)
            {
                IncreaseHP(1);
                yield return new WaitForSeconds(1);
            }
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