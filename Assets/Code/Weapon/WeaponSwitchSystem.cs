using System.Collections;
using UnityEngine;

using WhalePark18.Character.Player;
using WhalePark18.UI;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// ���� ��ü �ý��� Ŭ����
    /// </summary>
    public class WeaponSwitchSystem : MonoBehaviour
    {
        [SerializeField]
        private PlayerController    playerContoller;
        [SerializeField]
        private PlayerHUD           playerHUD;

        [SerializeField]
        private WeaponBase[]        weapons;        // �������� ���� 4����

        private WeaponBase          currentWeapon;  // ���� ������� ����
        private WeaponBase          previousWeapon; // ������ ����ߴ� ����

        /****************************************
         * ������Ƽ
         ****************************************/
        public WeaponBase[] Weapons => weapons;

        private void Awake()
        {
            /// ���� ���� ����� ���� ���� �������� ��� ���� �̺�Ʈ ���
            playerHUD.SetupAllWeapons(weapons);

            /// ���� �������� ��� ���� ������ �ʰ� ����
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i].gameObject != null)
                {
                    weapons[i].gameObject.SetActive(false);
                }
            }

            SwitchingWeapon(WeaponType.Sub);
        }

        private void Update()
        {
            UpdateSwitch();
        }

        private void LateUpdate()
        {
            ForcedSwitchngWeapon();
        }

        private void UpdateSwitch()
        {
            /// ��� Ű�� ������ �ʾ����� ����
            if (Input.anyKey == false) return;

            /// built-in method
            /// - int data
            /// - bool result = int.TryParse(string key, out data)
            ///  key ���ڿ��� ���ڷ� ����ȯ�ؼ� data�� ����,
            ///  �����ϸ� result: true, �����ϸ� result: false
            int inputIndex = 0;
            if (int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 5))
            {
                SwitchingWeapon((WeaponType)(inputIndex - 1));
            }
        }

        /// <summary>
        /// ������ ���⸦ ��ü�ϴ� �޼ҵ�
        /// </summary>
        private void ForcedSwitchngWeapon()
        {
            if(currentWeapon.WeaponLock)
            {
                SwitchingWeapon(WeaponType.Sub);
            }
        }

        /// <summary>
        /// ���� ��ü �޼���
        /// </summary>
        /// <param name="weaponType">������ ���� Ÿ��</param>
        private void SwitchingWeapon(WeaponType weaponType)
        {
            /// ��ü ������ ���Ⱑ ���ų� ��� �����̸� ����
            if (weapons[(int)weaponType] == null || weapons[(int)weaponType].WeaponLock) return;

            /// ���� ������� ���Ⱑ ������ ���� ���� ������ ����
            if (currentWeapon != null)
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
            if (previousWeapon != null)
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
}