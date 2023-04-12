using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.Character.Enemy;
using WhalePark18.Character.Player;
using WhalePark18.Manager;

namespace WhalePark18.Projectile
{
    public class EnemyNormalMissile : ProjectileBase
    {
        private ParticleSystem boost;

        private void Awake()
        {
            boost = GetComponentInChildren<ParticleSystem>();
        }

        public override void Fire(Vector3 targetPosition)
        {
            base.Fire(targetPosition);
            boost.Play();
        }

        protected override void Stop()
        {
            boost.Stop();
            EnemyProjectileManager.Instance.ReturnEnemyNormalMissile(this);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            LogManager.ConsoleDebugLog(gameObject.name, $"피격 시간: {Time.time}");

            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerController>().TakeDamage(damage);
            }
            else if (other.CompareTag("Intercation"))
            {

            }

            Stop();
        }
    }
}