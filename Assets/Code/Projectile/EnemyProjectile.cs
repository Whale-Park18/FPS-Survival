using System.Collections;
using UnityEngine;
using WhalePark18.Character.Player;

namespace WhalePark18.Projectile
{
    public class EnemyProjectile : MonoBehaviour
    {
        private MovementTransform movment;
        private float projectileDistance = 30f;   // 최대 발사 거리
        private int damage = 5;                 // 발사체 공격력

        public void Setup(Vector3 position)
        {
            movment = GetComponent<MovementTransform>();

            StartCoroutine("OnMove", position);
        }

        private IEnumerator OnMove(Vector3 targetPosition)
        {
            Vector3 start = transform.position;

            movment.MoveTo((targetPosition - start).normalized);

            while (true)
            {
                if (Vector3.Distance(transform.position, start) >= projectileDistance)
                {
                    Destroy(gameObject);

                    yield break;
                }

                yield return null;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerController>().TakeDamage(damage);

                Destroy(gameObject);
            }
        }
    }
}