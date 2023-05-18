using UnityEngine;
using WhalePark18.Character.Enemy;
using WhalePark18.Manager;
using WhalePark18.Objects;

namespace WhalePark18.Projectile
{
    public class ProjectileTurret : ProjectileBase
    {
        public override void Stop()
        {
            base.Stop();
            Destroy(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            LogManager.ConsoleDebugLog("ProjectileTurret", $"other.tag: {other.tag}");

            if (other.CompareTag("ImpactEnemy"))
            {
                other.GetComponent<EnemyBase>().TakeDamage(damage);
            }
            else if (other.CompareTag("InteractionObject"))
            {
                var interactionObject = other.GetComponent<InteractionObject>();
                if (interactionObject != null)
                {
                    interactionObject.TakeDamage(damage);
                }
            }

            Stop();
        }
    }
}