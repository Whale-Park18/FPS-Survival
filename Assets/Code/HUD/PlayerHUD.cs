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
        private WeaponBase weapon;              // ���� ������ ��µǴ� ����

        [Header("Components")]
        [SerializeField]
        private PlayerStatus status;                  // �÷��̾��� ����(�̵��ӵ�, ü��)

        [Header("Weapon Base")]
        [SerializeField]
        private TextMeshProUGUI textWeaponName; // ���� �̸�
        [SerializeField]
        private Image imageWeaponIcon;          // ���� ������
        [SerializeField]
        private Sprite[] spriteeWeaponIcions;   // ���� �����ܿ� ���Ǵ� sprite �迭
        [SerializeField]
        private Vector2[] sizeWeaponIcions;     // ���� �������� UI ũ�� �迭

        [Header("Ammo")]
        [SerializeField]
        private TextMeshProUGUI textAmmo;       // ����/�ִ� ź �� ��� text

        [Header("Magazine")]
        [SerializeField]
        private GameObject magagineUIPrefab;    // źâ UI ������
        [SerializeField]
        private Transform magazineParent;       // źâ UI�� ��ġ�Ǵ� �г�
        [SerializeField]
        private int maxMagazineCount;           // ó�� �����ϴ� �ִ� źâ ��

        private List<GameObject> magazineList;  // źâ UI ����Ʈ

        [Header("Coin")]
        [SerializeField]
        private TextMeshProUGUI textCoin;       // ���� �ؽ�Ʈ

        [Header("HP & Shield & BloodScreen UI")]
        [SerializeField]
        private Slider sliderHP;                // �÷��̾� ü���� ����ϴ� �����̴�
        [SerializeField]
        private TextMeshProUGUI textHP;         // �÷��̾��� ü���� ����ϴ� text
        [SerializeField]
        private Slider sliderShield;            // �÷��̾��� �ǵ带 ����ϴ� �����̴�
        [SerializeField]
        private TextMeshProUGUI textShield;     // �÷��̾��� �ǵ带 ����ϴ� text
        [SerializeField]
        private Image imageBloodScreen;         // �÷��̾ ���� �޾��� �� ȭ�鿡 ǥ�õǴ� Image
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

            /// �޼ҵ尡 ��ϵǾ� �ִ� �̺�Ʈ Ŭ����(weapon.xx)��
            /// Invoke() �޼ҵ尡 ȣ��� �� ��ϵ� �޼ҵ�(�Ű�����)�� ����ȴ�.
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
        /// ���� HUD ������Ʈ �޼ҵ�
        /// </summary>
        /// <param name="current"></param>
        private void UpdateCoinHUD(int current)
        {
            textCoin.text = current.ToString();
        }

        /// <summary>
        /// HP HUD ������Ʈ �޼ҵ�
        /// </summary>
        /// <param name="previous">���� ü��</param>
        /// <param name="current">���� ü��</param>
        private void UpdateHpHUD(int previous, int current)
        {
            textHP.text = current.ToString();
            sliderHP.value = (float)current / status.Hp.maxAbility;

            /// ü���� �������� ���� ȭ�鿡 ������ �̹����� ������� �ʵ��� return
            if (previous <= current) return;

            if (previous - current > 0)
            {
                StopCoroutine("OnBloodScreen");
                StartCoroutine("OnBloodScreen");
            }
        }

        /// <summary>
        /// ��ȣ�� HUD ������Ʈ �޼ҵ�
        /// </summary>
        /// <param name="current">���� ��ȣ��</param>
        private void UpdateShieldHUD(int current)
        {
            textShield.text = current.ToString();

            if (status.ShieldActive == false) 
                sliderShield.value = 0;
            else 
                sliderShield.value = (float)current / status.Shield.maxAbility;
        }

        /// <summary>
        /// ���� ������ ����Ʈ �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
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
        /// ���� ���� �޼ҵ�
        /// </summary>
        /// <param name="newWeapon">������ ���ο� ����</param>
        public void SwitchingWeapon(WeaponBase newWeapon)
        {
            weapon = newWeapon;

            SetupWeapon();
        }

        /// <summary>
        /// ���� ���� �޼ҵ�
        /// </summary>
        private void SetupWeapon()
        {
            textWeaponName.text = weapon.WeaponName.ToString();
            imageWeaponIcon.sprite = spriteeWeaponIcions[(int)weapon.WeaponName];
            imageWeaponIcon.rectTransform.sizeDelta = sizeWeaponIcions[(int)weapon.WeaponName];
        }

        /// <summary>
        /// ��� ���⸦ �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="weapons">������ ���� �迭</param>
        public void SetupAllWeapons(WeaponBase[] weapons)
        {
            SetupMagaine();

            /// ��� ������ ��� ������ �̺�Ʈ ���
            for (int i = 0; i < weapons.Length; i++)
            {
                weapons[i].onAmmoEvent.AddListener(UpdateAmmoHUD);
                weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
            }
        }

        /// <summary>
        /// źâ ���� �޼ҵ�
        /// </summary>
        private void SetupMagaine()
        {
            /// weapon�� ��ϵǾ� �ִ� �ִ� źâ ������ŭ Image Icon�� ����
            /// magazineParent ������Ʈ�� �ڽ����� ��� �� ��� ��Ȱ��ȭ/����Ʈ�� ����
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
        /// �Ѿ� HUD ������Ʈ �޼ҵ�
        /// </summary>
        /// <param name="currentAmmo">���� �Ѿ� ����</param>
        /// <param name="maxAmmo">�ִ� �Ѿ� ����</param>
        private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
        {
            textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
        }

        /// <summary>
        /// źâ HUD ������Ʈ�޼ҵ�
        /// </summary>
        /// <param name="currentMagazine">���� źâ</param>
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