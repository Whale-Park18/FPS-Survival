using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;
using WhalePark18.Manager;

namespace WhalePark18.Character.Enemy
{
    public enum EnemyNavMeshAgentStates { Move, Stop }
    public enum BreakMode { SmoothStop, SuddenStop }

    public class EnemyNavMeshAgentController : MonoBehaviour
    {
        private NavMeshAgent navMeshAgent;

        /****************************************
         * 프로퍼티
         ****************************************/
        public float speed
        {
            get => navMeshAgent.speed;
            set => navMeshAgent.speed = value;
        }

        public Vector3 destination => navMeshAgent.destination;

        public float stoppingDistance
        {
            set => navMeshAgent.stoppingDistance = value;
            get => navMeshAgent.stoppingDistance;
        }

        public bool NavMeshAgentEnabled
        {
            set => navMeshAgent.enabled = value;
            get => navMeshAgent.enabled;
        }

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// target 위치로 이동시키는 메소드
        /// </summary>
        /// <param name="target">목적지</param>
        public void SetDestination(Vector3 target)
        {
            navMeshAgent.SetDestination(target);
        }

        /// <summary>
        /// 경로 초기화 메소드
        /// </summary>
        public void ResetPath()
        {
            navMeshAgent.ResetPath();
        }

        /// <summary>
        /// NavMeshAgent의 상태를 변경하는 메소드
        /// </summary>
        /// <param name="newState">변경할 상태</param>
        /// <param name="breakMode">"Stop" 상태일 때, 정지 방법</param>
        public void ChangeState(EnemyNavMeshAgentStates newState, BreakMode breakMode = BreakMode.SuddenStop)
        {
            if (newState.Equals(EnemyNavMeshAgentStates.Move))
            {
                navMeshAgent.isStopped = false;
            }
            else if(newState.Equals(EnemyNavMeshAgentStates.Stop))
            {
                navMeshAgent.isStopped = true;

                if(breakMode.Equals(BreakMode.SuddenStop)) navMeshAgent.velocity = Vector3.zero;
            }
        }

        /// <summary>
        /// 에이전트가 도착 여부를 반환하는 메소드
        /// </summary>
        /// <returns>도착 여부</returns>
        public bool IsArrived()
        {
            /// 에이전트가 이동 명령을 받은 후, 경로를 계산 중이라면 pathPending이 활성화 되며,
            /// 목적지까지 이동 중이라면 hasPath이 활성화 된다. 목적지에 도착하면 비활성화 된다.
            /// 이를 이용해 경로 계산 중이거나 이동 중임을 확인할 수 있다.
            /// hasPath과 pathPending이 모두 비활성화 됐을 때, 남은 거리가 정지 거리 이하라면 도착했음을 의미한다.
            if (navMeshAgent.hasPath == false && navMeshAgent.pathPending == false)
            {
                if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    return true;
                }
            }

            return false;
        }
    }
}