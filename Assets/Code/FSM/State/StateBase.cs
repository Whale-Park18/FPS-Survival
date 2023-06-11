using UnityEngine;

namespace WhalePark18.FSM.State
{
    public abstract class StateBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected Coroutine coroutineHandle;

        /// <summary>
        /// 상태에 진입할 때, 처음 호출되는 메소드
        /// </summary>
        /// <param name="owner">상태를 가진 Enemy</param>
        public abstract void Enter(T owner);

        /// <summary>
        /// 상태를 실행할 때, 호출되는 메소드
        /// </summary>
        /// <param name="owner">상태를 가진 Enemy</param>
        public abstract void Execute(T owner);

        /// <summary>
        /// 상태를 나갈 때, 호출되는 메소드
        /// </summary>
        /// <param name="owner">상태를 가진 Enemy</param>
        public abstract void Exit(T owner);
    }
}