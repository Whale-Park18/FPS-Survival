using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace WhalePark18
{
    /// <summary>
    /// ���ø��� �̿��� �̱���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <see herf="https://icat2048.tistory.com/426">����1</see>
    /// <see href="http://www.unitygeek.com/unity_c_singleton/">����2</see>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;                 // �̱��� �ν��Ͻ�
        private static object lockObject = new object();  // ���� ������ ����ȭ�� ���Ǵ� lock ������Ʈ
        private static bool applicationQuit = false;      // ���� ���α׷� ���� ����

        public static T Instance
        {
            get
            {
                /// �ߺ� �����Ǿ� �޸� ������ �߻��ϴ� ���� �����Ѵ�.
                if (applicationQuit)
                    return null;

                /// lock�� �̿��� ���� �������� instance�� �ܵ� ������ �����Ѵ�.
                lock (lockObject)
                {
                    CreateInstance();
                }

                return instance;
            }
        }

        protected static void CreateInstance()
        {
            if (instance == null)
            {
                /// ���� T ������Ʈ�� ������ ��ü�� �ִ��� Ȯ���Ѵ�.
                instance = FindObjectOfType<T>();

                /// ���� T ��ü�� ���ٸ�
                if (instance == null)
                {
                    /// T ������Ʈ�� �̸����� ���� ������Ʈ�� �����ϰ�
                    /// T ������Ʈ�� �߰��� instace�� �����Ѵ�.
                    string componentName = typeof(T).Name;
                    GameObject newObject = new GameObject(componentName);
                    instance = newObject.AddComponent<T>();

                    /// ���� ����Ǿ ���� ������Ʈ�� �����ǵ��� �Ѵ�.
                    DontDestroyOnLoad(newObject);
                }
            }
        }

        /// �̱��� ������Ʈ�� �ı��� ��, �̱��� ������Ʈ�� ȣ��ȴٸ�
        /// ���� ����� ������ ���Ŀ��� ������ ������ ��Ʈ ��ü�� �����Ǳ� ������
        /// ��Ʈ ��ü�� ������ �����ϱ� ���� ���¸� �����Ѵ�.

        /// <summary>
        /// ���ø����̼��� ����Ǳ� ���� ��� ���� ������Ʈ�� ȣ��Ǵ� �޼ҵ�
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            applicationQuit = true;
        }

        /// <summary>
        /// Monohaviour�� ������ �� ȣ��Ǵ� �޼ҵ�
        /// </summary>
        protected virtual void OnDestroy()
        {
            applicationQuit = true;
        }
    }
}