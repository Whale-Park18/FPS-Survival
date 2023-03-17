using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace WhalePark18
{
    /// <summary>
    /// 템플릿을 이용한 싱글톤
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <see herf="https://icat2048.tistory.com/426">참조1</see>
    /// <see href="http://www.unitygeek.com/unity_c_singleton/">참조2</see>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;                 // 싱글톤 인스턴스
        private static object lockObject = new object();  // 공유 데이터 동기화에 사용되는 lock 오브젝트
        private static bool applicationQuit = false;      // 응용 프로그램 종료 상태

        public static T Instance
        {
            get
            {
                /// 중복 생성되어 메모리 누수가 발생하는 것을 방지한다.
                if (applicationQuit)
                    return null;

                /// lock을 이용해 공유 데이터인 instance의 단독 접근을 보장한다.
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
                /// 씬에 T 컴포넌트를 보유한 객체가 있는지 확인한다.
                instance = FindObjectOfType<T>();

                /// 씬에 T 객체가 없다면
                if (instance == null)
                {
                    /// T 컴포넌트의 이름으로 게임 오브젝트를 생성하고
                    /// T 컴포넌트를 추가해 instace에 대입한다.
                    string componentName = typeof(T).Name;
                    GameObject newObject = new GameObject(componentName);
                    instance = newObject.AddComponent<T>();

                    /// 씬이 변경되어도 게임 오브젝트가 유지되도록 한다.
                    DontDestroyOnLoad(newObject);
                }
            }
        }

        /// 싱글톤 오브젝트가 파괴된 후, 싱글톤 오브젝트가 호출된다면
        /// 앱의 재생이 정지된 이후에도 에디터 씬에서 고스트 객체가 생성되기 때문에
        /// 고스트 객체의 생성을 방지하기 위해 상태를 관리한다.

        /// <summary>
        /// 애플리케이션이 종료되기 전에 모든 게임 오브젝트에 호출되는 메소드
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            applicationQuit = true;
        }

        /// <summary>
        /// Monohaviour가 삭제될 때 호출되는 메소드
        /// </summary>
        protected virtual void OnDestroy()
        {
            applicationQuit = true;
        }
    }
}