using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WhalePark18.Character.Enemy;

namespace WhalePark18.ObjectPool
{
    public class EnemySpawnTileObjectPool : MonoObjectPool<EnemySpawnPoint>
    {
        /// <summary>
        /// ������Ʈ ��ȯ �������̽�
        /// </summary>
        /// <returns>������Ʈ</returns>
        /// <remarks>
        /// Vector3(0, 0, 0) ��ġ ������Ʈ
        /// </remarks>
        public override EnemySpawnPoint GetObject()
        {
            return GetObject(Vector3.zero);
        }

        /// <summary>
        /// ������Ʈ ��ȯ �������̽�
        /// </summary>
        /// <param name="position">������ ��ġ</param>
        /// <returns>������Ʈ</returns>
        /// <remarks>
        /// position ��ġ ������Ʈ
        /// </remarks>
        public EnemySpawnPoint GetObject(Vector3 position)
        {
            if (objectPool.Count == 0)
                CreateObjects();

            EnemySpawnPoint enemySpawnPoint = objectPool.Dequeue();
            EnableObject(enemySpawnPoint, position);

            return enemySpawnPoint;
        }
    }
}