using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.ObjectPool
{
    public class CasingObjectPool : MonoObjectPool<Casing>
    {
        /// <summary>
        /// ������Ʈ ��ȯ �������̽�
        /// </summary>
        /// <returns>������Ʈ</returns>
        /// <remarks>
        /// Vector3(0, 0, 0) ��ġ, Eunler(0, 0, 0) ȸ�� ������Ʈ
        /// </remarks>
        public override Casing GetObject()
        {
            return GetObject(Vector3.zero, Quaternion.Euler(0, 0, 0));
        }

        /// <summary>
        /// ������Ʈ ��ȯ �������̽�
        /// </summary>
        /// <param name="position">������ ��ġ</param>
        /// <param name="rotation">������ ȸ��</param>
        /// <returns>������Ʈ</returns>
        /// /// <remarks>
        /// position ��ġ, rotation ȸ�� ������Ʈ
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