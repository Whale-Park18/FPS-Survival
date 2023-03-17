using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using WhalePark18.Character.Player;
using WhalePark18.Weapon;
using WhalePark18.HUD.Window;
using WhalePark18.Manager;

namespace WhalePark18.HUD
{
    public class PlayerHUD : MonoBehaviour
    {
        private WeaponBase weapon;              // 현재 정보가 출력되는 무기

        [Header("Components")]
        [SerializeField]
        private PlayerStatus status;                  // 플레이어의 상태(이동속도, 체력)

        [Header("Weapon Base")]
        [SerializeField]
        private TextMeshProUGUI textWeaponName; // 무기 이름
        [SerializeField]
        private Image imageWeaponIcon;          // 무기 아이콘
        [SerializeField]
        private Sprite[] spriteeWeaponIcions;   // 무기 아이콘에 사용되는 sprite 배열
        [SerializeField]
        private Vector2[] sizeWeaponIcions;     // 무기 아이콘의 UI 크기 배열

        [Header("Ammo")]
        [SerializeField]
        private TextMeshProUGUI textAmmo;       // 현재/최대 탄 수 출력 text

        [Header("Magazine")]
        [SerializeField]
        private GameObject magagineUIPrefab;    // 탄창 UI 프리팹
        [SerializeField]
        private Transform magazineParent;       // 탄창 UI가 배치되는 패널
        [SerializeField]
        private int maxMagazineCount;           // 처음 생성하는 최대 탄창 수

        private List<GameObject> magazineList;  // 탄창 UI 리스트

        [Header("Coin")]
        [SerializeField]
        private TextMeshProUGUI textCoin;       // 코인 텍스트

        [Header("HP & Shield & BloodScreen UI")]
        [SerializeField]
        private Slider sliderHP;                // 플레이어 체력을 출력하는 슬라이더
        [SerializeField]
        private TextMeshProUGUI textHP;         // 플레이어의 체력을 출력하는 text
        [SerializeField]
        private Slider sliderShield;            // 플레이어의 실드를 출력하는 슬라이더
        [SerializeField]
        private TextMeshProUGUI textShield;     // 플레이어의 실드를 출력하는 text
        [SerializeField]
        private Image imageBloodScreen;         // 플레이어가 공격 받았을 때 화면에 표시되는 Image
        [SerializeField]
        private AnimationCurve curveBloodScreen;

        [Header("Timer")]
        [SerializeField]
        private TextMeshProUGUI textTimer;

        //[Header("Score")]
        //[SerializeField]
        //private 

        private void Awake()
        {
            //playerTraitsButton = GetComponent<PlayerTraitsButton>();

            /// 메소드가 등록되어 있는 이벤트 클래스(weapon.xx)의
            /// Invoke() 메소드가 호출될 때 등록된 메소드(매개변수)가 실행된다.
            status.onCoinEvent.AddListener(UpdateCoinHUD);
            status.onHPEvent.AddListener(UpdateHpHUD);
            status.onShieldEvent.AddListener(UpdateShieldHUD);
        }

        private void Start()
        {
            UpdateCoinHUD(status.CurrentCoin);
            UpdateHpHUD((int)status.Hp.currentAbility, (int)status.Hp.currentAbility);
            UpdateShieldHUD((int)status.Shield.currentAbility);

            StartCoroutine(UpdateTimeHUD());
        }

        /// <summary>
        /// 코인 HUD 업데이트 메소드
        /// </summary>
        /// <param name="current"></param>
        private void UpdateCoinHUD(int current)
        {
            textCoin.text = current.ToString();
        }

        /// <summary>
        /// HP HUD 업데이트 메소드
        /// </summary>
        /// <param name="previous">이전 체력</param>
        /// <param name="current">현재 체력</param>
        private void UpdateHpHUD(int previous, int current)
        {
            textHP.text = current.ToString();
            sliderHP.value = (float)current / status.Hp.maxAbility;

            /// 체력이 증가했을 때는 화면에 빨간색 이미지를 출력하지 않도록 return
            if (previous <= current) return;

            if (previous - current > 0)
            {
                StopCoroutine("OnBloodScreen");
                StartCoroutine("OnBloodScreen");
            }
        }

        /// <summary>
        /// 보호막 HUD 업데이트 메소드
        /// </summary>
        /// <param name="current">현재 보호막</param>
        private void UpdateShieldHUD(int current)
        {
            textShield.text = current.ToString();

            if (status.ShieldActive == false) 
                sliderShield.value = 0;
            else 
                sliderShield.value = (float)current / status.Shield.maxAbility;
        }

        /// <summary>
        /// 피해 가시적 이펙트 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator OnBloodScreen()
        {
            float percent = 0;

            while (percent < 1)
            {
                percent += Time.deltaTime;

                Color color = imageBloodScreen.color;
                color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));
                imageBloodScreen.color = color;

                yield return null;
            }
        }

        /// <summary>
        /// 무기 변경 메소드
        /// </summary>
        /// <param name="newWeapon">변경할 새로운 무기</param>
        public void SwitchingWeapon(WeaponBase newWeapon)
        {
            weapon = newWeapon;

            SetupWeapon();
        }

        /// <summary>
        /// 무기 설정 메소드
        /// </summary>
        private void SetupWeapon()
        {
            textWeaponName.text = weapon.WeaponName.ToString();
            imageWeaponIcon.sprite = spriteeWeaponIcions[(int)weapon.WeaponName];
            imageWeaponIcon.rectTransform.sizeDelta = sizeWeaponIcions[(int)weapon.WeaponName];
        }

        /// <summary>
        /// 모든 무기를 설정하는 메소드
        /// </summary>
        /// <param name="weapons">관리할 무기 배열</param>
        public void SetupAllWeapons(WeaponBase[] weapons)
        {
            SetupMagaine();

            /// 사용 가능한 모든 무기의 이벤트 등록
            for (int i = 0; i < weapons.Length; i++)
            {
                weapons[i].onAmmoEvent.AddListener(UpdateAmmoHUD);
                weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
            }
        }

        /// <summary>
        /// 탄창 설정 메소드
        /// </summary>
        private void SetupMagaine()
        {
            /// weapon에 등록되어 있는 최대 탄창 개수만큼 Image Icon을 생성
            /// magazineParent 오브젝트의 자식으로 등록 후 모두 비활성화/리스트에 저장
            magazineList = new List<GameObject>();
            for (int i = 0; i < maxMagazineCount; i++)
            {
                GameObject clone = Instantiate(magagineUIPrefab);
                clone.transform.SetParent(magazineParent);
                clone.SetActive(false);

                magazineList.Add(clone);
            }
        }

        /// <summary>
        /// 총알 HUD 업데이트 메소드
        /// </summary>
        /// <param name="currentAmmo">현재 총알 개수</param>
        /// <param name="maxAmmo">최대 총알 개수</param>
        private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
        {
            textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
        }

        /// <summary>
        /// 탄창 HUD 업데이트메소드
        /// </summary>
        /// <param name="currentMagazine">현재 탄창</param>
        private void UpdateMagazineHUD(int currentMagazine)
        {
            UnityEngine.Debug.LogFormat("<color=red>UpdateMagazineHUD</color>");
            for (int i = 0; i < magazineList.Count; i++)
            {
                magazineList[i].SetActive(false);
            }
            for (int i = 0; i < currentMagazine; i++)
            {
                magazineList[i].SetActive(true);
            }
        }

        private IEnumerator UpdateTimeHUD()
        {
            while(true)
            {
                textTimer.text = GameManager.Instance.TimeToString();

                yield return null;
            }
        }
    }
}