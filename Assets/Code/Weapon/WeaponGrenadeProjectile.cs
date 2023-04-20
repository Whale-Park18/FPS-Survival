using UnityEngine;

using WhalePark18.Objects;
using WhalePark18.Character.Enemy;
using WhalePark18.Character.Player;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// 수류탄 투사체 클래스
    /// </summary>
    public class WeaponGrenadeProjectile : MonoBehaviour
    {
        [Header("Explosion Barrel")]
        [SerializeField]
        private GameObject explosionPrefab;     // 폭발 이펙트 프리팹
        [SerializeField]
        private float explosionRadius = 10f;    // 폭발 범위
        [SerializeField]
        private float explosionForce = 500f;    // 폭발 힘
        [SerializeField]
        private float throwForce = 1000f;       // 수류탄 던지는 힘

        private int explosionDamage;            // 폭발 피해량
        private new Rigidbody rigidbody;

        public void Setup(int damage, Vector3 rotation)
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(rotation * throwForce);

            explosionDamage = damage;
        }

        private void OnCollisionEnter(Collision collision)
        {
            /// 어딘가에 충돌 됐을 때, 폭발 파티클 게임 오브젝트 생성
            Instantiate(explosionPrefab, transform.position, transform.rotation);

            /// 폭발 처리를 위해 폭발 범위만큼 충돌 가능한 객체 검색
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider hit in colliders)
            {
                /// 폭발 범위에 부딪힌 오브젝트가 플레이어일 때 처리
                PlayerController player = hit.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(explosionDamage);
                    continue;
                }

                /// 폭발 범위에 부딪힌 오브젝트가 적 캐릭터일 때 처리
                EnemyBase enemy = hit.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage(explosionDamage);
                    continue;
                }

                /// 폭발 범위에 부딪힌 오브젝트가 상호작용 오브젝트이면 TakeDamage()로 피해를 줌
                InteractionObject interactionObject = hit.GetComponent<InteractionObject>();
                if (interactionObject != null)
                {
                    interactionObject.TakeDamage(explosionDamage);
                }

                /// 폭발 범위에 부딪힌 오브젝트가 중력을 가지고있는 오브젝트이면 힘을 받아 밀려나도록 처리
                /// 플레이어나 적 캐릭터는 continue로 처리했기 때문에 중력 처리를 받지 않는다.
                Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}