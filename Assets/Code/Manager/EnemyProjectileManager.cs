using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.ObjectPool;
using WhalePark18.Projectile;

namespace WhalePark18.Manager
{
    public enum EnemyProjectileType { NormalMissile = 0, BossMissile, BossRock }

    public class EnemyProjectileManager : MonoSingleton<EnemyProjectileManager>
    {
        [SerializeField]
        private ProjectileObjectPool[] projectileObjectPools;

        public ProjectileBase GetEnemyNormalMissile(Vector3 position, Quaternion rotation)
        {
            return projectileObjectPools[(int)EnemyProjectileType.NormalMissile].GetObject(position, rotation);
        }

        public void ReturnEnemyNormalMissile(ProjectileBase projectile)
        {
            projectileObjectPools[(int)EnemyProjectileType.NormalMissile].ReturnObject(projectile);
        }
    }
}