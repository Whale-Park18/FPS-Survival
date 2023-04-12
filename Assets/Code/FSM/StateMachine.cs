using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WhalePark18.Manager;
using WhalePark18.FSM.State;

namespace WhalePark18.FSM
{
    public class StateMachine<T> where T : MonoBehaviour
    {
        private T owner;
        private StateBase<T> currentState;
        private StateBase<T> previousState;

        public StateBase<T> CurrentState => currentState;
        public StateBase<T> PreviousState => previousState;

        public void Setup(T owner, StateBase<T> entryState)
        {
            Setup(owner);

            ChangeState(entryState);
        }

        public void Setup(T owner)
        {
            this.owner = owner;
            currentState = null;
        }

        public void Execute()
        {
            if(currentState != null)
            {
                currentState.Execute(owner);
            }
        }

        public void ChangeState(StateBase<T> newState)
        {
            /// newState가 비어있거나 currentState와 newState가 같다면 종료한다.
            if (newState == null || currentState == newState)
            {
                if(newState == null)
                    LogManager.ConsoleErrorLog("StateMachine", $"{typeof(StateBase<T>)} is null");

                if(currentState == newState)
                    LogManager.ConsoleDebugLog("StateMachine", $"currentState == newState ({newState.GetType().Name})");

                return;
            }

            /// 진행 중인 상태가 있다면 해당 상태를 종료한다.
            if(currentState != null)
            {
                currentState.Exit(owner);
            }

            /// 현재 상태를 newState로 변경한 후, 진입힌다.
            currentState = newState;
            currentState.Enter(owner);
        }
    }
}