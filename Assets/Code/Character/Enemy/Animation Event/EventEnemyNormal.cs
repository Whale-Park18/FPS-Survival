using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WhalePark18.Manager;
using WhalePark18.Projectile;

namespace WhalePark18.Character.Enemy.Event.EventEnemyNormal
{
    public class EventAttack : EventBase<EnemyNormal>
    {
        private ProjectileBase projectile;

        public override void Setup(EnemyNormal owner)
        {
            this.owner = owner;
        }

        public void Reload()
        {
            projectile = EnemyProjectileManager.Instance.GetEnemyNormalMissile(
                    owner.ProjectileSpawnPoint.position, owner.ProjectileSpawnPoint.rotation
            );
        }

        public void Fire()
        {
            projectile.Fire(owner.Target.position);
            projectile = null;
        }
    }
}