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
    /// 코인 사용량 정보
    /// </summary>
    [System.Serializable]
    public struct CoinUsageInfo
    {
        public int baseCoinUsage;      // 기본 코인 사용량
        public int coinUaseIncrement;  // 코인 사용량 증가량
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
        /// 생명력 관련
        /// </summary>
        [SerializeField]
        private Ability hpTick;                 // [능력치] 생명력의 자연 회복력

        /// <summary>
        /// 보호막 관련
        /// </summary>
        [SerializeField]
        private Ability shield;                 // [능력치] 보호막 능력치
        [SerializeField]
        private Ability shieldTick;             // [능력치] 보호막의 자연 회복력

        /// <summary>
        /// 체력 및 보호막 공통
        /// </summary>
        [SerializeField]
        private float tickDelayTime = 5;        // 자연 회복 활성화 지연시간

        /// <summary>
        /// 공격 관련
        /// </summary>
        [SerializeField]
        private Ability attackDamage;           // [능력치] 공격력
        [SerializeField]
        private Ability attackSpeed;            // [능력치] 공격 속도
        [SerializeField]
        private Ability attackNumberOfPiercing; // [능력치] 적 관통력(관통)

        /// <summary>
        /// 아이템 관련
        /// </summary>
        [SerializeField]
        private Ability itemEfficiency;         // [능력치] 아이템 효율 능력치

        [Header("Coin")]
        [SerializeField]
        private int currentCoin = 0;            // 코인
        [SerializeField, Tooltip("특성에서 사용하는 코인 사용량 정보")]
        private CoinUsageInfo coinUsageInfo;

        /// <summary>
        /// Ability 프로퍼티
        /// </summary>
        public ref Ability HpTick => ref hpTick;
        public ref Ability Shield => ref shield;
        public ref Ability ShieldTick => ref shieldTick;
        public ref Ability AttackDamage => ref attackDamage;
        public ref Ability AttackSpeed => ref attackSpeed;
        public ref Ability AttackNunberOfPiercing => ref attackNumberOfPiercing;
        public ref Ability ItemEfficiency => ref itemEfficiency;
        public bool ShieldActive => shield.maxAbility > 0;                  // 보호막 활성화 여부

        /// <summary>
        /// Coin 프로퍼티
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
            /// 생명력
            base.OnInitialized();
            hpTick.OnInitialized();

            /// 보호막
            shield.OnInitialized();
            shieldTick.OnInitialized();

            /// 공격
            attackDamage.OnInitialized();
            attackSpeed.OnInitialized();
            attackNumberOfPiercing.OnInitialized();

            /// 아이템
            itemEfficiency.OnInitialized();
        }

        /// <summary>
        /// [인터페이스] 생명력 증가 메소드
        /// </summary>
        /// <param name="incrementValue">회복량</param>
        public override void IncreaseHp(int incrementValue)
        {
            int previousHP = (int)hp.currentAbility;
            hp.currentAbility = hp.currentAbility + incrementValue > hp.maxAbility ? hp.maxAbility : hp.currentAbility + incrementValue;
            
            /// 이벤트 활성화
            onHPEvent.Invoke(previousHP, (int)hp.currentAbility);
        }

        /// <summary>
        /// [인터페이스] 생명력 감소 메소드
        /// </summary>
        /// <param name="lossValue">피해량</param>
        /// <returns>사망 여부</returns>
        public override bool DecreaseHp(int lossValue)
        {
            int previousHP = (int)hp.currentAbility;
            hp.currentAbility = hp.currentAbility - lossValue > 0 ? hp.currentAbility - lossValue : 0;

            /// 이벤트 활성화
            onHPEvent.Invoke(previousHP, (int)hp.currentAbility);

            /// 자연 회복력 활성화
            DisableHPTick();
            EnableHPTick();

            if (hp.currentAbility <= 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 생명력의 자연 회복력 활성화 메소드
        /// </summary>
        private void EnableHPTick()
        {
            /// 현재 "체력 자연 회복" 능력치가 0 초과할 때만 작동하도록 함.
            if (hpTick.currentAbility > 0)
                StartCoroutine("OnHPTick");
        }

        /// <summary>
        /// 생명력의 자연 회복력 비활성화 메소드
        /// </summary>
        private void DisableHPTick()
        {
            /// 현재 "체력 자연 회복" 능력치가 0 초과할 때만 작동하도록 함.
            if(hpTick.currentAbility > 0)
                StopCoroutine("OnHPTick");
        }

        /// <summary>
        /// 생명력의 자연 회복력 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        /// <remarks>
        /// tickDelayTime초 만큼 대기 후, 1초마다 hpTick.currentAbility만큼 회복한다.
        /// (단, hp.currentAbility가 hp.maxAbility까지 회복할 만큼이다.)
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
        /// 실드 증가 메소드
        /// </summary>
        /// <param name="incrementValue">회복량</param>
        public void IncreaseShield(int incrementValue)
        {
            int previousShield = (int)shield.currentAbility;

            shield.currentAbility = shield.currentAbility + incrementValue > shield.maxAbility ? shield.maxAbility : shield.currentAbility + incrementValue;

            onShieldEvent.Invoke((int)shield.currentAbility);
        }

        /// <summary>
        /// 실드 감소 메소드
        /// </summary>
        /// <param name="lossValue">피해량</param>
        /// <returns>보호막의 크기를 초과한 피해량</returns>
        public int DecreaseShield(int lossValue)
        {
            int previousShield = (int)shield.currentAbility;

            shield.currentAbility = shield.currentAbility - lossValue > 0 ? shield.currentAbility - lossValue : 0;

            onShieldEvent.Invoke((int)shield.currentAbility);

            /// 보호막이 파괴되면
            if (shield.currentAbility <= 0)
            {
                DisableShieldTick();
                EnableShieldTick();

                return lossValue - previousShield;
            }

            return 0;
        }

        /// <summary>
        /// 보호막의 자연 회복력 활성화 메소드
        /// </summary>
        private void EnableShieldTick()
        {
            StartCoroutine("OnShieldTick");
        }

        /// <summary>
        /// 보호막의 자연 회복력 비활성화 메소드
        /// </summary>
        private void DisableShieldTick()
        {
            StopCoroutine("OnShieldTick");
        }

        /// <summary>
        /// 보호막의 자연 회보력 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        /// <remarks>
        /// tickDelayTime초 만큼 대기 후, 1초마다 shieldTick.currentAbility만큼 회복한다.
        /// (단, shield.currentAbility가 shield.maxAbility까지 회복할 만큼이다.)
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
        /// 공격 속도 증가 메소드
        /// </summary>
        /// <param name="increaseAttackSpeed">공격 속도 증가량</param>
        public void IncreaseAttackSpeed(float increaseAttackSpeed)
        {
            /// Math.Round(double):double - 소숫점 첫째 자리 반올림
            /// 아이템 효율(소수) 특성에 의해 공격 속도 증가량(소수)이 변하는 것이기 때문에
            /// 100을 곱해 %값으로 변환해(0.xxddd값 -> xx.ddd% 값) 반올림한다.
            print("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color>\n"
                + "아이템 증폭값: " + increaseAttackSpeed + " / " + increaseAttackSpeed * 100 + "%\n"
                + "아이템 효율 적용값: " + increaseAttackSpeed * itemEfficiency.currentAbility + '\n'
                + "반올림값: " + Math.Round(increaseAttackSpeed * itemEfficiency.currentAbility * 100) / 100
            );

            attackSpeed.IncreaseAbility((float)Math.Round(increaseAttackSpeed * itemEfficiency.currentAbility * 100) / 100);
        }

        /// <summary>
        /// 공격 속도 감소 메소드
        /// </summary>
        /// <param name="disincreaseAttackSpeed">공격 속도 감소량</param>
        public void DisincreaseAttackSpeed(float disincreaseAttackSpeed)
        {
            print("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color>\n"
                + "아이템 증폭값: " + disincreaseAttackSpeed + " / " + disincreaseAttackSpeed * 100 + "%\n"
                + "아이템 효율 적용값: " + disincreaseAttackSpeed * itemEfficiency.currentAbility + '\n'
                + "반올림값: " + Math.Round(disincreaseAttackSpeed * itemEfficiency.currentAbility * 100) / 100
            );

            attackSpeed.DisIncreaseAbility((float)Math.Round(disincreaseAttackSpeed * itemEfficiency.currentAbility * 100) / 100);
        }

        /// <summary>
        /// 특성 강화에 필요한 코인 사용량을 계산하는 메소드
        /// </summary>
        /// <param name="trait">특성 종류</param>
        /// <returns>코인 사용량</returns>
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
        /// 코인 증가 메소드
        /// </summary>
        /// <param name="coin">획득 코인</param>
        public void IncreaseCoin(int coin)
        {
            currentCoin += coin;
            onCoinEvent.Invoke(currentCoin);
        }

        /// <summary>
        /// 코인 감소 메소드
        /// </summary>
        /// <param name="coin">감소 코인</param>
        /// <returns>감소 성공 여부</returns>
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