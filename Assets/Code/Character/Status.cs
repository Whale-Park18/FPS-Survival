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
        private int startHP = 100;                      // 게임 시작 HP
        private int currentHP;                          // 현재 HP
        private int maxHP;                              // 최대 HP

        [SerializeField]
        private float delayHPResilience = 10;           // HP 회복 지연 시간
        [SerializeField]
        private int startHPResilienceForSec = 0;        // 게임 시작 초당 HP 회복량
        private int currentHPResilienceForSec;          // 현재 초당 HP 회복량

        [Header("Shield")]
        [SerializeField]
        private int startShield = 0;                    // 게임 시작 실드량
        private int currentShield;                      // 현재 실드량
        private int maxShield;                          // 최대 실드량

        [SerializeField]
        private float delayShieldResilience;            // 실드 회복 지연 시간
        [SerializeField]
        private float startShieldResilienceForSec = 0f; // 게임 시작 초당 실드 회복 퍼센트
        private float currentShieldResilienceForSec;    // 현재 초당 실드 회복 퍼센트

        [Header("Attack")]
        [SerializeField]
        private float startDamagePercentIncrease = 1f;  // 게임 시작 피해 % 증가량
        private float currentDamagePercentIncrease;     // 현재 피해 % 증가량

        [SerializeField]
        private int startPiercing = 0;                  // 게임 시작 관통력
        private int currentPiercing;                    // 현재 관통력

        [SerializeField]
        private float startAttackSpeed = 1f;            // 게임 시작 공격 속도
        private float currentAttackSpeed;               // 현재 공격 속도

        [Header("Item")]
        private float startItemEfficiency = 1f;         // 게임 시작 아이템 효율
        private float currentItemEfficiency;            // 현재 아이템 효율

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
        /// 실드 Property
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
            /// 코인
            currentCoin = startCoin;

            /// 생명력
            maxHP = currentHP = startHP;
            currentHPResilienceForSec = startHPResilienceForSec;

            /// 실드
            maxShield = currentShield = startShield;
            currentShieldResilienceForSec = startShieldResilienceForSec;

            /// 공격
            currentDamagePercentIncrease = startDamagePercentIncrease;
            currentPiercing = startPiercing;
            currentAttackSpeed = startAttackSpeed;

            /// 아이템
            currentItemEfficiency = startItemEfficiency;
        }

        /// <summary>
        /// 코인 증가 인터페이스
        /// </summary>
        /// <param name="coin">획득 코인</param>
        public void IncreaseCoin(int coin)
        {
            currentCoin += coin;
            onCoinEvent.Invoke(currentCoin);
        }

        /// <summary>
        /// 코인 감소 인터페이스
        /// </summary>
        /// <param name="coin">감소 코인</param>
        /// <returns>감소 성공 여부</returns>
        public bool DecreaseCoin(int coin)
        {
            if (currentCoin - coin < 0)
                return false;

            currentCoin -= coin;
            onCoinEvent.Invoke(coin);

            return true;
        }

        /// <summary>
        /// 체력 증가 인터페이스
        /// </summary>
        /// <param name="hp">회복량</param>
        public void IncreaseHP(int hp)
        {
            int previousHP = currentHP;

            currentHP = currentHP + hp > maxHP ? maxHP : currentHP + hp;

            onHPEvent.Invoke(previousHP, currentHP);
        }

        /// <summary>
        /// 체력 감소 인터페이스
        /// </summary>
        /// <param name="damage">피해량</param>
        /// <returns>사망 여부</returns>
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
        /// HP 회복력 활성화 인터페이스
        /// </summary>
        public void StartHPResilience()
        {
            print("StartHPResilience");
            StartCoroutine("OnHPResilience");
        }

        /// <summary>
        /// HP 회복력 비활성화 인터페이스
        /// </summary>
        public void StopHPResilience()
        {
            print("StopHPResilience");
            StopCoroutine("OnHPResilience");
        }

        /// <summary>
        /// HP 회복력 메소드
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
        /// 실드 증가 인터페이스
        /// </summary>
        /// <param name="shield">회복량</param>
        public void IncreaseShield(int shield)
        {
            int previousShield = currentShield;

            currentShield = currentShield + shield > maxShield ? maxShield : currentShield + shield;

            // TODO: onShieldEvent 객체 생성
            onHPEvent.Invoke(previousShield, currentHP);
        }

        /// <summary>
        /// 실드 감소 인터페이스
        /// </summary>
        /// <param name="damage">피해량</param>
        /// <returns>사망 여부</returns>
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
        /// 공격 속도 증가 인터페이스
        /// </summary>
        /// <param name="increaseAttackSpeed">공격 속도 증가량</param>
        public void IncreaseAttackSpeed(float increaseAttackSpeed)
        {
            Debug.LogFormat("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color>\n" +
                "공속증 * 아이템 효율 %: " + (increaseAttackSpeed * currentItemEfficiency) * 100 + '\n' +
                "반올림 값: " + Mathf.Round((increaseAttackSpeed * currentItemEfficiency) * 100) + '\n' +
                "실수형: " + Mathf.Round((increaseAttackSpeed * currentItemEfficiency) * 100) / 100);
            // increaseAttackSpeed * 100
            // Mathf.Round(increaseAttackSpeed * 100)
            // Mathf.Round(increaseAttackSpeed * 100) / 100

            /// Mathf.Round(): 소숫점 첫째자리에서 반올림
            /// 하지만 아이템 효과와 아이템 효율 스탯에 의해 소숫점 셋째자리를 반올림 해야 함
            /// 따라서 [아이템 효과 * 아이템 효율 스탯] 값에 100을 곱해 소수섬 셋째자리를 
            /// 소수점 첫째자리로 바꾸고 반올림을 한 후, 100을 나눠 복원함
            currentAttackSpeed += Mathf.Round((increaseAttackSpeed * currentItemEfficiency) * 100) / 100;
        }

        /// <summary>
        /// 공격 속도 감소 인터페이스
        /// </summary>
        /// <param name="disincreaseAttackSpeed">공격 속도 감소량</param>
        public void DisincreaseAttackSpeed(float disincreaseAttackSpeed)
        {
            Debug.LogFormat("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color>\n" +
                "공속증 * 아이템 효율 %: " + (disincreaseAttackSpeed * currentItemEfficiency) * 100 + '\n' +
                "반올림 값: " + Mathf.Round((disincreaseAttackSpeed * currentItemEfficiency) * 100) + '\n' +
                "실수형: " + Mathf.Round((disincreaseAttackSpeed * currentItemEfficiency) * 100) / 100);

            currentAttackSpeed -= Mathf.Round((disincreaseAttackSpeed * currentItemEfficiency) * 100) / 100;
        }
    }
}