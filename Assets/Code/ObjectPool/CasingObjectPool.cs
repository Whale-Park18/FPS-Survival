using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.ObjectPool
{
    public class CasingObjectPool : MonoObjectPool<Casing>
    {
        /// <summary>
        /// 오브젝트 반환 인터페이스
        /// </summary>
        /// <returns>오브젝트</returns>
        /// <remarks>
        /// Vector3(0, 0, 0) 위치, Eunler(0, 0, 0) 회전 오브젝트
        /// </remarks>
        public override Casing GetObject()
        {
            return GetObject(Vector3.zero, Quaternion.Euler(0, 0, 0));
        }

        /// <summary>
        /// 오브젝트 반환 인터페이스
        /// </summary>
        /// <param name="position">지정할 위치</param>
        /// <param name="rotation">지정할 회전</param>
        /// <returns>오브젝트</returns>
        /// /// <remarks>
        /// position 위치, rotation 회전 오브젝트
        /// </remarks>
        public Casing GetObject(Vector3 position, Quaternion rotation)
        {
            if (objectPool.Count == 0)
                CreateObjects();

            Casing casing = objectPool.Dequeue();
            EnableObject(casing, position, rotation);

            return casing;
        }
    }
}