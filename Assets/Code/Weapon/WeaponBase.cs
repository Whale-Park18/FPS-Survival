using UnityEngine;

using WhalePark18.Character.Player;

namespace WhalePark18.Weapon
{
    public enum WeaponType { Main = 0, Sub, Melee, Throw }

    /// Tip.
    ///  UnityEvent Ŭ������ �Ϲ�ȭ ���ǿ� ����
    ///  ȣ���� �� �ִ� �̺�Ʈ �޼ҵ���� �Ű������� �����ȴ�.
   
    /// <summary>
    /// ź�� ���� �̺�Ʈ Ŭ����
    /// </summary>
    [System.Serializable]
    public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

    /// <summary>
    /// źâ ���� �̺�Ʈ Ŭ����
    /// </summary>
    [System.Serializable]
    public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

    /// <summary>
    /// ���� �ֻ��� Ŭ����
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("Weapon Base")]
        [SerializeField]
        protected WeaponType weaponType;                // ���� ����
        [SerializeField]
        protected WeaponSetting weaponSetting;          // ���� ����

        protected float lastAttackTime = 0;             // ������ �߻�ð� üũ��
        protected bool isReload;                        // ������ ������ üũ
        protected bool isAttack = false;                // ���� ���� üũ��

        protected AudioSource audioSource;              // ���� ��� ������Ʈ
        protected PlayerAnimatorController animator;    // �ִϸ��̼� ��� ����

        /// <summary>
        /// �ܺο��� �̺�Ʈ �Լ� ����� �� �� �ֵ��� public ����
        /// </summary>
        [HideInInspector]
        public AmmoEvent onAmmoEvent = new AmmoEvent();             // ź�� ���� �̺�Ʈ ����
        [HideInInspector]
        public MagazineEvent onMagazineEvent = new MagazineEvent(); // źâ ���� �̺�Ʈ ����

        /// <summary>
        /// �ܺο��� �ʿ��� ������ �����ϱ� ���� ������ Get ������Ƽ
        /// </summary>
        public PlayerAnimatorController Animator => animator;
        public WeaponName WeaponName => weaponSetting.weaponName;
        public int CurrentMagazine => weaponSetting.currentMagazine;
        public int MaxMagazine => weaponSetting.maxMagazine;

        /// <summary>
        /// ���� ��� �������̽�
        /// </summary>
        /// <param name="type">���콺 ��ư Ÿ��</param>
        public abstract void StartWeaponAction(int type = 0);

        /// <summary>
        /// ���� ��� ���� �������̽�
        /// </summary>
        /// <param name="type">���콺 ��ư Ÿ��</param>
        public abstract void StopWeaponAction(int type = 0);

        /// <summary>
        /// ������ �������̽�
        /// </summary>
        public abstract void StartReload();

        /// <summary>
        /// ���� ���� ���� �÷��� �޼ҵ�
        /// </summary>
        /// <param name="clip">����� Ŭ��</param>
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
            animator = GetComponent<PlayerAnimatorController>();
        }

        /// <summary>
        /// źâ ���� �޼ҵ�
        /// </summary>
        /// <param name="magazine">źâ ������</param>
        /// <remarks>
        /// ������ ���� �ܺ� �������� źâ�� ������ �� ���
        /// </remarks>
        public virtual void IncreaseMagazine(int magazine)
        {
            weaponSetting.currentMagazine = CurrentMagazine + magazine > MaxMagazine ? MaxMagazine : CurrentMagazine + magazine;

            onMagazineEvent.Invoke(CurrentMagazine);
        }
    }
}