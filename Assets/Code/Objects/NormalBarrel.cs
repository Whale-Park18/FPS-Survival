namespace WhalePark18.Objects
{
    /// <summary>
    /// 일반 드럼통 클래스
    /// </summary>
    public class NormalBarrel : InteractionObject
    {
        public override void TakeDamage(int damage)
        {
            /// 태그가 "InteractionObject"인 모든 상호작용 오브젝트는
            /// InteractionObejct.TakeDamage()를 호출하기 때문에
            /// 기본 드럼통도 INteractionObject 클래스를 상속 받는 클래스로 제작한다.
            /// 다만 기본 드럼통은 설정상 체력이 무한해서 부서지지 않기 때문에 체력이 닳지
            /// 않도록 TakeDamage() 내부가 비어있다.
        }
    }
}