using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using WhalePark18.MemoryPool;
using WhalePark18.Objects;
using WhalePark18.Character.Enemy;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// 무기(메인): 돌격 소총 클래스
    /// </summary>
    public class WeaponAssultRifle : WeaponBase
    {
        [Header("Fire Effects")]
        [SerializeField]
        private GameObject muzzleFlashEffect;       // 총구 이펙트 (On/Off)

        [Header("Spawn Points")]
        [SerializeField]
        private Transform casingSpawnPoint;         // 탄피 생성 위치
        [SerializeField]
        private Transform bulletSpawnPoint;         // 총알 생성 위치

        [Header("Audio Cilps")]
        [SerializeField]
        private AudioClip audioClipTakeOutWeapon;   // 무기 장착 사운드
        [SerializeField]
        private AudioClip audioclipFire;            // 공격 사운드
        [SerializeField]
        private AudioClip audioClipReload;          // 재장전 사운드

        [Header("Aim UI")]
        [SerializeField]
        private Image imageAim;                     // default/aim 모드에 따라 Aim 이미지 활성/비활성

        private bool isModeChange = false;          // 모드 전환 여부 체크용
        private float defaultModeFOV = 60f;         // 기본모드에서의 카메라 FOV
        private float aimModeFOV = 30f;             // Aim모드에서의 카메라 FOV(시야각)

        private CasingMemoryPool casingMemoryPool;  // 탄피 생성 후, 활성/비활성 관리
        private ImpactMemoryPool impactMemoryPool;  // 공격 효과 생성 후 활성/비활성 관리
        private Camera mainCamera;                  // 광선 발사

        private void Awake()
        {
            base.Setup();

            casingMemoryPool = GetComponent<CasingMemoryPool>();
            impactMemoryPool = GetComponentInParent<ImpactMemoryPool>();
            mainCamera = Camera.main;

            /// 처음 탄창 수 최대로 설정
            weaponSetting.currentMagazine = weaponSetting.maxMagazine;

            /// 처음 탄 수 최대로 설정
            weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        }

        private void OnEnable()
        {
            /// 무기 장착 사운드 재생
            PlaySound(audioClipTakeOutWeapon);

            /// 총구 이펙트 오브젝트 비활성화
            muzzleFlashEffect.SetActive(false);

            /// 이벤트 클래스에 등록된 외부 메소드가 호출되는 시점은
            /// 이벤트 클래스의 Invoke() 메소드가 호출될 때이다.

            /// 무기가 활성화될 떄 해당 무기의 탄창 정보를 갱신
            onMagazineEvent.Invoke(weaponSetting.currentMagazine);

            /// 무기가 활성화될 때 해당 무기의 탄 수 정보를 갱신
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            ResetVariables();
        }

        public override void StartWeaponAction(int type = 0)
        {
            /// 재장전 중일 때는 무기 액션을 할 수 없다.
            if (isReload) return;

            /// 모드 전환중이면 무기 액션을 할 수 없다.
            if (isModeChange) return;

            /// 마우스 왼쪽 클릭(공격 시작)
            if (type == 0)
            {
                /// 연속 공격
                if (weaponSetting.isAutomaticAttack)
                {
                    isAttack = true;

                    /// OnAttack()을 매 프레임마다 재생
                    StartCoroutine("OnAttackLoop");
                }
                /// 단발 공격
                else
                {
                    /// 실제 공격 메소드
                    OnAttack();
                }
            }
            /// 마우스 오른쪽 클릭(모드 전환)
            else
            {
                /// 공격 중일 때는 모드 전환을 할 수 없다.
                if (isAttack) return;

                StartCoroutine("OnModeChange");
            }
        }

        public override void StopWeaponAction(int type = 0)
        {
            /// 마우스 왼쪽 클릭
            if (type == 0)
            {
                isAttack = false;
                StopCoroutine("OnAttackLoop");
            }
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
        /// 무기의 사격모드가 '자동'일 때 실행되는 공격 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        /// <remarks>
        /// 무한 반복문에서 OnAttack() 메소드 실행
        /// </remarks>
        private IEnumerator OnAttackLoop()
        {
            while (true)
            {
                OnAttack();

                yield return null;
            }
        }

        /// <summary>
        /// 공격 메소드
        /// </summary>
        /// <remarks>
        /// 실질적으로 공격을 실행하는 메소드
        /// </remarks>
        private void OnAttack()
        {
            /// 공격 주기 확인
            if (Time.time - lastAttackTime > weaponSetting.attackRate)
            {
                /// 달리기 중이면 공격할 수 없다.
                if (animator.MoveSpeed > 0.5f)
                {
                    return;
                }

                /// 공격 주기가 되어야 공격할 수 있도록 하기 위해 현재 시간 저장
                lastAttackTime = Time.time;

                /// 탄약 수가 없으면 공격 불가능
                if (weaponSetting.currentAmmo <= 0)
                {
                    return;
                }
                /// 공격시 currentAmmo 1 감소, 탄 수 UI 업데이트
                weaponSetting.currentAmmo--;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                /// 무기 애니메이션 재생
                /// 
                /// 알아가기
                /// animator.Play("Fire")
                ///  - 같은 애니메이션을 반복할 떄 중간에 끊지 못하고 재생 완료 후 다시 재생
                /// animator.Play("Fire", -1, 0)
                ///  - 같은 애니메이션을 반복할 때 애니메이션을 끊고 처음부터 다시 재생
                string animation = animator.AimModeIs ? "AimFire" : "Fire";
                animator.Play(animation, -1, 0);

                /// 총구 이펙트 재생 (default 모드 일 때만 재생)
                if (animator.AimModeIs == false) StartCoroutine("OnMuzzleFlashEffect");

                /// 공격 사운드 재생
                PlaySound(audioclipFire);

                /// 탄피 생성
                casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

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
                if (audioSource.isPlaying == false && (animator.CurrentAnimationIs("Movement") || animator.CurrentAnimationIs("aim_fire_pose@assault_rifle_01")))
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

        /// <summary>
        /// Aim 모드 변경 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator OnModeChange()
        {
            float current = 0;  // Lerp를 이용한 자연스러운 확대를 위한 변수
            float percent = 0;  // Lerp를 이용한 자연스러운 확대를 위한 변수
            float time = 0.35f; // Mode Change에 걸리는 시간

            animator.AimModeIs = !animator.AimModeIs;
            imageAim.enabled = !imageAim.enabled;

            float start = mainCamera.fieldOfView;
            float end = animator.AimModeIs ? aimModeFOV : defaultModeFOV;

            isModeChange = true;

            while (percent < 1)
            {
                current += Time.deltaTime;
                percent = current / time;

                /// 모드에 따라 카메라의 시야각을 변경
                mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

                yield return null;
            }

            isModeChange = false;
        }

        private void ResetVariables()
        {
            isReload = false;
            isAttack = false;
            isModeChange = false;
        }
    }
}