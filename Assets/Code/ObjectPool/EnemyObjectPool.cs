using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WhalePark18.Character.Enemy;

namespace WhalePark18.ObjectPool
{
    public class EnemyObjectPool : MonoObjectPool<EnemyFSM>
    {
        /// <summary>
        /// 오브젝트 반환 인터페이스
        /// </summary>
        /// <returns>오브젝트</returns>
        /// <remarks>
        /// Vector3(0, 0, 0) 위치 오브젝트
        /// </remarks>
        public override EnemyFSM GetObject()
        {
            return GetObject(Vector3.zero);
        }

        /// <summary>
        /// 오브젝트 반환 인터페이스
        /// </summary>
        /// <param name="position">지정할 위치</param>
        /// <returns>오브젝트</returns>
        /// <remarks>
        /// position 위치 오브젝트
        /// </remarks>
        public EnemyFSM GetObject(Vector3 position)
        {
            if (objectPool.Count == 0)
                CreateObjects();

            EnemyFSM enemyFSM = objectPool.Dequeue();
            EnableObject(enemyFSM, position);

            return enemyFSM;
        }
    }
}