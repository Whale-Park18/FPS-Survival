using System.Collections;
using UnityEngine;

using WhalePark18.MemoryPool;
using WhalePark18.Objects;
using WhalePark18.Character.Enemy;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// 무기(서브): 리볼버 클래스
    /// </summary>
    public class WeaponRevolver : WeaponBase
    {
        [Header("Fire Effect")]
        [SerializeField]
        private GameObject muzzleFlashEffect;       // 총구 이펙트

        [Header("Spawn Points")]
        [SerializeField]
        private Transform bulletSpawnPoint;         // 총알 생성 위치

        [Header("Audio Clip")]
        [SerializeField]
        private AudioClip audioclipFire;            // 공격 사운드
        [SerializeField]
        private AudioClip audioClipReload;          // 장전 사운드

        private ImpactMemoryPool impactMemoryPool;  // 공격 효과 생성 후 활성/비활성 관리
        private Camera mainCamera;                  // 광선 발사

        private void OnEnable()
        {
            /// 총구 이펙트 오브젝트 비활성화
            muzzleFlashEffect.SetActive(false);

            /// 무기 활성화될 때 해당 무기 탄창 정보 갱신
            onMagazineEvent.Invoke(weaponSetting.currentMagazine);

            /// 무기가 활성화 될 때 해당 무기의 탄 수 정보 갱신
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            ResetVariables();
        }

        private void Awake()
        {
            base.Setup();

            impactMemoryPool = GetComponent<ImpactMemoryPool>();
            mainCamera = Camera.main;

            /// 처음 탄창, 탄 수는 최대로 설정
            weaponSetting.currentMagazine = weaponSetting.maxMagazine;
            weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        }

        public override void StartWeaponAction(int type = 0)
        {
            if (type == 0 && isAttack == false && isReload == false)
            {
                OnAttack();
            }
        }

        public override void StopWeaponAction(int type = 0)
        {
            isAttack = false;
        }

        public override void StartReload()
        {
            /// 현재 재장전 중이거나 탄창 수가 0이면 재장전 불가능
            if (isReload || weaponSetting.currentMagazine <= 0) return;

            /// 무기 액션 도중에 'R'키를 눌러 재장전을 시도하면 무기 액션 종료 후 재장전
            StopWeaponAction();
            StartCoroutine("OnReload");
        }

        /// <summary>
        /// 공격 메소드
        /// </summary>
        /// /// <remarks>
        /// 실질적으로 공격을 실행하는 메소드
        /// </remarks>
        public void OnAttack()
        {
            if (Time.time - lastAttackTime > weaponSetting.attackRate)
            {
                if (animator.MoveSpeed > 0.5f) return;

                lastAttackTime = Time.time;

                if (weaponSetting.currentAmmo <= 0) return;

                weaponSetting.currentAmmo--;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                /// 무기 애니메이션 재생
                animator.Play("Fire", -1, 0);
                /// 총구 이펙트 재생
                StartCoroutine("OnMuzzleFlashEffect");
                /// 공격 사운드 재생
                PlaySound(audioclipFire);

                /// 광선을 발사해 원하는 위치 공격(+Impact Effect)
                TwoStepRaycast();
            }
        }

        /// <summary>
        /// 총알 발사시 생기는 광원효과 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator OnMuzzleFlashEffect()
        {
            muzzleFlashEffect.SetActive(true);

            /// 무기의 공격속도보다 빠르게 muzzleFlashEffect를
            /// 잠깐 활성화한 후 비활성화 한다.
            yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

            muzzleFlashEffect.SetActive(false);
        }

        /// <summary>
        /// 재장전 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        /// <remarks>
        /// 실질적으로 재장전을 실행하는 메소드
        /// </remarks>
        private IEnumerator OnReload()
        {
            isReload = true;

            /// 재장전 애니메이션, 사운드 재생
            animator.OnReload();
            PlaySound(audioClipReload);

            while (true)
            {
                /// 사운드가 재생중이 아니고, 현재 애니메이션이 Movement이면
                /// 재장전 애니메이션(, 사운드) 재생이 종료되었다는 뜻
                if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
                {
                    isReload = false;

                    /// 현채 탄창 수를 1 감소시키고, 바뀐 탄창 정보를 Text UI에 업데이트
                    weaponSetting.currentMagazine--;
                    onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                    /// 현재 탄수를 최대로 설정하고, 바뀐 탄 수 정보를 Text UI에 업데이트
                    weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                    onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                    yield break;
                }

                yield return null;
            }
        }

        /// <summary>
        /// 2단계에 걸쳐 광선을 발사하는 메소드
        /// </summary>
        /// <remarks>
        /// 플레이어가 기준으로 삼은 타겟의 위치는 화면 정 중앙의 Aim이지만
        /// 실제로 총알이 발사되는 위치는 Aim과 다른 위치에 있는 총구 부분이기 때문에
        /// 타겟 위치 전후로는 위치 갭이 발생한다.
        /// </remarks>
        private void TwoStepRaycast()
        {
            Ray ray;
            RaycastHit hit;
            Vector3 targetPoint = Vector3.zero;

            /// 화면 중앙 좌표 (Aim 기준으로 Raycast 연산)
            ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
            if (Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
            {
                targetPoint = hit.point;
            }
            /// 공격 사거리 안에 부딪히는 오브젝트가 없으면 targetPoint는 최대 사거리 위치
            else
            {
                targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
            }
            Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

            /// 첫 번째 Raycast연산으로 얻어진 targetPoint를 목표지점으로 설정하고
            /// 총구를 시적점으로 하여 Raycast 연산
            Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
            if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
            {
                impactMemoryPool.SpawnImpact(hit);

                if (hit.transform.CompareTag("ImpactEnemy"))
                {
                    hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
                }
                else if (hit.transform.CompareTag("InteractionObject"))
                {
                    hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
                }
            }
            Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
        }

        private void ResetVariables()
        {
            isReload = false;
            isAttack = false;
        }
    }
}