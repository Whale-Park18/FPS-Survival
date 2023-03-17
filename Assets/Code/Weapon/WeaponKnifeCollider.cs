using System.Collections;
using UnityEngine;

using WhalePark18;
using WhalePark18.Objects;
using WhalePark18.Character.Enemy;
using WhalePark18.Manager;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// ����(Į): Į��(���� ����)�� �ݶ��̴��� �����ϴ� Ŭ����
    /// </summary>
    public class WeaponKnifeCollider : MonoBehaviour
    {
        [SerializeField]
        private ImpactMemoryPool impactMemoryPool;
        [SerializeField]
        private Transform knifeTransform;

        private new Collider collider;
        private int damage;

        private void Awake()
        {
            collider = GetComponent<Collider>();
            collider.enabled = false;
        }

        /// <summary>
        /// �ݶ��̴� Ȱ��ȭ �������̽�
        /// </summary>
        /// <param name="damage">���ط�</param>
        public void StartCollider(int damage)
        {
            this.damage = damage;
            collider.enabled = true;

            StartCoroutine("DisablebyTime", 0.1f);
        }

        /// <summary>
        /// �ݶ��̴� ��Ȱ��ȭ �޼ҵ�
        /// </summary>
        /// <param name="time">��Ȱ��ȭ ������ ��� �ð�</param>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator DisablebyTime(float time)
        {
            yield return new WaitForSeconds(time);

            collider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            ImpactManager.Instance.SpawnImpact(other, knifeTransform);

            if (other.CompareTag("ImpactEnemy"))
            {
                other.GetComponent<EnemyFSM>().TakeDamage(damage);
            }
            else if (other.CompareTag("InteractionObject"))
            {
                other.GetComponent<InteractionObject>().TakeDamage(damage);
            }
        }
    }
}