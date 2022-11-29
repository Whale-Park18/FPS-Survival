using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using WhalePark18.MemoryPool;
using WhalePark18.Objects;
using WhalePark18.Character.Enemy;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// ����(����): ���� ���� Ŭ����
    /// </summary>
    public class WeaponAssultRifle : WeaponBase
    {
        [Header("Fire Effects")]
        [SerializeField]
        private GameObject muzzleFlashEffect;       // �ѱ� ����Ʈ (On/Off)

        [Header("Spawn Points")]
        [SerializeField]
        private Transform casingSpawnPoint;         // ź�� ���� ��ġ
        [SerializeField]
        private Transform bulletSpawnPoint;         // �Ѿ� ���� ��ġ

        [Header("Audio Cilps")]
        [SerializeField]
        private AudioClip audioClipTakeOutWeapon;   // ���� ���� ����
        [SerializeField]
        private AudioClip audioclipFire;            // ���� ����
        [SerializeField]
        private AudioClip audioClipReload;          // ������ ����

        [Header("Aim UI")]
        [SerializeField]
        private Image imageAim;                     // default/aim ��忡 ���� Aim �̹��� Ȱ��/��Ȱ��

        private bool isModeChange = false;          // ��� ��ȯ ���� üũ��
        private float defaultModeFOV = 60f;         // �⺻��忡���� ī�޶� FOV
        private float aimModeFOV = 30f;             // Aim��忡���� ī�޶� FOV(�þ߰�)

        private CasingMemoryPool casingMemoryPool;  // ź�� ���� ��, Ȱ��/��Ȱ�� ����
        private ImpactMemoryPool impactMemoryPool;  // ���� ȿ�� ���� �� Ȱ��/��Ȱ�� ����
        private Camera mainCamera;                  // ���� �߻�

        private void Awake()
        {
            base.Setup();

            casingMemoryPool = GetComponent<CasingMemoryPool>();
            impactMemoryPool = GetComponentInParent<ImpactMemoryPool>();
            mainCamera = Camera.main;

            /// ó�� źâ �� �ִ�� ����
            weaponSetting.currentMagazine = weaponSetting.maxMagazine;

            /// ó�� ź �� �ִ�� ����
            weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        }

        private void OnEnable()
        {
            /// ���� ���� ���� ���
            PlaySound(audioClipTakeOutWeapon);

            /// �ѱ� ����Ʈ ������Ʈ ��Ȱ��ȭ
            muzzleFlashEffect.SetActive(false);

            /// �̺�Ʈ Ŭ������ ��ϵ� �ܺ� �޼ҵ尡 ȣ��Ǵ� ������
            /// �̺�Ʈ Ŭ������ Invoke() �޼ҵ尡 ȣ��� ���̴�.

            /// ���Ⱑ Ȱ��ȭ�� �� �ش� ������ źâ ������ ����
            onMagazineEvent.Invoke(weaponSetting.currentMagazine);

            /// ���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ ����
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            ResetVariables();
        }

        public override void StartWeaponAction(int type = 0)
        {
            /// ������ ���� ���� ���� �׼��� �� �� ����.
            if (isReload) return;

            /// ��� ��ȯ���̸� ���� �׼��� �� �� ����.
            if (isModeChange) return;

            /// ���콺 ���� Ŭ��(���� ����)
            if (type == 0)
            {
                /// ���� ����
                if (weaponSetting.isAutomaticAttack)
                {
                    isAttack = true;

                    /// OnAttack()�� �� �����Ӹ��� ���
                    StartCoroutine("OnAttackLoop");
                }
                /// �ܹ� ����
                else
                {
                    /// ���� ���� �޼ҵ�
                    OnAttack();
                }
            }
            /// ���콺 ������ Ŭ��(��� ��ȯ)
            else
            {
                /// ���� ���� ���� ��� ��ȯ�� �� �� ����.
                if (isAttack) return;

                StartCoroutine("OnModeChange");
            }
        }

        public override void StopWeaponAction(int type = 0)
        {
            /// ���콺 ���� Ŭ��
            if (type == 0)
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
            while (true)
            {
                OnAttack();

                yield return null;
            }
        }

        /// <summary>
        /// ���� �޼ҵ�
        /// </summary>
        /// <remarks>
        /// ���������� ������ �����ϴ� �޼ҵ�
        /// </remarks>
        private void OnAttack()
        {
            /// ���� �ֱ� Ȯ��
            if (Time.time - lastAttackTime > weaponSetting.attackRate)
            {
                /// �޸��� ���̸� ������ �� ����.
                if (animator.MoveSpeed > 0.5f)
                {
                    return;
                }

                /// ���� �ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
                lastAttackTime = Time.time;

                /// ź�� ���� ������ ���� �Ұ���
                if (weaponSetting.currentAmmo <= 0)
                {
                    return;
                }
                /// ���ݽ� currentAmmo 1 ����, ź �� UI ������Ʈ
                weaponSetting.currentAmmo--;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                /// ���� �ִϸ��̼� ���
                /// 
                /// �˾ư���
                /// animator.Play("Fire")
                ///  - ���� �ִϸ��̼��� �ݺ��� �� �߰��� ���� ���ϰ� ��� �Ϸ� �� �ٽ� ���
                /// animator.Play("Fire", -1, 0)
                ///  - ���� �ִϸ��̼��� �ݺ��� �� �ִϸ��̼��� ���� ó������ �ٽ� ���
                string animation = animator.AimModeIs ? "AimFire" : "Fire";
                animator.Play(animation, -1, 0);

                /// �ѱ� ����Ʈ ��� (default ��� �� ���� ���)
                if (animator.AimModeIs == false) StartCoroutine("OnMuzzleFlashEffect");

                /// ���� ���� ���
                PlaySound(audioclipFire);

                /// ź�� ����
                casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

                /// ������ �߻��� ���ϴ� ��ġ ����(+Impact Effect)
                TwoStepRaycast();
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
                if (audioSource.isPlaying == false && (animator.CurrentAnimationIs("Movement") || animator.CurrentAnimationIs("aim_fire_pose@assault_rifle_01")))
                {
                    isReload = false;

                    /// ��ä źâ ���� 1 ���ҽ�Ű��, �ٲ� źâ ������ Text UI�� ������Ʈ
                    weaponSetting.currentMagazine--;
                    onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                    /// ���� ź���� �ִ�� �����ϰ�, �ٲ� ź �� ������ Text UI�� ������Ʈ
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
                targetPoint = hit.point;
            }
            /// ���� ��Ÿ� �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� �ִ� ��Ÿ� ��ġ
            else
            {
                targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
            }
            Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red);

            /// ù ��° Raycast�������� ����� targetPoint�� ��ǥ�������� �����ϰ�
            /// �ѱ��� ���������� �Ͽ� Raycast ����
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
        /// Aim ��� ���� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator OnModeChange()
        {
            float current = 0;  // Lerp�� �̿��� �ڿ������� Ȯ�븦 ���� ����
            float percent = 0;  // Lerp�� �̿��� �ڿ������� Ȯ�븦 ���� ����
            float time = 0.35f; // Mode Change�� �ɸ��� �ð�

            animator.AimModeIs = !animator.AimModeIs;
            imageAim.enabled = !imageAim.enabled;

            float start = mainCamera.fieldOfView;
            float end = animator.AimModeIs ? aimModeFOV : defaultModeFOV;

            isModeChange = true;

            while (percent < 1)
            {
                current += Time.deltaTime;
                percent = current / time;

                /// ��忡 ���� ī�޶��� �þ߰��� ����
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