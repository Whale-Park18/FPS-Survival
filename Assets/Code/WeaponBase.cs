using System.Collections;
using UnityEngine;

public enum WeaponType { Main = 0, Sub, Melee, Throw }

/// Tip.
///  UnityEvent 클래스의 일반화 정의에 따라
///  호출할 수 있는 이벤트 메소드들의 매개변수가 결정된다.
[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Base")]
    [SerializeField]
    protected WeaponType                weaponType;         // 무기 종류
    [SerializeField]
    protected WeaponSetting             weaponSetting;      // 무기 설정

    protected float                     lastAttackTime = 0; // 마지막 발사시간 체크용
    protected bool                      isReload;           // 재장전 중인지 체크
    protected bool                      isAttack = false;   // 공격 여부 체크용

    protected AudioSource               audioSource;        // 사운드 재생 컴포넌트
    protected PlayerAnimatorController  animator;           // 애니메이션 재생 제어
 
    /// <summary>
    /// 외부에서 이벤트 함수 등록을 할 수 있도록 public 선언
    /// </summary>
    [HideInInspector]
    public AmmoEvent                onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent            onMagazineEvent = new MagazineEvent();

    /// <summary>
    /// 외부에서 필요한 정보를 열람하기 위해 정의한 Get 프로퍼티
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
        // 기존에 재생중인 사운드 정지
        // 새로운 사운드 clip으로 교체
        // 사운드 재생
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
