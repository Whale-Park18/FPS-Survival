using UnityEngine;

using WhalePark18.Objects;
using WhalePark18.Character.Enemy;
using WhalePark18.Character.Player;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// ����ź ����ü Ŭ����
    /// </summary>
    public class WeaponGrenadeProjectile : MonoBehaviour
    {
        [Header("Explosion Barrel")]
        [SerializeField]
        private GameObject explosionPrefab;     // ���� ����Ʈ ������
        [SerializeField]
        private float explosionRadius = 10f;    // ���� ����
        [SerializeField]
        private float explosionForce = 500f;    // ���� ��
        [SerializeField]
        private float throwForce = 1000f;       // ����ź ������ ��

        private int explosionDamage;            // ���� ���ط�
        private new Rigidbody rigidbody;

        public void Setup(int damage, Vector3 rotation)
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(rotation * throwForce);

            explosionDamage = damage;
        }

        private void OnCollisionEnter(Collision collision)
        {
            /// ��򰡿� �浹 ���� ��, ���� ��ƼŬ ���� ������Ʈ ����
            Instantiate(explosionPrefab, transform.position, transform.rotation);

            /// ���� ó���� ���� ���� ������ŭ �浹 ������ ��ü �˻�
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider hit in colliders)
            {
                /// ���� ������ �ε��� ������Ʈ�� �÷��̾��� �� ó��
                PlayerController player = hit.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(explosionDamage);
                    continue;
                }

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
                    interactionObject.TakeDamage(explosionDamage);
                }

                /// ���� ������ �ε��� ������Ʈ�� �߷��� �������ִ� ������Ʈ�̸� ���� �޾� �з������� ó��
                /// �÷��̾ �� ĳ���ʹ� continue�� ó���߱� ������ �߷� ó���� ���� �ʴ´�.
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