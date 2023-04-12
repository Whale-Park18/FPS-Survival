using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using WhalePark18.Manager;
using static UnityEngine.UI.GridLayoutGroup;
using WhalePark18.Projectile;

public class Test : MonoBehaviour
{
    public Transform projectileSpawnPoint;
    public Transform target;
    private WaitForSeconds attackDelay = new WaitForSeconds(0.15f);

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Fire()
    {
        StartCoroutine(OnFire());
    }

    private IEnumerator OnFire()
    {
        ProjectileBase projectile = EnemyProjectileManager.Instance.GetEnemyNormalMissile(
            projectileSpawnPoint.position, projectileSpawnPoint.rotation
        );
        yield return attackDelay;
        projectile.Fire(target.position);
    }
}
