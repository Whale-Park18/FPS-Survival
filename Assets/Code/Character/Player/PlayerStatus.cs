using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using WhalePark18.UI.Window.Game;

namespace WhalePark18.Character.Player
{
    [System.Serializable]
    public class CoinEvent : UnityEngine.Events.UnityEvent<int> { }

    [System.Serializable]
    public class HPEvent : UnityEngine.Events.UnityEvent<int, int> { }

    [System.Serializable]
    public class ShieldEvent : UnityEngine.Events.UnityEvent<int> { }

    /// <summary>
    /// ���� ��뷮 ����
    /// </summary>
    [System.Serializable]
    public struct CoinUsageInfo
    {
        public int baseCoinUsage;      // �⺻ ���� ��뷮
        public int coinUaseIncrement;  // ���� ��뷮 ������
    }

    public class PlayerStatus : Status
    {
        [HideInInspector]
        public CoinEvent onCoinEvent = new CoinEvent();
        [HideInInspector]
        public HPEvent onHPEvent = new HPEvent();
        [HideInInspector]
        public ShieldEvent onShieldEvent = new ShieldEvent();

        /// <summary>
        /// ����� ����
        /// </summary>
        [SerializeField]
        private Ability hpTick;                 // [�ɷ�ġ] ������� �ڿ� ȸ����

        /// <summary>
        /// ��ȣ�� ����
        /// </summary>
        [SerializeField]
        private Ability shield;                 // [�ɷ�ġ] ��ȣ�� �ɷ�ġ
        [SerializeField]
        private Ability shieldTick;             // [�ɷ�ġ] ��ȣ���� �ڿ� ȸ����

        /// <summary>
        /// ü�� �� ��ȣ�� ����
        /// </summary>
        [SerializeField]
        private float tickDelayTime = 5;        // �ڿ� ȸ�� Ȱ��ȭ �����ð�

        /// <summary>
        /// ���� ����
        /// </summary>
        [SerializeField]
        private Ability attackDamage;           // [�ɷ�ġ] ���ݷ�
        [SerializeField]
        private Ability attackSpeed;            // [�ɷ�ġ] ���� �ӵ�
        [SerializeField]
        private Ability attackNumberOfPiercing; // [�ɷ�ġ] �� �����(����)

        /// <summary>
        /// ������ ����
        /// </summary>
        [SerializeField]
        private Ability itemEfficiency;         // [�ɷ�ġ] ������ ȿ�� �ɷ�ġ

        [Header("Coin")]
        [SerializeField]
        private int currentCoin = 0;            // ����
        [SerializeField, Tooltip("Ư������ ����ϴ� ���� ��뷮 ����")]
        private CoinUsageInfo coinUsageInfo;

        /// <summary>
        /// Ability ������Ƽ
        /// </summary>
        public ref Ability HpTick => ref hpTick;
        public ref Ability Shield => ref shield;
        public ref Ability ShieldTick => ref shieldTick;
        public ref Ability AttackDamage => ref attackDamage;
        public ref Ability AttackSpeed => ref attackSpeed;
        public ref Ability AttackNunberOfPiercing => ref attackNumberOfPiercing;
        public ref Ability ItemEfficiency => ref itemEfficiency;
        public bool ShieldActive => shield.maxAbility > 0;                  // ��ȣ�� Ȱ��ȭ ����

        /// <summary>
        /// Coin ������Ƽ
        /// </summary>
        public int CurrentCoin => currentCoin;
        public CoinUsageInfo CoinUsageInfo => coinUsageInfo;

        override protected void Awake()
        {
            base.Awake();
            OnInitialized();
        }

        public override void OnInitialized()
        {
            /// �����
            base.OnInitialized();
            hpTick.OnInitialized();

            /// ��ȣ��
            shield.OnInitialized();
            shieldTick.OnInitialized();

            /// ����
            attackDamage.OnInitialized();
            attackSpeed.OnInitialized();
            attackNumberOfPiercing.OnInitialized();

            /// ������
            itemEfficiency.OnInitialized();
        }

        /// <summary>
        /// [�������̽�] ����� ���� �޼ҵ�
        /// </summary>
        /// <param name="incrementValue">ȸ����</param>
        public override void IncreaseHp(int incrementValue)
        {
            int previousHP = (int)hp.currentAbility;
            hp.currentAbility = hp.currentAbility + incrementValue > hp.maxAbility ? hp.maxAbility : hp.currentAbility + incrementValue;
            
            /// �̺�Ʈ Ȱ��ȭ
            onHPEvent.Invoke(previousHP, (int)hp.currentAbility);
        }

