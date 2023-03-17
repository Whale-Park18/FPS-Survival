using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WhalePark18.ObjectPool;

namespace WhalePark18.Manager
{

    public class CasingManager : MonoSingleton<CasingManager>
    {
        private CasingObjectPool casingObjectPool;

        private void Awake()
        {
            casingObjectPool = GetComponentInChildren<CasingObjectPool>();
        }

        /// <summary>
        /// ź�� ���� �������̽�
        /// </summary>
        /// <param name="position">ź�� ���� ��ġ</param>
        /// <param name="direction">ź��</param>
        public void SpawnCasing(Vector3 position, Vector3 direction)
        {
            Casing casing = casingObjectPool.GetObject(position, Random.rotation);
            casing.Setup(direction);
        }

        /// <summary>
        /// ź�� ��ȯ �������̽�
        /// </summary>
        /// <param name="returnCasing">��ȯ�� ź��</param>
        public void ReturnCasing(Casing returnCasing)
        {
            casingObjectPool.ReturnObject(returnCasing);
        }
    }
}