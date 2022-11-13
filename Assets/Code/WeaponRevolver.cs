using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class WeaponRevolver : WeaponBase
{
    [Header("Fire Effect")]
    [SerializeField]
    private GameObject          muzzleFlashEffect;  // �ѱ� ����Ʈ

    [Header("Spawn Points")]
    [SerializeField]
    private Transform           bulletSpawnPoint;   // �Ѿ� ���� ��ġ

    [Header("Audio Clip")]
    [SerializeField]
    private AudioClip           audioclipFire;      // ���� ����
    [SerializeField]            
    private AudioClip           audioClipReload;    // ���� ����

    private ImpactMemoryPool    impactMemoryPool;   // ���� ȿ�� ���� �� Ȱ��/��Ȱ�� ����
    private Camera              mainCamera;         // ���� �߻�

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

    private void Awake()
    {
        base.Setup();

        impactMemoryPool    = GetComponent<ImpactMemoryPool>();
        mainCamera          = Camera.main;

        /// ó�� źâ, ź ���� �ִ�� ����
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    public override void StopWeaponAction(int type = 0)
    {
        if(type == 0 && isAttack == false && isReload == false)
        {
            OnAttack();
        }
    }

    public override void StartWeaponAction(int type = 0)
    {
        isAttack = false;
    }

    public override void StartReload()
    {
        /// ���� ������ ���̰ų� źâ ���� 0�̸� ������ �Ұ���
        if (isReload || weaponSetting.currentMagazine <= 0) return;

        /// ���� �׼� ���߿� 'R'Ű�� ���� �������� �õ��ϸ� ���� �׼� ���� �� ������
        StopWeaponAction();
        StartCoroutine("OnReload");
    }

    public void OnAttack()
    {
        if(Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            if (animator.MoveSpeed > 0.5f) return;

            lastAttackTime = Time.time;

            if (weaponSetting.currentAmmo <= 0) return;

            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            /// ���� �ִϸ��̼� ���
            animator.Play("Fire", -1, 0);
            /// �ѱ� ����Ʈ ���
            StartCoroutine("OnMuzzleFlashEffect");
            /// ���� ���� ���
            PlaySound(audioclipFire);

            /// ������ �߻��� ���ϴ� ��ġ ����(+Impact Effect)
            TwoStepRaycast();
        }
    }

    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        /// ������ ���ݼӵ����� ������ muzzleFlashEffect��
        /// ��� Ȱ��ȭ�� �� ��Ȱ��ȭ �Ѵ�.
        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }

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

    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
    }

}
