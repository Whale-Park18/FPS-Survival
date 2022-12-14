using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.MemoryPool;

public class ProjectileMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject projectilePrefab;

    private MemoryPool memoryPool;

    private void Awake()
    {
        memoryPool = new MemoryPool(projectilePrefab);
    }

    public void SpawnProjectile()
    {

    }
}
