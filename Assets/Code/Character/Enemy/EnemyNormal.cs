using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

using WhalePark18.Manager;
using WhalePark18.FSM;
using WhalePark18.FSM.State;
using WhalePark18.FSM.State.EnemyNoramlState;
using WhalePark18.Character.Enemy.Event;

namespace WhalePark18.Character.Enemy
{
    public enum EnemyNormalStates { Idle = 0, Wander, Pursuit, Attack, }
    public enum EnemyNormalAnimParam { isMovement, isAttack, doDie, }

    public class EnemyNormal : EnemyBase
    {
        [Header("Idle")]
        [SerializeField]
        private float maxIdleTime = 5f;

        [Header("Wander")]

        [Header("Pursuit")]
        [SerializeField]
        private float targetRecognitionRange = 8f;          // 인식 범위(이 범위 안에 들어오면 "Pursuit" 상태로 변경
        [SerializeField]
        private float pursuitLimitRange = 10f;              // 추적 범위(이 범위 밖으로 나가면 "Wander" 상태로 변경

        [Header("Attack")]
        [SerializeField]
        private GameObject projectilePrefab;                // 발사체 프리팹
        [SerializeField]
        private Transform projectileSpawnPoint;             // 발사체 생성 위치
        [SerializeField]
        private float attackRange = 5f;                     // 공격 범위(이 범위 안에 들어오면 "Attack"상태로 변경)
        [SerializeField]
        private float attackRate = 1f;                      // 공격 속도


        //private Transform target;

        private StateBase<EnemyNormal>[] states;
        private StateMachine<EnemyNormal> stateMachine;
        private EnemyNormalStates currentState;
        private bool canChangeState = true;

        //private EventBase

        //public Transform Target => target;
        public float MaxIdleTime => maxIdleTime;
        public float TargetRecognitionRange => targetRecognitionRange;
        public float PursuitLimitRange => pursuitLimitRange;
        public float AttackRange => attackRange;
        public float AttackRate => attackRate;
        public GameObject ProjetilePrefab => projectilePrefab;
        public Transform ProjectileSpawnPoint => projectileSpawnPoint;
        public EnemyNormalStates CurrentState => currentState;
        //public bool CanChangeState { set; get; } = true;

        private void Awake()
        {
            /// 1. 컴포넌트 초기화
            status                  = GetComponent<EnemyStatus>();
            navMeshAgentController  = GetComponent<EnemyNavMeshAgentController>();
            animatorController      = GetComponent<EnemyAnimatorController>();

            /// 2. 상태 초기화
            /// 2.1. 상태 컴포넌트를 붙일 오브젝트 생성 및 초기화
            GameObject statesObject = new GameObject("States");
            statesObject.transform.parent = transform;
            statesObject.transform.localPosition = Vector3.zero;

            /// 2.2. 상태 컴포넌트 부착
            states = new StateBase<EnemyNormal>[4];
            states[(int)EnemyNormalStates.Idle]     = statesObject.AddComponent<Idle>();
            states[(int)EnemyNormalStates.Wander]   = statesObject.AddComponent<Wander>();
            states[(int)EnemyNormalStates.Pursuit]  = statesObject.AddComponent<Pursuit>();
            states[(int)EnemyNormalStates.Attack]   = statesObject.AddComponent<Attack>();

            /// 3. 애니메이션 이벤트 컴포넌트 부착
            //Transform meshObject = transform.GetChild(0).Find("Mesh Object");
            //LogManager.Instance.ConsoleDebugLog("EnemyNormal", $"meshObject: {meshObject.name}");
            //var eventAttack = meshObject.AddComponent<Event.EventEnemyNormal.EventAttack>();
            //eventAttack.SetUp(this);
        }

        private void Start()
        {
            // DEBUG
            //SetUp(0);
        }

        public override void Setup(int id, Transform target)
        {
            base.Setup(id, target);

            stateMachine = new StateMachine<EnemyNormal>();
            //stateMachine.SetUp(this, states[(int)EnemyNormalStates.Idle]);
            stateMachine.Setup(this);
            ChangeState(EnemyNormalStates.Idle);

            // DEBUG
            //target = GameObject.Find("Player").transform;
            //LogManager.Instance.ConsoleDebugLog("Enemy1.SetUp", $"target is {target}");
        }

