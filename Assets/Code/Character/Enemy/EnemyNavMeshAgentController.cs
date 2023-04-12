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
        /// 이동 모드로 바꾸는 메소드
        /// </summary>
        public void MoveMode()
        {
            navMeshAgent.isStopped = false;
            //navMeshAgent.updatePosition = true;
            //navMeshAgent.updateRotation = true;
        }

        /// <summary>
        /// 정지 모드로 바꾸는 메소드
        /// </summary>
        /// <param name="breakMode">정지 모드</param>
        public void StopMode(BreakMode breakMode)
        {
            navMeshAgent.isStopped = true;
            //navMeshAgent.updatePosition = false;
            //navMeshAgent.updateRotation = false;

            if(breakMode == BreakMode.SuddenStop) navMeshAgent.velocity = Vector3.zero;
        }
    }
}