using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.ObjectPool
{
    public class ImpactObjectPool : MonoObjectPool<Impact>
    {
        /// <summary>
        /// 오브젝트 반환 인터페이스
        /// </summary>
        /// <returns>오브젝트</returns>
        /// <remarks>
        /// Vector3(0, 0, 0) 위치, Euler(0, 0, 0) 회전, 색 변경 X 오브젝트
        /// </remarks>
        public override Impact GetObject()
        {
            return GetObject(Vector3.zero, Quaternion.Euler(0, 0, 0));
        }

        /// <summary>
        /// 오브젝트 반환 인터페이스
        /// </summary>
        /// <param name="position">지정할 위치</param>
        /// <param name="rotation">지정할 회전</param>
        /// <returns>오브젝트</returns>
        /// <remarks>
        /// position 위치, rotation 회전, 색 변경 X 오브젝트
        /// </remarks>
        public Impact GetObject(Vector3 position, Quaternion rotation)
        {
            if (objectPool.Count == 0)
                CreateObjects();

            Impact impact = objectPool.Dequeue();
            EnableObject(impact, position, rotation);

            return impact;
        }

        /// <summary>
        /// 오브젝트 반환 인터페이스
        /// </summary>
        /// <param name="position">지정할 위치</param>
        /// <param name="rotation">지정할 회전</param>
        /// <param name="color">지정할 색</param>
        /// <returns>오브젝트</returns>
        /// <remarks>
        /// position 위치, rotation 회전, color 색 오브젝트
        /// </remarks>
        public Impact GetObject(Vector3 position, Quaternion rotation, Color color)
        {
            Impact impact = GetObject(position, rotation);

            ParticleSystem.MainModule main = impact.GetComponent<ParticleSystem>().main;
            main.startColor = color;

            return impact;
        }
    }
}