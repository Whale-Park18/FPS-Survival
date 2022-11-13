using System.Collections;
using UnityEngine;

public class WeaponKnife : WeaponBase
{
    [SerializeField]
    WeaponKnifeCollider weaponKnifeCollider;

    private void OnEnable()
    {
        isAttack = false;

        /// ���� Ȱ��ȭ�� �� �ش� ���� źâ ���� ����
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);

        /// ���Ⱑ Ȱ��ȭ �� �� �ش� ������ ź �� ���� ����
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    private void Awake()
    {
        base.Setup();

        /// ó�� źâ, ź ���� �ִ�� ����
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    public override void StartWeaponAction(int type = 0)
    {
        if (isAttack) return;

        /// ���� ����
        if(weaponSetting.isAutomaticAttack)
        {
            StartCoroutine("OnAttackLoop", type);
        }
        /// ���� ����
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

        /// ���� ��� ���� (0, 1)
        animator.SetFloat("attackType", type);
        /// ���� �ִϸ��̼� ���
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
    /// arms_assault_rifle_01.fbx��
    /// knife_attack_1@assault_rifle_01, knife_attack_1@assault_rifle_02
    /// �ִϸ��̼� �̺�Ʈ �Լ�
    /// </summary>
    public void StartWeaponKnifeCollider()
    {
        weaponKnifeCollider.StartCollider(weaponSetting.damage);
    }
}
