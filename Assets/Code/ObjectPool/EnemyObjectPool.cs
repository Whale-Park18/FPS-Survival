using UnityEngine;

using WhalePark18.Character.Enemy;
using WhalePark18.Manager;

namespace WhalePark18.ObjectPool
{
    /// <summary>
    /// Enemy 오브젝트풀
    /// </summary>
    /// <remarks>
    /// 오브젝트 추적
    /// </remarks>
    public class EnemyObjectPool : MonoObjectPool<EnemyBase>
    {
        private Transform target;

        public Transform Target { get; set; }

        protected override EnemyBase CreateObject()
        {
            GameObject instance = GameObject.Instantiate(prefab);
            EnemyBase enemy = instance.GetComponent<EnemyBase>();
            enemy.Setup(idToAssignToTheObject++, target);

            return enemy;
        }

        /// <summary>
        /// 오브젝트 반환 인터페이스
        /// </summary>
        /// <returns>오브젝트</returns>
        /// <remarks>
        /// Vector3(0, 0, 0) 위치 오브젝트
        /// </remarks>
        public override EnemyBase GetObject()
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
        public EnemyBase GetObject(Vector3 position)
        {
            /// 1. 오브젝트풀이 비었는 경우 오브젝트 생성
            if (objectPool.Count == 0)
                CreateObjects();

            /// 2. 오브젝트풀에서 오브젝트를 받고 활성화 시킨다.
            EnemyBase enemy = objectPool.Dequeue();
            EnableObject(enemy, position);

            /// 3. trackActiveObject가 활성화 되었을 때, 오브젝트를 활성화 오브젝트 자료형에 추가한다.
            if (isTrackActiveObject)
            {
                activeObjects.Add(enemy.ID, enemy);
            }

            return enemy;
        }

        public override void ReturnObject(EnemyBase returnObject)
        {
            /// 반환된 오브젝트를 비활성화 한 후, 오브젝트풀에 넣는다.
            base.ReturnObject(returnObject);

            /// trackActiveObject가 활성화 되었을 때, 활성화 오브젝트 추적 자료형에서 제거한다.
            if(isTrackActiveObject)
            {
                activeObjects.Remove(returnObject.ID);
            }
        }
    }
}