using System.Collections;
using UnityEngine;

using WhalePark18.Character.Player;
using WhalePark18.Character.Enemy;

namespace WhalePark18.Objects
{
    /// <summary>
    /// 폭발되는 드럼통 클래스
    /// </summary>
    public class ExplosionBarrel : InteractionObject
    {
        [Header("Explosion Barrel")]
        [SerializeField]
        private GameObject explosionPrefab;     // 폭발 이펙트 프리펩
        [SerializeField]
        private float explosionDelyTime = 0.3f; // 폭발 지연 시간
        [SerializeField]
        private float explosionRadius = 10f;    // 폭발 범위
        [SerializeField]
        private float explosionForce = 1000f;   // 폭발 힘
        [SerializeField]
        private int explosionDamage = 100;      // 폭발 피해량

        private bool isExplode = false;         // 폭발 상태

        public override void TakeDamage(int damage)
        {
            currentHP -= damage;

            if (currentHP <= 0 && isExplode == false)
            {
                StartCoroutine("ExplodeBarrel");
            }
        }

        /// <summary>
        /// 폭발 처리 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator ExplodeBarrel()
        {
            yield return new WaitForSeconds(explosionDelyTime);

            /// 근처의 배럴이 터져서 다시 현재 배럴을 터트리려고 할 때(Stack Over Flow 방지)
            isExplode = true;

            /// 폭발 이펙트 생성
            Bounds bounds = GetComponent<Collider>().bounds;
            Instantiate(explosionPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);

            /// 폭발 범위에 있는 모든 오브젝트의 Collider 정보를 받아와 폭발 효과 처리
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider hit in colliders)
            {
                /// 폭발 범위에 부딪힌 오브젝트가 플레이어일 때 처리
                PlayerController player = hit.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(explosionDamage / 5);
                    continue;
                }

                /// 폭발 범위에 부딪힌 오브젝트가 적 캐릭터일 때 처리
                EnemyFSM enemy = hit.GetComponent<EnemyFSM>();
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

            /// 배럴 오브젝트 삭제
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}