        /// <summary>
        /// [�������̽�] ����� ���� �޼ҵ�
        /// </summary>
        /// <param name="lossValue">���ط�</param>
        /// <returns>��� ����</returns>
        public override bool DecreaseHp(int lossValue)
        {
            int previousHP = (int)hp.currentAbility;
            hp.currentAbility = hp.currentAbility - lossValue > 0 ? hp.currentAbility - lossValue : 0;

            /// �̺�Ʈ Ȱ��ȭ
            onHPEvent.Invoke(previousHP, (int)hp.currentAbility);

            /// �ڿ� ȸ���� Ȱ��ȭ
            DisableHPTick();
            EnableHPTick();

            if (hp.currentAbility <= 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// ������� �ڿ� ȸ���� Ȱ��ȭ �޼ҵ�
        /// </summary>
        private void EnableHPTick()
        {
            /// ���� "ü�� �ڿ� ȸ��" �ɷ�ġ�� 0 �ʰ��� ���� �۵��ϵ��� ��.
            if (hpTick.currentAbility > 0)
                StartCoroutine("OnHPTick");
        }

        /// <summary>
        /// ������� �ڿ� ȸ���� ��Ȱ��ȭ �޼ҵ�
        /// </summary>
        private void DisableHPTick()
        {
            /// ���� "ü�� �ڿ� ȸ��" �ɷ�ġ�� 0 �ʰ��� ���� �۵��ϵ��� ��.
            if(hpTick.currentAbility > 0)
                StopCoroutine("OnHPTick");
        }

        /// <summary>
        /// ������� �ڿ� ȸ���� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        /// <remarks>
        /// tickDelayTime�� ��ŭ ��� ��, 1�ʸ��� hpTick.currentAbility��ŭ ȸ���Ѵ�.
        /// (��, hp.currentAbility�� hp.maxAbility���� ȸ���� ��ŭ�̴�.)
        /// </remarks>
        private IEnumerator OnHPTick()
        {
            yield return new WaitForSeconds(tickDelayTime);

            while (hp.currentAbility < hp.maxAbility)
            {
                print(hp.currentAbility + " / " + hp.maxAbility);
                IncreaseHp((int)hpTick.currentAbility);
                yield return new WaitForSeconds(1);
            }
        }

        /// <summary>
        /// �ǵ� ���� �޼ҵ�
        /// </summary>
        /// <param name="incrementValue">ȸ����</param>
        public void IncreaseShield(int incrementValue)
        {
            int previousShield = (int)shield.currentAbility;

            shield.currentAbility = shield.currentAbility + incrementValue > shield.maxAbility ? shield.maxAbility : shield.currentAbility + incrementValue;

            onShieldEvent.Invoke((int)shield.currentAbility);
        }

        /// <summary>
        /// �ǵ� ���� �޼ҵ�
        /// </summary>
        /// <param name="lossValue">���ط�</param>
        /// <returns>��ȣ���� ũ�⸦ �ʰ��� ���ط�</returns>
        public int DecreaseShield(int lossValue)
        {
            int previousShield = (int)shield.currentAbility;

            shield.currentAbility = shield.currentAbility - lossValue > 0 ? shield.currentAbility - lossValue : 0;

            onShieldEvent.Invoke((int)shield.currentAbility);

            /// ��ȣ���� �ı��Ǹ�
            if (shield.currentAbility <= 0)
            {
                DisableShieldTick();
                EnableShieldTick();

                return lossValue - previousShield;
            }

            return 0;
        }

        /// <summary>
        /// ��ȣ���� �ڿ� ȸ���� Ȱ��ȭ �޼ҵ�
        /// </summary>
        private void EnableShieldTick()
        {
            StartCoroutine("OnShieldTick");
        }

        /// <summary>
        /// ��ȣ���� �ڿ� ȸ���� ��Ȱ��ȭ �޼ҵ�
        /// </summary>
        private void DisableShieldTick()
        {
            StopCoroutine("OnShieldTick");
        }

        /// <summary>
        /// ��ȣ���� �ڿ� ȸ���� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        /// <remarks>
        /// tickDelayTime�� ��ŭ ��� ��, 1�ʸ��� shieldTick.currentAbility��ŭ ȸ���Ѵ�.
        /// (��, shield.currentAbility�� shield.maxAbility���� ȸ���� ��ŭ�̴�.)
        /// </remarks>
        private IEnumerator OnShieldTick()
        {
            yield return new WaitForSeconds(tickDelayTime);

            while (shield.currentAbility < shield.maxAbility)
            {
                IncreaseShield((int)shieldTick.currentAbility);
                yield return new WaitForSeconds(1);
            }
        }

        /// <summary>
        /// ���� �ӵ� ���� �޼ҵ�
        /// </summary>
        /// <param name="increaseAttackSpeed">���� �ӵ� ������</param>
        public void IncreaseAttackSpeed(float increaseAttackSpeed)
        {
            /// Math.Round(double):double - �Ҽ��� ù° �ڸ� �ݿø�
            /// ������ ȿ��(�Ҽ�) Ư���� ���� ���� �ӵ� ������(�Ҽ�)�� ���ϴ� ���̱� ������
            /// 100�� ���� %������ ��ȯ��(0.xxddd�� -> xx.ddd% ��) �ݿø��Ѵ�.
            print("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color>\n"
                + "������ ������: " + increaseAttackSpeed + " / " + increaseAttackSpeed * 100 + "%\n"
                + "������ ȿ�� ���밪: " + increaseAttackSpeed * itemEfficiency.currentAbility + '\n'
                + "�ݿø���: " + Math.Round(increaseAttackSpeed * itemEfficiency.currentAbility * 100) / 100
            );

            attackSpeed.IncreaseAbility((float)Math.Round(increaseAttackSpeed * itemEfficiency.currentAbility * 100) / 100);
        }

        /// <summary>
        /// ���� �ӵ� ���� �޼ҵ�
        /// </summary>
        /// <param name="disincreaseAttackSpeed">���� �ӵ� ���ҷ�</param>
        public void DisincreaseAttackSpeed(float disincreaseAttackSpeed)
        {
            print("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color>\n"
                + "������ ������: " + disincreaseAttackSpeed + " / " + disincreaseAttackSpeed * 100 + "%\n"
                + "������ ȿ�� ���밪: " + disincreaseAttackSpeed * itemEfficiency.currentAbility + '\n'
                + "�ݿø���: " + Math.Round(disincreaseAttackSpeed * itemEfficiency.currentAbility * 100) / 100
            );

            attackSpeed.DisIncreaseAbility((float)Math.Round(disincreaseAttackSpeed * itemEfficiency.currentAbility * 100) / 100);
        }

        /// <summary>
        /// Ư�� ��ȭ�� �ʿ��� ���� ��뷮�� ����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="trait">Ư�� ����</param>
        /// <returns>���� ��뷮</returns>
        public int CalculateCoinUsage(Trait trait)
        {
            int coinUsage = 0;

            switch (trait)
            {
                case Trait.HpUp:
                    coinUsage = hp.currentTraitLevel >= hp.maxTraitLevel ? 
                        0 : coinUsageInfo.baseCoinUsage + coinUsageInfo.coinUaseIncrement * hp.currentTraitLevel;
                    break;

                case Trait.HpTickUp:
                    coinUsage = hpTick.currentTraitLevel >= hpTick.maxTraitLevel ? 
                        0 : coinUsageInfo.baseCoinUsage + coinUsageInfo.coinUaseIncrement * hpTick.currentTraitLevel;
                    break;

                case Trait.ShieldUp:
                    coinUsage = shield.currentTraitLevel >= shield.maxTraitLevel ? 
                        0 : coinUsageInfo.baseCoinUsage + coinUsageInfo.coinUaseIncrement * shield.currentTraitLevel;
                    break;

                case Trait.ShieldTickUp:
                    coinUsage = shieldTick.currentTraitLevel >= shieldTick.maxTraitLevel ? 
                        0 : coinUsageInfo.baseCoinUsage + coinUsageInfo.coinUaseIncrement * shieldTick.currentTraitLevel;
                    break;

                case Trait.AttackDamageUp:
                    coinUsage = attackDamage.currentTraitLevel >= attackDamage.maxTraitLevel ? 
                        0 : coinUsageInfo.baseCoinUsage + coinUsageInfo.coinUaseIncrement * attackDamage.currentTraitLevel;
                    break;

                case Trait.AttackNumberOfPiercingUp:
                    coinUsage = attackNumberOfPiercing.currentTraitLevel >= attackNumberOfPiercing.maxTraitLevel ? 
                        0 : coinUsageInfo.baseCoinUsage + coinUsageInfo.coinUaseIncrement * attackNumberOfPiercing.currentTraitLevel;
                    break;

                case Trait.ItemEfficiencyUp:
                    coinUsage = itemEfficiency.currentTraitLevel >= itemEfficiency.maxTraitLevel ? 
                        0 : coinUsageInfo.baseCoinUsage + coinUsageInfo.coinUaseIncrement * itemEfficiency.currentTraitLevel;
                    break;
            }

            return coinUsage;
        }

        /// <summary>
        /// ���� ���� �޼ҵ�
        /// </summary>
        /// <param name="coin">ȹ�� ����</param>
        public void IncreaseCoin(int coin)
        {
            currentCoin += coin;
            onCoinEvent.Invoke(currentCoin);
        }

        /// <summary>
        /// ���� ���� �޼ҵ�
        /// </summary>
        /// <param name="coin">���� ����</param>
        /// <returns>���� ���� ����</returns>
        public bool DecreaseCoin(int coin)
        {
            if (currentCoin - coin < 0)
                return false;

            currentCoin -= coin;
            onCoinEvent.Invoke(currentCoin);

            return true;
        }
    }
}