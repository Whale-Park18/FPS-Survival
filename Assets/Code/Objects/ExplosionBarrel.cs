using System.Collections;
using UnityEngine;

using WhalePark18.Character.Player;
using WhalePark18.Character.Enemy;

namespace WhalePark18.Objects
{
    /// <summary>
    /// ���ߵǴ� �巳�� Ŭ����
    /// </summary>
    public class ExplosionBarrel : InteractionObject
    {
        [Header("Explosion Barrel")]
        [SerializeField]
        private GameObject explosionPrefab;     // ���� ����Ʈ ������
        [SerializeField]
        private float explosionDelyTime = 0.3f; // ���� ���� �ð�
        [SerializeField]
        private float explosionRadius = 10f;    // ���� ����
        [SerializeField]
        private float explosionForce = 1000f;   // ���� ��
        [SerializeField]
        private int explosionDamage = 100;      // ���� ���ط�

        private bool isExplode = false;         // ���� ����

        public override void TakeDamage(int damage)
        {
            currentHP -= damage;

            if (currentHP <= 0 && isExplode == false)
            {
                StartCoroutine("ExplodeBarrel");
            }
        }

        /// <summary>
        /// ���� ó�� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator ExplodeBarrel()
        {
            yield return new WaitForSeconds(explosionDelyTime);

            /// ��ó�� �跲�� ������ �ٽ� ���� �跲�� ��Ʈ������ �� ��(Stack Over Flow ����)
            isExplode = true;

            /// ���� ����Ʈ ����
            Bounds bounds = GetComponent<Collider>().bounds;
            Instantiate(explosionPrefab, new Vector3(bounds.center.x, bounds.min.y, bounds.center.z), transform.rotation);

            /// ���� ������ �ִ� ��� ������Ʈ�� Collider ������ �޾ƿ� ���� ȿ�� ó��
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider hit in colliders)
            {
                /// ���� ������ �ε��� ������Ʈ�� �÷��̾��� �� ó��
                PlayerController player = hit.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(explosionDamage / 5);
                    continue;
                }

                /// ���� ������ �ε��� ������Ʈ�� �� ĳ������ �� ó��
                EnemyFSM enemy = hit.GetComponent<EnemyFSM>();
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

            /// �跲 ������Ʈ ����
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}