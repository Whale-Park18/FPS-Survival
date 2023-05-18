using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhalePark18.ObjectPool
{
    /// <summary>
    /// 제네릭 오브젝트풀 클래스
    /// </summary>
    /// <typeparam name="T">MonoBehaviour</typeparam>
    public abstract class MonoObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        /****************************************
         * ObjectPool
         ****************************************/
        [SerializeField, Tooltip("관리할 프리팹")]
        protected GameObject    prefab;
        protected Queue<T>      objectPool;
        [SerializeField,Tooltip("증가 갯수")]
        protected int           increase = 5;

        /****************************************
         * 관리용
         ****************************************/
        [SerializeField]
        protected bool                  isTrackActiveObject;    // 오브젝트를 추적할 것인지
        protected int                   idToAssignToTheObject;  // 오브젝트에 부여할 ID
        protected Dictionary<int, T>    activeObjectDictionary; // 활성화된 오브젝트
        //protected int maxObjectCount;
        //protected int activeObjectCount;

        protected void Awake()
        {
            objectPool = new Queue<T>();
            activeObjectDictionary = new Dictionary<int, T>();

            CreateObjects();
        }

        /// <summary>
        /// 오브젝트를 increase만큼 생성 메소드
        /// </summary>
        protected void CreateObjects()
        {
            /// increase 만큼 오브젝트 생성
            for (int i = 0; i < increase; i++)
            {
                T newObject = CreateObject();

                DisableObject(newObject);

                objectPool.Enqueue(newObject);
            }
        }

        /// <summary>
        /// 오브젝트를 생성하는 메소드
        /// </summary>
        /// <returns>T 타입 오브젝트</returns>
        /// <remarks>
        /// 활성화 오브젝트 관리 여부에 따라 오브젝트 생성 방식이 다르기 때문에
        /// 오브젝트를 생성하는 메소드를 추상 메소드로 선언한다.
        /// </remarks>
        protected abstract T CreateObject();

        /// <summary>
        /// 오브젝트 획득 인터페이스
        /// </summary>
        /// <returns>T 타입 오브젝트</returns>
        public abstract T GetObject();

        /// <summary>
        /// 오브젝트 반환 인터페이스
        /// </summary>
        /// <param name="returnObject">반환할 T 타입 오브젝트</param>
        /// <remarks>
        /// 반환된 오브젝트를 비활성화 한 후, 오브젝트풀에 넣는다.
        /// trackActiveObject 활성화 되었을 때, 필요한 과정은 오버라이딩으로 작성한다.
        /// </remarks>
        public virtual void ReturnObject(T returnObject)
        {
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
            enableObject.transform.position = position;
            EnableObject(enableObject);
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