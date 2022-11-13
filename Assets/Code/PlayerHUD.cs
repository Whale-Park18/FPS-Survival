using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerHUD : MonoBehaviour
{
    private WeaponBase          weapon;                 // ���� ������ ��µǴ� ����

    [Header("Components")]
    [SerializeField]
    private Status              status;                 // �÷��̾��� ����(�̵��ӵ�, ü��)

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI     textWeaponName;         // ���� �̸�
    [SerializeField]
    private Image               imageWeaponIcon;        // ���� ������
    [SerializeField]
    private Sprite[]            spriteeWeaponIcions;    // ���� �����ܿ� ���Ǵ� sprite �迭
    [SerializeField]
    private Vector2[]           sizeWeaponIcions;       // ���� �������� UI ũ�� �迭

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI     textAmmo;               // ����/�ִ� ź �� ��� text

    [Header("Magazine")]
    [SerializeField]
    private GameObject          magagineUIPrefab;       // źâ UI ������
    [SerializeField]
    private Transform           magazineParent;         // źâ UI�� ��ġ�Ǵ� �г�
    [SerializeField]
    private int                 maxMagazineCount;       // ó�� �����ϴ� �ִ� źâ ��

    private List<GameObject>    magazineList;           // źâ UI ����Ʈ

    [Header("HP & BloodScreen UI")]
    [SerializeField]   
    private TextMeshProUGUI     textHP;                 // �÷��̾��� ü���� ����ϴ� text
    [SerializeField]
    private Image               imageBloodScreen;       // �÷��̾ ���� �޾��� �� ȭ�鿡 ǥ�õǴ� Image
    [SerializeField]
    private AnimationCurve      curveBloodScreen;

    private void Awake()
    {
        /// �޼ҵ尡 ��ϵǾ� �ִ� �̺�Ʈ Ŭ����(weapon.xx)��
        /// Invoke() �޼ҵ尡 ȣ��� ���� ��ϵ� �޼ҵ�(�Ű�����)�� ����ȴ�.
        status.onHPEvent.AddListener(UpdateHPHUD);
    }

    public void SetupAllWeapons(WeaponBase[] weapons)
    {
        SetupMagaine();

        /// ��� ������ ��� ������ �̺�Ʈ ���
        for(int i=0;i<weapons.Length;i++)
        {
            weapons[i].onAmmoEvent.AddListener(UpdateAmmoHUD);
            weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
        }
    }

    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;

        SetupWeapon();
    }

    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteeWeaponIcions[(int)weapon.WeaponName];
        imageWeaponIcon.rectTransform.sizeDelta = sizeWeaponIcions[(int)weapon.WeaponName];
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }

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

    private void UpdateMagazineHUD(int currentMagazine)
    {
        for (int i = 0; i < magazineList.Count; i++)
        {
            magazineList[i].SetActive(false);
        }
        for (int i= 0; i < currentMagazine; i++)
        {
            magazineList[i].SetActive(true);
        }
    }

    private void UpdateHPHUD(int previous, int current)
    {
        textHP.text = "HP " + current;

        /// ü���� �������� ���� ȭ�鿡 ������ �̹����� ������� �ʵ��� return
        if (previous <= current) return;

        if(previous - current > 0)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    private IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
