using UnityEngine;

namespace WhalePark18.FSM.State
{
    public abstract class StateBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected Coroutine coroutineHandle;

        /// <summary>
        /// ���¿� ������ ��, ó�� ȣ��Ǵ� �޼ҵ�
        /// </summary>
        /// <param name="owner">���¸� ���� Enemy</param>
        public abstract void Enter(T owner);

        /// <summary>
        /// ���¸� ������ ��, ȣ��Ǵ� �޼ҵ�
        /// </summary>
        /// <param name="owner">���¸� ���� Enemy</param>
        public abstract void Execute(T owner);

        /// <summary>
        /// ���¸� ���� ��, ȣ��Ǵ� �޼ҵ�
        /// </summary>
        /// <param name="owner">���¸� ���� Enemy</param>
        public abstract void Exit(T owner);
    }
}