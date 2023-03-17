using System.Collections;
using UnityEngine;
using WhalePark18.Character.Enemy;
using WhalePark18.Character.Player;

namespace WhalePark18.Projectile
{
    public class ProjectileBase : MonoBehaviour
    {
        [SerializeField]
        protected float projectileSpeed = 5f;
        [SerializeField]
        protected float projectileDistance = 30f;
        [SerializeField]
        protected int damage = 10;

        public virtual void Move()
        {
            StartCoroutine("OnMove");
        }

        protected virtual IEnumerator OnMove()
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
            }
            else if(other.CompareTag("Player"))
            {
                other.GetComponent<PlayerController>().TakeDamage(damage);
            }
            else if(other.CompareTag("Intercation"))
            {

            }
            
            Destroy(gameObject);
        }
    }
}