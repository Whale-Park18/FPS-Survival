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
        /****************************************
         * ProjectieBase
         ****************************************/
        [SerializeField]
        protected float projectileSpeed = 5f;
        [SerializeField]
        protected float projectileDistance = 30f;
        [SerializeField]
        protected int damage = 10;

        /****************************************
         * ProjectileBase 관리용
         ****************************************/
        protected int id;
        [SerializeField]
        private string baseName;

        /****************************************
         * 프로퍼티
         ****************************************/
        public int ID => id;

        public virtual void Setup(int id)
        {
            this.id = id;
            name = $"{baseName}_{id}";
        }

        public virtual void Fire(Vector3 targetPosition)
        {
            StartCoroutine(OnMove(targetPosition));
        }

        /// <summary>
        /// targetPosition을 향해 projectileSpeed 속도로 
        /// 이동하는 메소드
        /// </summary>
        /// <param name="targetPosition">목표 위치</param>
        /// <returns>코루틴</returns>
        /// <remarks>
        /// 기본적으로 직선으로 움직이지만 투사체에 따라 오버로딩을 활용해
        /// 목표물을 추적하는 코드로 작성한다.
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
        /// 투사체가 작동을 멈췄을 때, 작동
        /// </summary>
        protected abstract void Stop();
    }
}