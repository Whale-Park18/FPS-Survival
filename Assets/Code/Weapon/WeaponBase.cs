using UnityEngine;

using WhalePark18.Character.Player;

namespace WhalePark18.Weapon
{
    public enum WeaponType { Main = 0, Sub, Melee, Throw }

    /// Tip.
    ///  UnityEvent 클래스의 일반화 정의에 따라
    ///  호출할 수 있는 이벤트 메소드들의 매개변수가 결정된다.
   
    /// <summary>
    /// 탄수 관련 이벤트 클래스
    /// </summary>
    [System.Serializable]
    public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }

    /// <summary>
    /// 탄창 관련 이벤트 클래스
    /// </summary>
    [System.Serializable]
    public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

    /// <summary>
    /// 무기 최상위 클래스
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("Weapon Base")]
        [SerializeField]
        bool weaponLock = true;                         // 무기 해금 
        [SerializeField]
        protected WeaponType weaponType;                // 무기 종류
        [SerializeField]
        protected WeaponSetting weaponSetting;          // 무기 설정

        protected float lastAttackTime = 0;             // 마지막 발사시간 체크용
        protected bool isReload;                        // 재장전 중인지 체크
        protected bool isAttack = false;                // 공격 여부 체크용
        protected bool isAimModeChaing = false;         // 에임 모드 전환중 체크용
        protected bool isAimMode = false;               // 모드 전환 여부 체크용

        protected AudioSource audioSource;              // 사운드 재생 컴포넌트
        protected PlayerStatus playerStatus;            // 플레이어 상태
        protected PlayerAnimatorController animator;    // 애니메이션 재생 제어

        public bool IsAimMode => isAimMode;

        /// <summary>
        /// 외부에서 이벤트 함수 등록을 할 수 있도록 public 선언
        /// </summary>
        [HideInInspector]
        public AmmoEvent onAmmoEvent = new AmmoEvent();             // 탄수 관련 이벤트 관리
        [HideInInspector]
        public MagazineEvent onMagazineEvent = new MagazineEvent(); // 탄창 관련 이벤트 관리


        /****************************************
         * 프로퍼티
         ****************************************/
        public bool WeaponLock
        {
            set => weaponLock = value;
            get => weaponLock;
        }
        public PlayerAnimatorController Animator => animator;
        public WeaponName WeaponName => weaponSetting.weaponName;
        public int CurrentMagazine => weaponSetting.currentMagazine;
        public int MaxMagazine => weaponSetting.maxMagazine;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            animator    = GetComponent<PlayerAnimatorController>();
        }

        /// <summary>
        /// 무기 잠금 메소드
        /// </summary>
        public void Lock()
        {
            weaponLock = true;
        }

        /// <summary>
        /// 무기 잠금 해제 메소드
        /// </summary>
        public void UnLock()
        {
            weaponLock = false;
            
            weaponSetting.currentAmmo = weaponSetting.maxAmmo;
            weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        }

        /// <summary>
        /// 무기 사용 인터페이스
        /// </summary>
        /// <param name="type">마우스 버튼 타입</param>
        public abstract void StartWeaponAction(int type = 0);

        /// <summary>
        /// 무기 사용 중지 인터페이스
        /// </summary>
        /// <param name="type">마우스 버튼 타입</param>
        public abstract void StopWeaponAction(int type = 0);

        /// <summary>
        /// 재장전 인터페이스
        /// </summary>
        public abstract void StartReload();

        /// <summary>
        /// 탄창 증가 메소드
        /// </summary>
        /// <param name="magazine">탄창 증가량</param>
        /// <remarks>
        /// 아이템 등의 외부 요인으로 탄창이 증가할 때 사용
        /// </remarks>
        public virtual void IncreaseMagazine(int magazine)
        {
            weaponSetting.currentMagazine = CurrentMagazine + magazine > MaxMagazine ? MaxMagazine : CurrentMagazine + magazine;

            onMagazineEvent.Invoke(CurrentMagazine);
        }

        /// <summary>
        /// 무기 관련 사운드 플레이 메소드
        /// </summary>
        /// <param name="clip">재생할 클립</param>
        protected void PlaySound(AudioClip clip)
        {
            // 기존에 재생중인 사운드 정지
            // 새로운 사운드 clip으로 교체
            // 사운드 재생
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}