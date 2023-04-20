using System.Collections;
using UnityEngine;
using WhalePark18.Character.Enemy;
using WhalePark18.Objects;

namespace WhalePark18.Item.Active
{
    public class Bomb : MonoBehaviour
    {
        [Header("Explosion Bomb")]
        [SerializeField]
        private GameObject explosionPrefab;     // 폭발 이펙트 프리펩
        [SerializeField]
        private float explosionDelyTime = 0.3f; // 폭발 지연 시간
        [SerializeField]
        private float explosionRadius = 10f;    // 폭발 범위
        [SerializeField]
        private float explosionForce = 1000f;   // 폭발 힘
        [SerializeField]
        private int explosionDamage = 300;      // 폭발 피해량

        [SerializeField]
        private float minDelayTime = 0f;        // 낙하 최소 대기 시간
        [SerializeField]
        private float maxDelayTime = 2f;        // 낙하 최대 대기 시간

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Bounds bounds = GetComponent<Collider>().bounds;
            Instantiate(explosionPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);

            Damage();

            Destroy(gameObject);
        }

        private void Damage()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider hit in colliders)
            {
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
                    interactionObject.TakeDamage(300);
                }

                /// 폭발 범위에 부딪힌 오브젝트가 중력을 가지고있는 오브젝트이면 힘을 받아 밀려나도록 처리
                /// 플레이어나 적 캐릭터는 continue로 처리했기 때문에 중력 처리를 받지 않는다.
                Rigidbody rigidbody = hit.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }
        }

        public void Use()
        {
            StartCoroutine("DropDelay");
        }

        private IEnumerator DropDelay()
        {
            yield return new WaitForSeconds(Random.Range(minDelayTime, maxDelayTime));
            rb.useGravity = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);

            Gizmos.color = Color.red;
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit);
            Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
        }
    }
}