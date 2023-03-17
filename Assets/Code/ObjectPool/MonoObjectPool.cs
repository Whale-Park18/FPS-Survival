using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.ObjectPool
{
    /// <summary>
    /// ���׸� ������ƮǮ Ŭ����
    /// </summary>
    /// <typeparam name="T">MonoBehaviour</typeparam>
    public abstract class MonoObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField, Tooltip("������ ������")]
        protected GameObject prefab;
        protected Queue<T> objectPool;

        [SerializeField,Tooltip("���� ����")]
        protected int increase = 5;
        protected int maxObjectCount;
        protected int activeObjectCount;

        protected void Awake()
        {
            OnInitialized();
        }

        /// <summary>
        /// �ʱ�ȭ �޼ҵ�
        /// </summary>
        protected void OnInitialized()
        {
            objectPool = new Queue<T>();
            CreateObjects();
        }

        /// <summary>
        /// ������Ʈ ���� �޼ҵ�
        /// </summary>
        protected void CreateObjects()
        {
            /// �ִ� ������Ʈ ���� ����
            maxObjectCount += increase;

            /// increase ��ŭ ������Ʈ ����
            for (int i = 0; i < increase; i++)
            {
                GameObject instacePrefab = GameObject.Instantiate(prefab);
                T newObject = instacePrefab.GetComponent<T>();

                DisableObject(newObject);

                objectPool.Enqueue(newObject);
            }
        }

        /// <summary>
        /// ������Ʈ ȹ�� �������̽�
        /// </summary>
        /// <returns>T Ÿ�� ������Ʈ</returns>
        public abstract T GetObject();

        /// <summary>
        /// ������Ʈ ��ȯ �������̽�
        /// </summary>
        /// <param name="returnObject">��ȯ�� T Ÿ�� ������Ʈ</param>
        public void ReturnObject(T returnObject)
        {
            /// ��ȯ�� ������Ʈ�� ��Ȱ��ȭ �� ��, ������ƮǮ�� �ִ´�.
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
            EnableObject(enableObject);
            enableObject.transform.position = position;
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