using System.Collections;
using UnityEngine;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// ����(����ź): ����ź Ŭ����
    /// </summary>
    public class WeaponGrenade : WeaponBase
    {
        [Header("Audio Clips")]
        [SerializeField]
        private AudioClip audioClipFire;        // ���� ����

        [Header("Grenade")]
        [SerializeField]
        private GameObject grenadePrefab;       // ����ź ������
        [SerializeField]
        private Transform grenadeSpawnPoint;    // ����ź ���� ��ġ

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
            /// ���� �����, ���� ���尡 ����Ǵ� ���� �����ϱ� ���� ����� ����� ������
            audioSource.Stop();

            /// ���� Ȱ��ȭ�� �� �ش� ���� źâ�� ź ��  ���� ����
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
            /// ���� ������ �����Ƿ� 
        }

        public override void StartReload()
        {
            /// ����ź�� ź���� �ƴ� źâ���� �����Ǳ� ������ �������� ����.
        }

        /// <summary>
        /// ���� �޼ҵ�
        /// </summary>
        /// /// <remarks>
        /// ���������� ������ �����ϴ� �޼ҵ�
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
        /// ����ź ���� ������Ʈ ���� �޼ҵ�(�ִϸ��̼� �̺�Ʈ)
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
            /// ����ź�� źâ�� ���� ����, ź��(Ammo)�� ����ź ������ ����ϱ� ������ ź���� ������Ų��.
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