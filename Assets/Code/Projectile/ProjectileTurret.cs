using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.Character.Player;
using WhalePark18.Manager;
using WhalePark18.Objects;

namespace WhalePark18.Projectile
{
    public class ProjectileTurret : ProjectileBase
    {
        protected override void Stop()
        {
            Destroy(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.ImpactEnemy.ToString()))
            {
                other.GetComponent<PlayerController>().TakeDamage(damage);
            }
            else if (other.CompareTag(Tag.InteractionObejct.ToString()))
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