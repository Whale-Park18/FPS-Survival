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
        private GameObject explosionPrefab;     // ���� ����Ʈ ������
        [SerializeField]
        private float explosionDelyTime = 0.3f; // ���� ���� �ð�
        [SerializeField]
        private float explosionRadius = 10f;    // ���� ����
        [SerializeField]
        private float explosionForce = 1000f;   // ���� ��
        [SerializeField]
        private int explosionDamage = 300;      // ���� ���ط�

        [SerializeField]
        private float minDelayTime = 0f;        // ���� �ּ� ��� �ð�
        [SerializeField]
        private float maxDelayTime = 2f;        // ���� �ִ� ��� �ð�

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
                /// ���� ������ �ε��� ������Ʈ�� �� ĳ������ �� ó��
                EnemyBase enemy = hit.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage(explosionDamage);
                    continue;
                }

                /// ���� ������ �ε��� ������Ʈ�� ��ȣ�ۿ� ������Ʈ�̸� TakeDamage()�� ���ظ� ��
                InteractionObject interactionObject = hit.GetComponent<InteractionObject>();
                if (interactionObject != null)
                {
                    interactionObject.TakeDamage(300);
                }

                /// ���� ������ �ε��� ������Ʈ�� �߷��� �������ִ� ������Ʈ�̸� ���� �޾� �з������� ó��
                /// �÷��̾ �� ĳ���ʹ� continue�� ó���߱� ������ �߷� ó���� ���� �ʴ´�.
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