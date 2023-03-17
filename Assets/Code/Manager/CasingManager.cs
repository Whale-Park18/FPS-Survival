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
        /// 탄피 생성 인터페이스
        /// </summary>
        /// <param name="position">탄피 생성 위치</param>
        /// <param name="direction">탄피</param>
        public void SpawnCasing(Vector3 position, Vector3 direction)
        {
            Casing casing = casingObjectPool.GetObject(position, Random.rotation);
            casing.Setup(direction);
        }

        /// <summary>
        /// 탄피 반환 인터페이스
        /// </summary>
        /// <param name="returnCasing">반환할 탄피</param>
        public void ReturnCasing(Casing returnCasing)
        {
            casingObjectPool.ReturnObject(returnCasing);
        }
    }
}