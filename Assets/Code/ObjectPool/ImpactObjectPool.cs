using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.ObjectPool
{
    public class ImpactObjectPool : MonoObjectPool<Impact>
    {
        /// <summary>
        /// ������Ʈ ��ȯ �������̽�
        /// </summary>
        /// <returns>������Ʈ</returns>
        /// <remarks>
        /// Vector3(0, 0, 0) ��ġ, Euler(0, 0, 0) ȸ��, �� ���� X ������Ʈ
        /// </remarks>
        public override Impact GetObject()
        {
            return GetObject(Vector3.zero, Quaternion.Euler(0, 0, 0));
        }

        /// <summary>
        /// ������Ʈ ��ȯ �������̽�
        /// </summary>
        /// <param name="position">������ ��ġ</param>
        /// <param name="rotation">������ ȸ��</param>
        /// <returns>������Ʈ</returns>
        /// <remarks>
        /// position ��ġ, rotation ȸ��, �� ���� X ������Ʈ
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
        /// ������Ʈ ��ȯ �������̽�
        /// </summary>
        /// <param name="position">������ ��ġ</param>
        /// <param name="rotation">������ ȸ��</param>
        /// <param name="color">������ ��</param>
        /// <returns>������Ʈ</returns>
        /// <remarks>
        /// position ��ġ, rotation ȸ��, color �� ������Ʈ
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