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
         * ������Ƽ
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
        /// target ��ġ�� �̵���Ű�� �޼ҵ�
        /// </summary>
        /// <param name="target">������</param>
        public void SetDestination(Vector3 target)
        {
            navMeshAgent.SetDestination(target);
        }

        /// <summary>
        /// ��� �ʱ�ȭ �޼ҵ�
        /// </summary>
        public void ResetPath()
        {
            navMeshAgent.ResetPath();
        }

        /// <summary>
        /// NavMeshAgent�� ���¸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="newState">������ ����</param>
        /// <param name="breakMode">"Stop" ������ ��, ���� ���</param>
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
        /// ������Ʈ�� ���� ���θ� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <returns>���� ����</returns>
        public bool IsArrived()
        {
            /// ������Ʈ�� �̵� ����� ���� ��, ��θ� ��� ���̶�� pathPending�� Ȱ��ȭ �Ǹ�,
            /// ���������� �̵� ���̶�� hasPath�� Ȱ��ȭ �ȴ�. �������� �����ϸ� ��Ȱ��ȭ �ȴ�.
            /// �̸� �̿��� ��� ��� ���̰ų� �̵� ������ Ȯ���� �� �ִ�.
            /// hasPath�� pathPending�� ��� ��Ȱ��ȭ ���� ��, ���� �Ÿ��� ���� �Ÿ� ���϶�� ���������� �ǹ��Ѵ�.
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