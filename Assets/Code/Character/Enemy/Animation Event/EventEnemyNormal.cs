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

        //public void Setup(EnemyNormal owner, float animSpeed)
        //{
        //    base.Setup(owner);
        //}

        public void Reload()
        {
            owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Stop);

            projectile = EnemyProjectileManager.Instance.GetEnemyNormalMissile(
                    owner.ProjectileSpawnPoint.position, owner.ProjectileSpawnPoint.rotation
            );
        }

        public void Fire()
        {
            projectile.Fire(owner.Target.position);
            projectile = null;
        }

        public void Exit()
        {
            //LogManager.ConsoleDebugLog("EventEnemyNormal", $"Exit");
            //owner.AnimatorController.SetBool(EnemyNormalAnimParam.isAttack.ToString(), false);
            //LogManager.ConsoleDebugLog("EventEnemyNormal", $"isAttack: {owner.AnimatorController.GetBool(EnemyNormalAnimParam.isAttack.ToString())}");
            //owner.NavMeshAgentController.Move();
        }
    }
}