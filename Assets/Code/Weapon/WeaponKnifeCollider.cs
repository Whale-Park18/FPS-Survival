using System.Collections;
using UnityEngine;

using WhalePark18;
using WhalePark18.Objects;
using WhalePark18.Character.Enemy;
using WhalePark18.Manager;

namespace WhalePark18.Weapon
{
    /// <summary>
    /// 무기(칼): 칼날(공격 범위)의 콜라이더를 제어하는 클래스
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
        /// 콜라이더 활성화 인터페이스
        /// </summary>
        /// <param name="damage">피해량</param>
        public void StartCollider(int damage)
        {
            this.damage = damage;
            collider.enabled = true;

            StartCoroutine("DisablebyTime", 0.1f);
        }

        /// <summary>
        /// 콜라이더 비활성화 메소드
        /// </summary>
        /// <param name="time">비활성화 전까지 대기 시간</param>
        /// <returns>코루틴</returns>
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