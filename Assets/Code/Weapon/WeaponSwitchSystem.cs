using System.Collections;
using UnityEngine;

using WhalePark18.Character.Player;
using WhalePark18.UI;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// 무기 교체 시스템 클래스
    /// </summary>
    public class WeaponSwitchSystem : MonoBehaviour
    {
        [SerializeField]
        private PlayerController    playerContoller;
        [SerializeField]
        private PlayerHUD           playerHUD;

        [SerializeField]
        private WeaponBase[]        weapons;        // 소지중인 무기 4종류

        private WeaponBase          currentWeapon;  // 현재 사용중인 무기
        private WeaponBase          previousWeapon; // 직전에 사용했던 무기

        /****************************************
         * 프로퍼티
         ****************************************/
        public WeaponBase[] Weapons => weapons;

        private void Awake()
        {
            /// 무기 정보 출력을 위해 현재 소지중인 모든 무기 이벤트 등록
            playerHUD.SetupAllWeapons(weapons);

            /// 현재 소지중인 모든 무기 보이지 않게 설정
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
            /// 모든 키가 눌리지 않았으면 종료
            if (Input.anyKey == false) return;

            /// built-in method
            /// - int data
            /// - bool result = int.TryParse(string key, out data)
            ///  key 문자열을 숫자로 형변환해서 data에 저장,
            ///  성공하면 result: true, 실패하면 result: false
            int inputIndex = 0;
            if (int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 5))
            {
                SwitchingWeapon((WeaponType)(inputIndex - 1));
            }
        }

        /// <summary>
        /// 강제로 무기를 교체하는 메소드
        /// </summary>
        private void ForcedSwitchngWeapon()
        {
            if(currentWeapon.WeaponLock)
            {
                SwitchingWeapon(WeaponType.Sub);
            }
        }

        /// <summary>
        /// 무기 교체 메서드
        /// </summary>
        /// <param name="weaponType">변경할 무기 타입</param>
        private void SwitchingWeapon(WeaponType weaponType)
        {
            /// 교체 가능한 무기가 없거나 잠금 상태이면 종료
            if (weapons[(int)weaponType] == null || weapons[(int)weaponType].WeaponLock) return;

            /// 현재 사용중인 무기가 있으면 이전 무기 정보에 저장
            if (currentWeapon != null)
            {
                previousWeapon = currentWeapon;
            }

            /// 무기 교체
            currentWeapon = weapons[(int)weaponType];

            /// 현재 사용중인 무기로 교체하려고 할 때 종료
            if (currentWeapon == previousWeapon) return;

            /// 무기를 사용하는 PlayerController, PlayerHUD에 현재 무기 정보 전달
            playerContoller.SwitchingWeapon(currentWeapon);
            playerHUD.SwitchingWeapon(currentWeapon);

            /// 이전에 사용하던 무기 비활성화
            if (previousWeapon != null)
            {
                previousWeapon.gameObject.SetActive(false);
            }
            /// 현재 사용하는 무기 활성화
            currentWeapon.gameObject.SetActive(true);
        }

        /// <summary>
        /// weaponType의 무기에만 탄창 수 증가
        /// </summary>
        /// <param name="weaponType">무기 타입</param>
        /// <param name="magazine">증가량</param>
        public void IncreaseMagazine(WeaponType weaponType, int magazine)
        {
            /// 해당 무기가 있는지 검사
            if (weapons[(int)weaponType] != null)
            {
                /// 해당 무기의 탄창 수를 magazine만큼 증가
                weapons[(int)weaponType].IncreaseMagazine(magazine);
            }
        }

        /// <summary>
        /// 소지중인 모든 무기의 탄창 수 증가
        /// </summary>
        /// <param name="magazine">증가량</param>
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