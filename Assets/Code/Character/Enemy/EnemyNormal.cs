using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

using WhalePark18.Manager;
using WhalePark18.FSM;
using WhalePark18.FSM.State;
using WhalePark18.FSM.State.StateEnemyNormal;
using WhalePark18.Character.Enemy.Event;
using NUnit.Framework;

namespace WhalePark18.Character.Enemy
{
    public enum EnemyNormalStates { Idle = 0, Wander, Pursuit, Attack, }
    public enum EnemyNormalAnimParam { isMovement, isAttack, doDie, }

    public class EnemyNormal : EnemyBase
    {
        /****************************************
         * 상태 관련
         ****************************************/
        private StateBase<EnemyNormal>[]    states;
        private StateMachine<EnemyNormal>   stateMachine;
        private EnemyNormalStates           currentState;
        protected bool                      canChangeState = true;  // 상태를 변경할 수 있는지 나타내는 플래그
                                                                    // 확정 변수X

        [Header("Attack")]
        [SerializeField]
        private GameObject                  projectilePrefab;       // 발사체 프리팹
        [SerializeField]
        private Transform                   projectileSpawnPoint;   // 발사체 생성 위치
        [SerializeField]
        private float                       attackDistance = 5f;    // 공격 범위(이 범위 안에 들어오면 "Attack"상태로 변경)
        [SerializeField]
        private float                       attackRate = 1f;        // 공격 속도

        /****************************************
         * 프로퍼티
         ****************************************/
        public float AttackDistance => attackDistance;
        public float AttackRate => attackRate;
        public Transform ProjectileSpawnPoint => projectileSpawnPoint;
        public EnemyNormalStates CurrentState => currentState;

        public float attackAnimSpeed; // 공격 애니메이션 속도(테스트용)

        protected override void Awake()
        {
            base.Awake();

            // #DEBUG
            // 애니메이션 이벤트 관련 초기화
            //OnAnimEventInitialized();

        }

        // #DEBUG
        private void Start()
        {
            // #DEBUG
            //Setup(0);
            //Run();
        }

        protected override void OnVariableInitialized()
        {
            base.OnVariableInitialized();

            // #DEBUG
            //animatorController.AnimatorSpeed = attackAnimSpeed;
        }

        protected override void OnStateInitialized()
        {
            /// 1. 상태 컴포넌트를 붙일 오브젝트 생성 및 초기화
            GameObject statesObject = new GameObject("States");
            statesObject.transform.parent = transform;
            statesObject.transform.localPosition = Vector3.zero;

            /// 2. 상태 컴포넌트 부착
            states = new StateBase<EnemyNormal>[4];
            states[(int)EnemyNormalStates.Idle]     = statesObject.AddComponent<Idle>();
            states[(int)EnemyNormalStates.Wander]   = statesObject.AddComponent<Wander>();
            states[(int)EnemyNormalStates.Pursuit]  = statesObject.AddComponent<Pursuit>();
            states[(int)EnemyNormalStates.Attack]   = statesObject.AddComponent<Attack>();

            /// 3. 상태 머신 초기화
            stateMachine = new StateMachine<EnemyNormal>();
            stateMachine.Setup(this);
        }

        /// <summary>
        /// 애니메이션 이벤트 초기화(테스트)
        /// </summary>
        private void OnAnimEventInitialized()
        {
            Transform meshObject = transform.GetChild(0).Find("Mesh Object");
            LogManager.ConsoleDebugLog("EnemyNormal", $"meshObject: {meshObject.name}");
            var eventAttack = meshObject.AddComponent<Event.EventEnemyNormal.EventAttack>();
            eventAttack.Setup(this);
        }

        //public override void Setup(int id)
        //{
        //    //base.Setup(id, target);
        //    base.Setup(id);
        //}

        public override void Run()
        {
            navMeshAgentController.NavMeshAgentEnabled = true;
            ChangeState(EnemyNormalStates.Idle);
        }

        public override void Reset()
        {
            base.Reset();
            stateMachine.Reset();
        }

        protected override void Die()
        {
            /// 현재 상태가 "Attack"일 때, 발사 전인 미사일이 있다면 정지시킨다.
            if (currentState.Equals(EnemyNormalStates.Attack))
            {
                Attack stateAttack = (Attack)states[(int)currentState];
                if (stateAttack.IsBeforeMissileFired())
                    stateAttack.Missile.Stop();
            }

            base.Die();
        }

        public override void CalculateDistanceToTargetAndSelectState()
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
            if (distance <= attackDistance)
            {
                ChangeState(EnemyNormalStates.Attack);
            }
            else if (distance <= pursuitDistance)
            {
                ChangeState(EnemyNormalStates.Pursuit);
            }
            /// 현재 상태가 Idle라면 Wander 상태로 변경하지 않는다.
            else if (distance >= wanderDistance && currentState != EnemyNormalStates.Idle)
            {
                ChangeState(EnemyNormalStates.Wander);
            }
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

        protected void OnDrawGizmos()
        {
            if (target == null) return;
            
            /// "배회" 상태일 떄 이동할 경로 표시
            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position, navMeshAgentController.destination - transform.position);

            /// 목표 인식 범위
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, pursuitDistance);

            /// 추적 범위
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, wanderDistance);

            Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
            Gizmos.DrawWireSphere(transform.position, attackDistance);
        }
    }
}