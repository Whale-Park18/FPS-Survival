using System.Collections;
using UnityEngine;
using WhalePark18.Character.Enemy;

namespace WhalePark18.Projectile
{
    public class ProjectileBase : MonoBehaviour
    {
        [SerializeField]
        private float projectileSpeed = 5f;
        [SerializeField]
        private float projectileDistance = 30f;
        [SerializeField]
        private int damage = 10;

        public void Move()
        {
            StartCoroutine("OnMove");
        }

        private IEnumerator OnMove()
        {
            float moveDistance = 0f;
            while(true)
            {
                if (moveDistance >= projectileDistance)
                {
                    Destroy(gameObject);
                    yield break;
                }

                transform.position += transform.forward * projectileSpeed * Time.deltaTime;
                moveDistance += projectileSpeed * Time.deltaTime;

                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ImpactEnemy"))
            {
                other.GetComponent<EnemyFSM>().TakeDamage(damage);

                Destroy(gameObject);
            }
        }
    }
}