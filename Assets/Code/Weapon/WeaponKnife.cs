using System.Collections;
using UnityEngine;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// 무기(칼): 칼 클래스
    /// </summary>
    public class WeaponKnife : WeaponBase
    {
        [SerializeField]
        WeaponKnifeCollider weaponKnifeCollider;    // 칼 콜라이더 제어

        private void OnEnable()
        {
            isAttack = false;

            /// 무기 활성화될 때 해당 무기 탄창 정보 갱신
            onMagazineEvent.Invoke(weaponSetting.currentMagazine);

            /// 무기가 활성화 될 때 해당 무기의 탄 수 정보 갱신
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
        }

        private void Awake()
        {
            base.Setup();

            /// 처음 탄창, 탄 수는 최대로 설정
            weaponSetting.currentMagazine = weaponSetting.maxMagazine;
            weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        }

        public override void StartWeaponAction(int type = 0)
        {
            if (isAttack) return;

            /// 연속 공격
            if (weaponSetting.isAutomaticAttack)
            {
                StartCoroutine("OnAttackLoop", type);
            }
            /// 단일 공격
            else
            {
                StartCoroutine("OnAttack", type);
            }
        }

        public override void StopWeaponAction(int type = 0)
        {
            isAttack = false;
            StopCoroutine("OnAttackLoop");
        }

        public override void StartReload()
        {
            /// 칼같은 경우 재장전이 필요없는 무기이기 때문에 코드를 작성하지 않는다.
        }

        /// <summary>
        /// 무기의 사격모드가 '자동'일 때 실행되는 공격 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        /// <remarks>
        /// 무한 반복문에서 OnAttack() 메소드 실행
        /// </remarks>
        private IEnumerator OnAttackLoop(int type)
        {
            while (true)
            {
                yield return StartCoroutine("OnAttack", type);
            }
        }

        /// <summary>
        /// 공격 메소드
        /// </summary>
        /// <remarks>
        /// 실질적으로 공격을 실행하는 메소드
        /// </remarks>
        private IEnumerator OnAttack(int type)
        {
            isAttack = true;

            /// 공격 모션 선택 (0, 1)
            animator.SetFloat("attackType", type);
            /// 공격 애니메이션 재생
            animator.Play("Fire", -1, 0);

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
        /// 콜라이더 제어 메소드(애니메이션 이벤트)
        /// </summary>
        /// <remarks>
        /// arms_assault_rifle_01.fbx의
        /// knife_attack_1@assault_rifle_01, knife_attack_1@assault_rifle_02
        /// 애니메이션 이벤트 함수
        /// </remarks>
        public void StartWeaponKnifeCollider()
        {
            weaponKnifeCollider.StartCollider(weaponSetting.damage);
        }
    }
}