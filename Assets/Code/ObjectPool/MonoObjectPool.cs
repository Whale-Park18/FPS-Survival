using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhalePark18.ObjectPool
{
    /// <summary>
    /// ���׸� ������ƮǮ Ŭ����
    /// </summary>
    /// <typeparam name="T">MonoBehaviour</typeparam>
    public abstract class MonoObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        /****************************************
         * ObjectPool
         ****************************************/
        [SerializeField, Tooltip("������ ������")]
        protected GameObject    prefab;
        protected Queue<T>      objectPool;
        [SerializeField,Tooltip("���� ����")]
        protected int           increase = 5;

        /****************************************
         * ������
         ****************************************/
        [SerializeField]
        protected bool                  isTrackActiveObject;    // ������Ʈ�� ������ ������
        protected int                   idToAssignToTheObject;  // ������Ʈ�� �ο��� ID
        protected Dictionary<int, T>    activeObjectDictionary; // Ȱ��ȭ�� ������Ʈ
        //protected int maxObjectCount;
        //protected int activeObjectCount;

        protected void Awake()
        {
            objectPool = new Queue<T>();
            activeObjectDictionary = new Dictionary<int, T>();

            CreateObjects();
        }

        /// <summary>
        /// ������Ʈ�� increase��ŭ ���� �޼ҵ�
        /// </summary>
        protected void CreateObjects()
        {
            /// increase ��ŭ ������Ʈ ����
            for (int i = 0; i < increase; i++)
            {
                T newObject = CreateObject();

                DisableObject(newObject);

                objectPool.Enqueue(newObject);
            }
        }

        /// <summary>
        /// ������Ʈ�� �����ϴ� �޼ҵ�
        /// </summary>
        /// <returns>T Ÿ�� ������Ʈ</returns>
        /// <remarks>
        /// Ȱ��ȭ ������Ʈ ���� ���ο� ���� ������Ʈ ���� ����� �ٸ��� ������
        /// ������Ʈ�� �����ϴ� �޼ҵ带 �߻� �޼ҵ�� �����Ѵ�.
        /// </remarks>
        protected abstract T CreateObject();

        /// <summary>
        /// ������Ʈ ȹ�� �������̽�
        /// </summary>
        /// <returns>T Ÿ�� ������Ʈ</returns>
        public abstract T GetObject();

        /// <summary>
        /// ������Ʈ ��ȯ �������̽�
        /// </summary>
        /// <param name="returnObject">��ȯ�� T Ÿ�� ������Ʈ</param>
        /// <remarks>
        /// ��ȯ�� ������Ʈ�� ��Ȱ��ȭ �� ��, ������ƮǮ�� �ִ´�.
        /// trackActiveObject Ȱ��ȭ �Ǿ��� ��, �ʿ��� ������ �������̵����� �ۼ��Ѵ�.
        /// </remarks>
        public virtual void ReturnObject(T returnObject)
        {
            DisableObject(returnObject);
            objectPool.Enqueue(returnObject);
        }

        /// <summary>
        /// ������Ʈ Ȱ��ȭ �޼ҵ�
        /// </summary>
        /// <param name="enableObject">Ȱ��ȭ�� ������Ʈ</param>
        protected void EnableObject(T enableObject)
        {
            enableObject.transform.SetParent(null);
            enableObject.gameObject.SetActive(true);
        }

        /// <summary>
        /// ������Ʈ Ȱ��ȭ �޼ҵ�
        /// </summary>
        /// <param name="enableObject">Ȱ��ȭ�� ������Ʈ</param>
        /// <param name="position">Ȱ��ȭ�� ������Ʈ�� ��ġ</param>
        protected void EnableObject(T enableObject, Vector3 position)
        {
            enableObject.transform.position = position;
            EnableObject(enableObject);
        }

        /// <summary>
        /// ������Ʈ Ȱ��ȭ �޼ҵ�
        /// </summary>
        /// <param name="enableObject">Ȱ��ȭ�� ������Ʈ</param>
        /// <param name="position">Ȱ��ȭ�� ������Ʈ�� ��ġ</param>
        /// <param name="rotation">Ȱ��ȭ�� ������Ʈ�� ȸ����</param>
        protected void EnableObject(T enableObject, Vector3 position, Quaternion rotation)
        {
            EnableObject(enableObject, position);
            enableObject.transform.rotation = rotation;
        }

        /// <summary>
        /// ������Ʈ ��Ȱ��ȭ �޼ҵ�
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