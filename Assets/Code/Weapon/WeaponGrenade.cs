using System.Collections;
using UnityEngine;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// 무기(수류탄): 수류탄 클래스
    /// </summary>
    public class WeaponGrenade : WeaponBase
    {
        [Header("Audio Clips")]
        [SerializeField]
        private AudioClip audioClipFire;        // 공격 사운드

        [Header("Grenade")]
        [SerializeField]
        private GameObject grenadePrefab;       // 수류탄 프리팹
        [SerializeField]
        private Transform grenadeSpawnPoint;    // 수류탄 생성 위치

        protected override void Awake()
        {
            /// 1. 컴포넌트 초기화
            base.Awake();

            /// 2. 탄창, 탄약 초기화
            weaponSetting.currentMagazine = weaponSetting.maxMagazine;
            weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        }

        private void OnEnable()
        {
            /// 무기 변경시, 공격 사운드가 재생되는 현상 제어하기 위해 오디오 출력을 정지함
            audioSource.Stop();

            /// 무기 활성화될 때 해당 무기 탄창과 탄 수  정보 갱신
            onMagazineEvent.Invoke(weaponSetting.currentMagazine);
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            ResetAttackVariables();
        }

        public override void StartWeaponAction(int type = 0)
        {
            if (type == 0 && isAttack == false && weaponSetting.currentAmmo > 0)
            {
                StartCoroutine("OnAttack");
            }
        }

        public override void StopWeaponAction(int type = 0)
        {
            /// 연속 동작이 없으므로 
        }

        public override void StartReload()
        {
            /// 수류탄은 탄수가 아닌 탄창으로 관리되기 때문에 재장전이 없다.
        }

        /// <summary>
        /// 공격 메소드
        /// </summary>
        /// /// <remarks>
        /// 실질적으로 공격을 실행하는 메소드
        /// </remarks>
        private IEnumerator OnAttack()
        {
            isAttack = true;

            animator.Play("Fire", -1, 0);
            PlaySound(audioClipFire);

            yield return new WaitForEndOfFrame();

            while (true)
            {
                if (animator.CurrentAnimationIs("Movement"))
                {
                    isAttack = false;

                    yield break;
                }

                yield return null;
            }
        }

        /// <summary>
        /// 수류탄 게임 오브젝트 생성 메소드(애니메이션 이벤트)
        /// </summary>
        public void SpawnGrenadeProjectile()
        {
            GameObject grenadeClone = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Random.rotation);
            grenadeClone.GetComponent<WeaponGrenadeProjectile>().Setup(weaponSetting.damage, transform.parent.forward);

            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            if(weaponSetting.currentAmmo <= 0)
            {
                Lock();
            }
        }

        public override void IncreaseMagazine(int ammo)
        {
            /// 수류탄은 탄창이 따로 없고, 탄수(Ammo)를 수류탄 개수로 사용하기 때문에 탄수르 증가시킨다.
            weaponSetting.currentAmmo = weaponSetting.currentAmmo + ammo > weaponSetting.maxAmmo ? weaponSetting.maxAmmo : weaponSetting.currentAmmo + ammo;

            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
        }

        private void ResetAttackVariables()
        {
            isReload = false;
            isAttack = false;
            isAimMode = false;
        }
    }
}