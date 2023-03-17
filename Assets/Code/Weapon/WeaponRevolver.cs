using System.Collections;
using UnityEngine;

using WhalePark18;
using WhalePark18.Objects;
using WhalePark18.Character.Enemy;
using WhalePark18.Character.Player;
using System.Reflection;
using System;
using WhalePark18.Manager;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// ����(����): ������ Ŭ����
    /// </summary>
    public class WeaponRevolver : WeaponBase
    {
        [Header("Fire Effect")]
        [SerializeField]
        private GameObject muzzleFlashEffect;       // �ѱ� ����Ʈ (On/Off)

        [Header("Spawn Points")]
        [SerializeField]
        private Transform bulletSpawnPoint;         // �Ѿ� ���� ��ġ

        [Header("Audio Clip")]
        [SerializeField]
        private AudioClip audioclipFire;            // ���� ����
        [SerializeField]
        private AudioClip audioClipReload;          // ������ ����

        private PlayerStatus status;                // �÷��̾� �������ͽ�
        private Camera mainCamera;                  // ���� �߻�

        private void Awake()
        {
            base.Setup();

            status = GetComponentInParent<PlayerStatus>();
            //impactMemoryPool = GetComponent<ImpactMemoryPool>();
            mainCamera = Camera.main;

            /// ���� �������� ���� źâ ���������� ����
            //weaponSetting.currentMagazine = weaponSetting.maxMagazine;

            /// �Ѿ� �� �ִ�� ����
            weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        }

        private void OnEnable()
        {
            /// �ѱ� ����Ʈ ������Ʈ ��Ȱ��ȭ
            muzzleFlashEffect.SetActive(false);

            /// ���� Ȱ��ȭ�� �� �ش� ���� źâ ���� ����
            onMagazineEvent.Invoke(weaponSetting.currentMagazine);

            /// ���Ⱑ Ȱ��ȭ �� �� �ش� ������ ź �� ���� ����
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            ResetVariables();
        }

        public override void StartWeaponAction(int type = 0)
        {
            if (type == 0 && isAttack == false && isReload == false)
            {
                isAttack = true;

                if(weaponSetting.isAutomaticAttack)
                {
                    StartCoroutine("OnAttackLoop");
                }
                else
                {
                    OnAttack();
                }
            }
        }

        public override void StopWeaponAction(int type = 0)
        {
            if(type == 0)
            {
                isAttack = false;
                StopCoroutine("OnAttackLoop");
            }
        }

        public override void StartReload()
        {
            /// ���� ������ ���̰ų� źâ ���� 0�̸� ������ �Ұ���
            if (isReload || weaponSetting.currentMagazine <= 0) return;

            /// ���� �׼� ���߿� 'R'Ű�� ���� �������� �õ��ϸ� ���� �׼� ���� �� ������
            StopWeaponAction();
            StartCoroutine("OnReload");
        }

        /// <summary>
        /// ������ ��ݸ�尡 '�ڵ�'�� �� ����Ǵ� ���� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        /// <remarks>
        /// ���� �ݺ������� OnAttack() �޼ҵ� ����
        /// </remarks>
        private IEnumerator OnAttackLoop()
        {
            while(true)
            {
                OnAttack();

                yield return null;
            }
        }

        /// <summary>
        /// ���� �޼ҵ�
        /// </summary>
        /// /// <remarks>
        /// ���������� ������ �����ϴ� �޼ҵ�
        /// </remarks>
        private void OnAttack()
        {
            /// ���� �ֱ� Ȯ��
            float attackRate = weaponSetting.attackRate / status.AttackSpeed.currentAbility;
            if (Time.time - lastAttackTime > attackRate)
            {
                if (animator.MoveSpeed > 0.5f) return;

                lastAttackTime = Time.time;

                /// �Ѿ��� ���ٸ� ���� �Ұ���
                /// ���� �������� �������� �ڵ� ������
                if (weaponSetting.currentAmmo <= 0)
                {
                    StartReload();
                    return;
                }

                weaponSetting.currentAmmo--;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                /// ���� �ִϸ��̼� ���
                animator.Play("Fire", -1, 0);
                /// �ѱ� ����Ʈ ���
                StartCoroutine("OnMuzzleFlashEffect");
                /// ���� ���� ���
                PlaySound(audioclipFire);

                /// ������ �߻��� ���ϴ� ��ġ ����(+Impact Effect)
                //TwoStepRaycast();
                HitScan();
            }
        }

        /// <summary>
        /// �Ѿ� �߻�� ����� ����ȿ�� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator OnMuzzleFlashEffect()
        {
            muzzleFlashEffect.SetActive(true);

            /// ������ ���ݼӵ����� ������ muzzleFlashEffect��
            /// ��� Ȱ��ȭ�� �� ��Ȱ��ȭ �Ѵ�.
            yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

            muzzleFlashEffect.SetActive(false);
        }

        /// <summary>
        /// ������ �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        /// <remarks>
        /// ���������� �������� �����ϴ� �޼ҵ�
        /// </remarks>
        private IEnumerator OnReload()
        {
            isReload = true;

            /// ������ �ִϸ��̼�, ���� ���
            animator.OnReload();
            PlaySound(audioClipReload);

            while (true)
            {
                /// ���尡 ������� �ƴϰ�, ���� �ִϸ��̼��� Movement�̸�
                /// ������ �ִϸ��̼�(, ����) ����� ����Ǿ��ٴ� ��
                if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
                {
                    isReload = false;

                    /// ��ä źâ ���� 1 ���ҽ�Ű��, �ٲ� źâ ������ Text UI�� ������Ʈ
                    /// ���� ������ źâ�� ���� ���������� �����ϹǷ� �ڵ� �ּ�ó��
                    //weaponSetting.currentMagazine--;
                    //onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                    /// ���� �Ѿ��� �ִ�� �����ϰ�, �ٲ� ź �� ������ Text UI�� ������Ʈ
                    weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                    onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                    yield break;
                }

                yield return null;
            }
        }

        /// <summary>
        /// 2�ܰ迡 ���� ������ �߻��ϴ� �޼ҵ�
        /// </summary>
        /// <remarks>
        /// �÷��̾ �������� ���� Ÿ���� ��ġ�� ȭ�� �� �߾��� Aim������
        /// ������ �Ѿ��� �߻�Ǵ� ��ġ�� Aim�� �ٸ� ��ġ�� �ִ� �ѱ� �κ��̱� ������
        /// Ÿ�� ��ġ ���ķδ� ��ġ ���� �߻��Ѵ�.
        /// </remarks>
        private void TwoStepRaycast()
        {
            Ray ray;
            RaycastHit hit;
            Vector3 targetPoint = Vector3.zero;

            /// ȭ�� �߾� ��ǥ (Aim �������� Raycast ����)
            ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
            if (Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
            {
                print(MethodBase.GetCurrentMethod().Name + ": " + hit.collider.name);
                targetPoint = hit.point;
            }
            /// ���� ��Ÿ� �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� �ִ� ��Ÿ� ��ġ
            else
            {
                targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
            }
            UnityEngine.Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

            /// ù ��° Raycast�������� ����� targetPoint�� ��ǥ�������� �����ϰ�
            /// �ѱ��� ���������� �Ͽ� Raycast ����
            Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
            if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
            {
                ImpactManager.Instance.SpawnImpact(hit);

                if (hit.transform.CompareTag("ImpactEnemy"))
                {
                    print("<Color=green>" + MethodBase.GetCurrentMethod().Name + "</Color>" + '\n'
                        + "���ݷ� ������: " + status.AttackDamage.currentAbility + " / " + status.AttackDamage.currentAbility * 100 + "%\n"
                        + "���밪: " + weaponSetting.damage * status.AttackDamage.currentAbility + '\n'
                        + "�ݿø�: " + Math.Round(weaponSetting.damage * status.AttackDamage.currentAbility)
                    );
                    hit.transform.GetComponent<EnemyFSM>().TakeDamage((int)Math.Round(weaponSetting.damage * status.AttackDamage.currentAbility));
                }
                else if (hit.transform.CompareTag("InteractionObject"))
                {
                    hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
                }
            }
            UnityEngine.Debug.DrawRay(bulletSpawnPoint.position, attackDirection * weaponSetting.attackDistance, Color.blue);
        }

        private void HitScan()
        {
            /// ȭ�� �߾ӿ��� Ray�� �߻��� �ǰݵ� ��� ��ü ������ hitInfoList�� ��´�.
            Ray ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
            RaycastHit[] hitInfoList = Physics.RaycastAll(ray, weaponSetting.attackDistance);

            /// �ǰݵ� ��ü�� ���ٸ� �����Ѵ�.
            if (hitInfoList.Length <= 0)
                return;

            /// Physics.RaycatAll()�� ������ ������ �����b ��ȯ�ϱ� ������ ª�� �Ÿ������� �����Ѵ�.
            Array.Sort(hitInfoList, (RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));

            /// �ε����� Raycast�� �˻��� ����Ʈ�� ���̸� ����� �ʴ� �Ϳ� ����
            /// ����� �ɷ�ġ ��ŭ �ǰ�ó���� �Ѵ�.
            for (int i = 0; i < status.AttackNunberOfPiercing.currentAbility && i < hitInfoList.Length; i++)
            {              
                RaycastHit hitInfo = hitInfoList[i];
                int damage = (int)Math.Round(weaponSetting.damage * status.AttackDamage.currentAbility);

                ImpactManager.Instance.SpawnImpact(hitInfo);

                if (hitInfo.transform.CompareTag("ImpactEnemy"))
                {
                    UnityEngine.Debug.LogFormat("<color=red>{0} - {1}</color>\n" +
                    "hitPoint: {2}\n" + 
                    "���ط� = ��������({3}%) x ���ݷ�({4}) = {5}"
                    , MethodBase.GetCurrentMethod().Name, hitInfo.collider.name, hitInfo.point
                    , status.AttackDamage.currentAbility * 100, weaponSetting.damage, damage);
                    WhalePark18.Debug.Log(DebugCategory.Debug, MethodBase.GetCurrentMethod().Name,
                        ""
                    );

                    hitInfo.transform.GetComponent<EnemyFSM>().TakeDamage(damage);
                }
                else if(hitInfo.transform.CompareTag("InteractionObject"))
                {
                    hitInfo.transform.GetComponent<InteractionObject>().TakeDamage(damage);
                }
            }
        }

        private void ResetVariables()
        {
            isReload = false;
            isAttack = false;
        }
    }
}