        /// <summary>
        /// 상태를 변경하는 메소드
        /// </summary>
        /// <param name="newState">변경할 새로운 상태</param>
        public void ChangeState(EnemyNormalStates newState)
        {
            currentState = newState;
            stateMachine.ChangeState(states[(int)newState]);
        }

        /// <summary>
        /// 탐색할 위치를 계산하는 메소드
        /// </summary>
        /// <returns>탐색 위치</returns>
        public Vector3 CalculateWanderPosition()
        {
            float wanderRadius = 10f;   // 현재 위치를 원점으로 하는 원의 반지름
            int wanderJitter = 0;       // 선택된 각도 (wanderJitterMin ~ wanderJitterMax)
            int wanderJitterMin = 0;    // 최소 각도
            int wanderJitterMax = 360;  // 최대 각도

            /// 현재 적 캐릭터가 있는 월드의 중심 위치와 크기(구역을 벗어난 행동을 하지 않도록)
            Vector3 rangePosition = Vector3.zero;
            Vector3 rangeScale = Vector3.one * 100f;

            /// 자신의 위치를 중심으로 반지금(wanderRadius) 거리,
            /// 선택된 각도(wanderJitter)에 위치한 좌표를 목표지점으로 설정
            wanderJitter = UnityEngine.Random.Range(wanderJitterMin, wanderJitterMax);
            Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

            /// 생성된 목표위치가 자신의 이동구역을 벗어나지 않게 조절
            targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
            targetPosition.y = 0f;
            targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

            return targetPosition;
        }

        /// <summary>
        /// 각도에 따른 반지름 거리의 위치를 반환하는 메소드
        /// </summary>
        /// <param name="radius">반지름</param>
        /// <param name="angle">각도</param>
        /// <returns>각도에 따른 반지름 거리의 위치</returns>
        public Vector3 SetAngle(float radius, int angle)
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Cos(angle) * radius;
            position.z = Mathf.Sin(angle) * radius;

            return position;
        }

        /// <summary>
        /// 타겟을 향해 바라보게 회전시키는 메소드
        /// </summary>
        public void LookRotationToTarget()
        {
            Vector3 to = new Vector3(target.position.x, 0, target.position.z);
            Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

            /// 바로 회전
            transform.rotation = Quaternion.LookRotation(to - from);
            /// 서서히 회전
            //rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
        }

        /// <summary>
        /// 타겟과의 거리와 선택할 상태를 계산하는 메소드
        /// </summary>
        public void CalculateDistanceToTargetAndSelectState()
        {
            /// 타겟이 비어있거나 바꿀 수 없는 상태이면 종료한다.
            if(target == null)
            {
                LogManager.ConsoleErrorLog("CalculateDistanceToTargetAndSelectState",
                    $"target is null"
                );
                return;
            }
            if (canChangeState == false)
            {
                LogManager.ConsoleDebugLog("CalculateDistanceToTargetAndSelectState", 
                    $"CurrentState: {stateMachine.CurrentState}" + $"CanChangeState: {canChangeState}"
                );
                return;
            }

            float distance = Vector3.Distance(target.position, transform.position);

            if (distance <= attackRange)
            {
                ChangeState(EnemyNormalStates.Attack);
            }
            else if (distance <= targetRecognitionRange)
            {
                ChangeState(EnemyNormalStates.Pursuit);
            }
            /// 현재 상태가 Idle라면 Wander 상태로 변경하지 않는다.
            else if (distance >= pursuitLimitRange && currentState != EnemyNormalStates.Idle)
            {
                ChangeState(EnemyNormalStates.Wander);
            }
        }

        public void SetCanChangeState(bool canChange, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "")
        {
            LogManager.ConsoleDebugLog("SetCanChangeState", $"Caller: {callerName}, Set: {canChange}");
            this.canChangeState = canChange;
        }

        public bool GetCanChangeState([System.Runtime.CompilerServices.CallerMemberName] string callerName = "")
        {
            LogManager.ConsoleDebugLog("GetCanChangeState", $"Caller: {callerName}, Get: {canChangeState}");
            return canChangeState;
        }

        private void OnDrawGizmos()
        {
            try
            {
                /// "배회" 상태일 떄 이동할 경로 표시
                Gizmos.color = Color.black;
                Gizmos.DrawRay(transform.position, navMeshAgentController.destination - transform.position);

                /// 목표 인식 범위
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

                /// 추적 범위
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);

                Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
                Gizmos.DrawWireSphere(transform.position, attackRange);
            }
            catch (Exception e)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}