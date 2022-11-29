using UnityEngine;

namespace WhalePark18.Objects
{
    /// <summary>
    /// 파괴 가능한 드럼통 클래스
    /// </summary>
    public class DesturctibleBarrel : InteractionObject
    {
        [Header("Destructible Barrel")]
        [SerializeField]
        private GameObject destructibleBarrelPieces;    // 파괴시 대체되는 프리팹(파편 오브젝트)

        private bool isDestroyed = false;               // 파괴 상태

        public override void TakeDamage(int damage)
        {
            currentHP = -damage;

            if (currentHP <= 0 && isDestroyed == false)
            {
                isDestroyed = true;

                /// 파편 오브젝트가 현재 오브젝트를 대신한다.
                Instantiate(destructibleBarrelPieces, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }
}