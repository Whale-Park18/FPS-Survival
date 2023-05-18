using System.Collections;
using UnityEngine;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// ����(Į): Į Ŭ����
    /// </summary>
    public class WeaponKnife : WeaponBase
    {
        [SerializeField]
        WeaponKnifeCollider weaponKnifeCollider;    // Į �ݶ��̴� ����

        protected override void Awake()
        {
            /// 1. ������Ʈ �ʱ�ȭ
            base.Awake();

            /// 2. źâ, ź�� �ʱ�ȭ
            weaponSetting.currentMagazine = weaponSetting.maxMagazine;
            weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        }

        private void OnEnable()
        {
            isAttack = false;

            /// ���� Ȱ��ȭ�� �� �ش� ���� źâ�� ź ��  ���� ����
            onMagazineEvent.Invoke(weaponSetting.currentMagazine);
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
        }

        public override void StartWeaponAction(int type = 0)
        {
            if (isAttack) return;

            /// ���� ����
            if (weaponSetting.isAutomaticAttack)
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
            /// Į���� ��� �������� �ʿ���� �����̱� ������ �ڵ带 �ۼ����� �ʴ´�.
        }

        /// <summary>
        /// ������ ��ݸ�尡 '�ڵ�'�� �� ����Ǵ� ���� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        /// <remarks>
        /// ���� �ݺ������� OnAttack() �޼ҵ� ����
        /// </remarks>
        private IEnumerator OnAttackLoop(int type)
        {
            while (true)
            {
                yield return StartCoroutine("OnAttack", type);
            }
        }

        /// <summary>
        /// ���� �޼ҵ�
        /// </summary>
        /// <remarks>
        /// ���������� ������ �����ϴ� �޼ҵ�
        /// </remarks>
        private IEnumerator OnAttack(int type)
        {
            isAttack = true;

            /// ���� ��� ���� (0, 1)
            animator.SetFloat("attackType", type);
            /// ���� �ִϸ��̼� ���
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
        /// �ݶ��̴� ���� �޼ҵ�(�ִϸ��̼� �̺�Ʈ)
        /// </summary>
        /// <remarks>
        /// arms_assault_rifle_01.fbx��
        /// knife_attack_1@assault_rifle_01, knife_attack_1@assault_rifle_02
        /// �ִϸ��̼� �̺�Ʈ �Լ�
        /// </remarks>
        public void StartWeaponKnifeCollider()
        {
            weaponKnifeCollider.StartCollider(weaponSetting.damage);
        }
    }
}