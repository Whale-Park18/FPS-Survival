using System.Collections;
using UnityEngine;

public class WeaponKnife : WeaponBase
{
    [SerializeField]
    WeaponKnifeCollider weaponKnifeCollider;

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
        if(weaponSetting.isAutomaticAttack)
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
        
    }

    private IEnumerator OnAttackLoop(int type)
    {
        while(true)
        {
            yield return StartCoroutine("OnAttack", type);
        }
    }

    private IEnumerator OnAttack(int type)
    {
        isAttack = true;

        /// 공격 모션 선택 (0, 1)
        animator.SetFloat("attackType", type);
        /// 공격 애니메이션 재생
        animator.Play("Fire", -1, 0);

        yield return new WaitForEndOfFrame();

        while(true)
        {
            if(animator.CurrentAnimationIs("Movement"))
            {
                isAttack = false;

                yield break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// arms_assault_rifle_01.fbx의
    /// knife_attack_1@assault_rifle_01, knife_attack_1@assault_rifle_02
    /// 애니메이션 이벤트 함수
    /// </summary>
    public void StartWeaponKnifeCollider()
    {
        weaponKnifeCollider.StartCollider(weaponSetting.damage);
    }
}
