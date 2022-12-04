/// 무기의 종류가 여러 종류일 때 공용으로 사용하는 변수들은 구조체로 묶어서 정의하면
/// 변수가 추가/삭제될 때 구조체에 선언하기 때문에 추가/삭제에 대한 관리가 용이함

/// 직렬화 속성
/// Tip.
///  [System.Serializable]을 이용해 직렬화 하지 않으면 다른 클래스의
///  변수로 생성되었을 때 인스펙터 뷰에 멤버 변수들의 목록이 뜨지 않는다.

namespace WhalePark18.Weapon
{
    public enum WeaponName { AssaultRifle = 0, Revolver, CombatKnife, HandGrenade }

    /// <summary>
    /// 무기 속성 구조체
    /// </summary>
    [System.Serializable]
    public struct WeaponSetting
    {
        public WeaponName weaponName;   // 무기 이름
        public int damage;              // 무기 공격력
        public int currentMagazine;     // 현재 탄창 수
        public int maxMagazine;         // 최대 탄창 수
        public int currentAmmo;         // 현재 탄약 수
        public int maxAmmo;             // 최대 탄약 수
        public float attackRate;        // 공격 딜레이
        public float attackDistance;    // 공격 사거리
        public bool isAutomaticAttack;  // 연속 공격 여부
    }
}