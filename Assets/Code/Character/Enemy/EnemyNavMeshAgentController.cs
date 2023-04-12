using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

namespace WhalePark18.Character.Enemy
{
    public enum BreakMode { SmoothStop, SuddenStop }

    public class EnemyNavMeshAgentController : MonoBehaviour
    {
        private NavMeshAgent navMeshAgent;

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
        /// �̵� ���� �ٲٴ� �޼ҵ�
        /// </summary>
        public void MoveMode()
        {
            navMeshAgent.isStopped = false;
            //navMeshAgent.updatePosition = true;
            //navMeshAgent.updateRotation = true;
        }

        /// <summary>
        /// ���� ���� �ٲٴ� �޼ҵ�
        /// </summary>
        /// <param name="breakMode">���� ���</param>
        public void StopMode(BreakMode breakMode)
        {
            navMeshAgent.isStopped = true;
            //navMeshAgent.updatePosition = false;
            //navMeshAgent.updateRotation = false;

            if(breakMode == BreakMode.SuddenStop) navMeshAgent.velocity = Vector3.zero;
        }
    }
}