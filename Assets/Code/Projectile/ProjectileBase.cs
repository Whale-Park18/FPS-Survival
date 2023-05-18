using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using WhalePark18.Character.Enemy;
using WhalePark18.Character.Player;

namespace WhalePark18.Projectile
{
    [RequireComponent(typeof(CapsuleCollider))]
    public abstract class ProjectileBase : MonoBehaviour
    {
        [Header("Projectile Info")]       
        protected int id;
        [SerializeField]
        private string baseName;

        [Header("Projectile Attack Info")]
        [SerializeField]
        protected float projectileSpeed = 5f;
        [SerializeField]
        protected float projectileDistance = 30f;
        [SerializeField]
        protected int damage = 10;
        protected bool isFired = false;

        /****************************************
         * ������Ƽ
         ****************************************/
        public int ID => id;
        public bool IsFired => isFired;

        public virtual void Setup(int id)
        {
            this.id = id;
            name = $"{baseName}_{id}";
        }

        public virtual void Fire(Vector3 targetPosition)
        {
            StartCoroutine(OnMove(targetPosition));
            isFired = true;
        }

        /// <summary>
        /// targetPosition�� ���� projectileSpeed �ӵ��� 
        /// �̵��ϴ� �޼ҵ�
        /// </summary>
        /// <param name="targetPosition">��ǥ ��ġ</param>
        /// <returns>�ڷ�ƾ</returns>
        /// <remarks>
        /// �⺻������ �������� ���������� ����ü�� ���� �����ε��� Ȱ����
        /// ��ǥ���� �����ϴ� �ڵ�� �ۼ��Ѵ�.
        /// </remarks>
        protected virtual IEnumerator OnMove(Vector3 targetPosition)
        {
            Vector3 start = transform.position;
            transform.LookAt(targetPosition);

            for(float moveDistance = 0; moveDistance < projectileDistance; moveDistance = Vector3.Distance(transform.position, targetPosition))
            {
                transform.position += transform.forward * projectileSpeed * Time.deltaTime;

                yield return null;
            }

            Stop();
        }

        /// <summary>
        /// ����ü�� �۵��� ������ ��, �۵�
        /// </summary>
        public virtual void Stop()
        {
            isFired = false;
        }
    }
}