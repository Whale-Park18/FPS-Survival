using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WhalePark18.Character.Enemy;

namespace WhalePark18.ObjectPool
{
    /// <summary>
    /// 적 소환 알림 타일 오브젝트풀
    /// </summary>
    /// <remarks>
    /// 오브젝트 추적
    /// </remarks>
    public class EnemySpawnTileObjectPool : MonoObjectPool<EnemySpawnPoint>
    {
        protected override EnemySpawnPoint CreateObject()
        {
            GameObject instance = GameObject.Instantiate(prefab);
            EnemySpawnPoint spawnPoint = instance.GetComponent<EnemySpawnPoint>();
            spawnPoint.Setup(idToAssignToTheObject++);

            return spawnPoint;
        }

       public override EnemySpawnPoint GetObject()
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
        public EnemySpawnPoint GetObject(Vector3 position)
        {
            if (objectPool.Count == 0)
                CreateObjects();

            EnemySpawnPoint enemySpawnPoint = objectPool.Dequeue();
            EnableObject(enemySpawnPoint, position);

            if(isTrackActiveObject)
            {
                activeObjectDictionary.Add(enemySpawnPoint.ID, enemySpawnPoint);
            }

            return enemySpawnPoint;
        }

        public override void ReturnObject(EnemySpawnPoint returnObject)
        {
            /// 반환된 오브젝트를 비활성화 한 후, 오브젝트풀에 넣는다.
            base.ReturnObject(returnObject);

            /// trackActiveObject가 활성화 되었을 때, 활성화 오브젝트 추적 자료형에서 제거한다.
            if (isTrackActiveObject)
            {
                activeObjectDictionary.Remove(returnObject.ID);
            }
        }
    }
}