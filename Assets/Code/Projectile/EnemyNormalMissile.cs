using UnityEngine;
using WhalePark18.Character.Player;
using WhalePark18.Manager;
using WhalePark18.Objects;

namespace WhalePark18.Projectile
{
    public class EnemyNormalMissile : ProjectileBase
    {
        private ParticleSystem boost;

        private static float lastAttckTime;

        private void Awake()
        {
            boost = GetComponentInChildren<ParticleSystem>();
        }

        public override void Fire(Vector3 targetPosition)
        {
            base.Fire(targetPosition);
            boost.Play();
        }

        public override void Stop()
        {
            base.Stop();
            boost.Stop();
            EnemyProjectileManager.Instance.ReturnEnemyNormalMissile(this);
        }

        ///
        protected virtual void OnTriggerEnter(Collider other)
        {
            LogManager.ConsoleDebugLog("EnemyNormalMissile", $"피격 객체: {other.gameObject.name}");
            LogManager.ConsoleDebugLog(gameObject.name, $"피격 시간: {Time.time}, 이전 피격 시간과의 차이: {Time.time - lastAttckTime}");
            lastAttckTime = Time.time;

            if (other.CompareTag("Player") && other.name.Equals("PlayerCollider"))
            {
                var player = other.transform.parent.GetComponent<PlayerController>();
                if (player != null) player.TakeDamage(damage);
            }
            else if (other.CompareTag("InteractionObject"))
            {
                var interactionObject = other.GetComponent<InteractionObject>();
                if (interactionObject != null) interactionObject.TakeDamage(damage);
            }

            Stop();
        }
    }
}