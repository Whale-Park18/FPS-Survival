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
            /// newState�� ����ְų� currentState�� newState�� ���ٸ� �����Ѵ�.
            if (newState == null || currentState == newState)
            {
                if(newState == null)
                    LogManager.ConsoleErrorLog("StateMachine", $"{typeof(StateBase<T>)} is null");

                if(currentState == newState)
                    LogManager.ConsoleDebugLog("StateMachine", $"currentState == newState ({newState.GetType().Name})");

                return;
            }

            /// ���� ���� ���°� �ִٸ� �ش� ���¸� �����Ѵ�.
            if(currentState != null)
            {
                currentState.Exit(owner);
            }

            /// ���� ���¸� newState�� ������ ��, ��������.
            currentState = newState;
            currentState.Enter(owner);
        }
    }
}