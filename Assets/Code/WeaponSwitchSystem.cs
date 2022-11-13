using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSwitchSystem : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerContoller;
    [SerializeField]
    private PlayerHUD       playerHUD;

    [SerializeField]
    private WeaponBase[]    weapons;            // �������� ���� 4����

    private WeaponBase      currentWeapon;      // ���� ������� ����
    private WeaponBase      previousWeapon;     // ������ ����ߴ� ����

    private void Awake()
    {
        /// ���� ���� ����� ���� ���� �������� ��� ���� �̺�Ʈ ���
        playerHUD.SetupAllWeapons(weapons);

        /// ���� �������� ��� ���� ������ �ʰ� ����
        for(int i=0;i<weapons.Length;i++)
        {
            if (weapons[i].gameObject != null)
            {
                weapons[i].gameObject.SetActive(false);
            }
        }

        SwitchingWeapon(WeaponType.Main);
    }

    private void Update()
    {
        UpdateSwitch();
    }

    private void UpdateSwitch()
    {
        if (Input.anyKey == false) return;

        /// built-in method
        /// - int data
        /// - bool result = int.TryParse(string key, out data)
        ///  key ���ڿ��� ���ڷ� ����ȯ�ؼ� data�� ����,
        ///  �����ϸ� result: true, �����ϸ� result: false
        int inputIndex = 0;
        if(int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 5))
        {
            SwitchingWeapon((WeaponType)(inputIndex - 1));
        }

    }

    private void SwitchingWeapon(WeaponType weaponType)
    {
        /// ��ü ������ ���Ⱑ ������ ����
        if (weapons[(int)weaponType] == null) return;

        /// ���� ������� ���Ⱑ ������ ���� ���� ������ ����
        if(currentWeapon != null)
        {
            previousWeapon = currentWeapon;
        }

        /// ���� ��ü
        currentWeapon = weapons[(int)weaponType];

        /// ���� ������� ����� ��ü�Ϸ��� �� �� ����
        if (currentWeapon == previousWeapon) return;

        /// ���⸦ ����ϴ� PlayerController, PlayerHUD�� ���� ���� ���� ����
        playerContoller.SwitchingWeapon(currentWeapon);
        playerHUD.SwitchingWeapon(currentWeapon);

        /// ������ ����ϴ� ���� ��Ȱ��ȭ
        if(previousWeapon!= null)
        {
            previousWeapon.gameObject.SetActive(false);
        }
        /// ���� ����ϴ� ���� Ȱ��ȭ
        currentWeapon.gameObject.SetActive(true);
    }

    /// <summary>
    /// weaponType�� ���⿡�� źâ �� ����
    /// </summary>
    /// <param name="weaponType">���� Ÿ��</param>
    /// <param name="magazine">������</param>
    public void IncreaseMagazine(WeaponType weaponType, int magazine)
    {
        /// �ش� ���Ⱑ �ִ��� �˻�
        if (weapons[(int)weaponType] != null)
        {
            /// �ش� ������ źâ ���� magazine��ŭ ����
            weapons[(int)weaponType].IncreaseMagazine(magazine);
        }
    }

    /// <summary>
    /// �������� ��� ������ źâ �� ����
    /// </summary>
    /// <param name="magazine">������</param>
    public void IncreaseMagazine(int magazine)
    {
        for (int i = 0; i < weapons.Length; i++) 
        {
            if (weapons[i] != null)
            {
                weapons[i].IncreaseMagazine(magazine);
            }
        }
    }
}
