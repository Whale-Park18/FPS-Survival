using System.Collections;
using UnityEngine;

public enum WeaponType { Main = 0, Sub, Melee, Throw }

/// Tip.
///  UnityEvent Ŭ������ �Ϲ�ȭ ���ǿ� ����
///  ȣ���� �� �ִ� �̺�Ʈ �޼ҵ���� �Ű������� �����ȴ�.
[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Base")]
    [SerializeField]
    protected WeaponType                weaponType;         // ���� ����
    [SerializeField]
    protected WeaponSetting             weaponSetting;      // ���� ����

    protected float                     lastAttackTime = 0; // ������ �߻�ð� üũ��
    protected bool                      isReload;           // ������ ������ üũ
    protected bool                      isAttack = false;   // ���� ���� üũ��

    protected AudioSource               audioSource;        // ���� ��� ������Ʈ
    protected PlayerAnimatorController  animator;           // �ִϸ��̼� ��� ����
 
    /// <summary>
    /// �ܺο��� �̺�Ʈ �Լ� ����� �� �� �ֵ��� public ����
    /// </summary>
    [HideInInspector]
    public AmmoEvent                onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent            onMagazineEvent = new MagazineEvent();

    /// <summary>
    /// �ܺο��� �ʿ��� ������ �����ϱ� ���� ������ Get ������Ƽ
    /// </summary>
    public PlayerAnimatorController Animator => animator;
    public WeaponName               WeaponName => weaponSetting.weaponName;
    public int                      CurrentMagazine => weaponSetting.currentMagazine;
    public int                      MaxMagazine => weaponSetting.maxMagazine;

    public abstract void StartWeaponAction(int type = 0);
    public abstract void StopWeaponAction(int type = 0);
    public abstract void StartReload();

    protected void PlaySound(AudioClip clip)
    {
        // ������ ������� ���� ����
        // ���ο� ���� clip���� ��ü
        // ���� ���
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    protected void Setup()
    {
        audioSource = GetComponent<AudioSource>();
        animator    = GetComponent<PlayerAnimatorController>();
    }

    public virtual void IncreaseMagazine(int magazine)
    {
        weaponSetting.currentMagazine = CurrentMagazine + magazine > MaxMagazine ? MaxMagazine : CurrentMagazine + magazine;

        onMagazineEvent.Invoke(CurrentMagazine);
    }
}
