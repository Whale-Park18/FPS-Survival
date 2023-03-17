using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.ObjectPool
{
    /// <summary>
    /// 제네릭 오브젝트풀 클래스
    /// </summary>
    /// <typeparam name="T">MonoBehaviour</typeparam>
    public abstract class MonoObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField, Tooltip("관리할 프리팹")]
        protected GameObject prefab;
        protected Queue<T> objectPool;

        [SerializeField,Tooltip("증가 갯수")]
        protected int increase = 5;
        protected int maxObjectCount;
        protected int activeObjectCount;

        protected void Awake()
        {
            OnInitialized();
        }

        /// <summary>
        /// 초기화 메소드
        /// </summary>
        protected void OnInitialized()
        {
            objectPool = new Queue<T>();
            CreateObjects();
        }

        /// <summary>
        /// 오브젝트 생성 메소드
        /// </summary>
        protected void CreateObjects()
        {
            /// 최대 오브젝트 갯수 갱신
            maxObjectCount += increase;

            /// increase 만큼 오브젝트 생성
            for (int i = 0; i < increase; i++)
            {
                GameObject instacePrefab = GameObject.Instantiate(prefab);
                T newObject = instacePrefab.GetComponent<T>();

                DisableObject(newObject);

                objectPool.Enqueue(newObject);
            }
        }

        /// <summary>
        /// 오브젝트 획득 인터페이스
        /// </summary>
        /// <returns>T 타입 오브젝트</returns>
        public abstract T GetObject();

        /// <summary>
        /// 오브젝트 반환 인터페이스
        /// </summary>
        /// <param name="returnObject">반환할 T 타입 오브젝트</param>
        public void ReturnObject(T returnObject)
        {
            /// 반환된 오브젝트를 비활성화 한 후, 오브젝트풀에 넣는다.
            DisableObject(returnObject);
            objectPool.Enqueue(returnObject);
        }

        /// <summary>
        /// 오브젝트 활성화 메소드
        /// </summary>
        /// <param name="enableObject">활성화할 오브젝트</param>
        protected void EnableObject(T enableObject)
        {
            enableObject.transform.SetParent(null);
            enableObject.gameObject.SetActive(true);
        }

        /// <summary>
        /// 오브젝트 활성화 메소드
        /// </summary>
        /// <param name="enableObject">활성화할 오브젝트</param>
        /// <param name="position">활성화할 오브젝트의 위치</param>
        protected void EnableObject(T enableObject, Vector3 position)
        {
            EnableObject(enableObject);
            enableObject.transform.position = position;
        }

        /// <summary>
        /// 오브젝트 활성화 메소드
        /// </summary>
        /// <param name="enableObject">활성화할 오브젝트</param>
        /// <param name="position">활성화할 오브젝트의 위치</param>
        /// <param name="rotation">활성화할 오브젝트의 회전값</param>
        protected void EnableObject(T enableObject, Vector3 position, Quaternion rotation)
        {
            EnableObject(enableObject, position);
            enableObject.transform.rotation = rotation;
        }

        /// <summary>
        /// 오브젝트 비활성화 메소드
        /// </summary>
        /// <param name="disableObject"></param>
        protected void DisableObject(T disableObject)
        {
            disableObject.transform.SetParent(transform);
            disableObject.transform.localPosition = Vector3.zero;
            disableObject.gameObject.SetActive(false);
        }
    }
}