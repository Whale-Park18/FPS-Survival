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
         * ���� ����
         ****************************************/
        private StateBase<EnemyNormal>[]    states;
        private StateMachine<EnemyNormal>   stateMachine;
        private EnemyNormalStates           currentState;
        protected bool                      canChangeState = true;  // ���¸� ������ �� �ִ��� ��Ÿ���� �÷���
                                                                    // Ȯ�� ����X

        [Header("Attack")]
        [SerializeField]
        private GameObject                  projectilePrefab;       // �߻�ü ������
        [SerializeField]
        private Transform                   projectileSpawnPoint;   // �߻�ü ���� ��ġ
        [SerializeField]
        private float                       attackDistance = 5f;    // ���� ����(�� ���� �ȿ� ������ "Attack"���·� ����)
        [SerializeField]
        private float                       attackRate = 1f;        // ���� �ӵ�

        /****************************************
         * ������Ƽ
         ****************************************/
        public float AttackDistance => attackDistance;
        public float AttackRate => attackRate;
        public Transform ProjectileSpawnPoint => projectileSpawnPoint;
        public EnemyNormalStates CurrentState => currentState;

        public float attackAnimSpeed; // ���� �ִϸ��̼� �ӵ�(�׽�Ʈ��)

        protected override void Awake()
        {
            base.Awake();

            // #DEBUG
            // �ִϸ��̼� �̺�Ʈ ���� �ʱ�ȭ
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
            /// 1. ���� ������Ʈ�� ���� ������Ʈ ���� �� �ʱ�ȭ
            GameObject statesObject = new GameObject("States");
            statesObject.transform.parent = transform;
            statesObject.transform.localPosition = Vector3.zero;

            /// 2. ���� ������Ʈ ����
            states = new StateBase<EnemyNormal>[4];
            states[(int)EnemyNormalStates.Idle]     = statesObject.AddComponent<Idle>();
            states[(int)EnemyNormalStates.Wander]   = statesObject.AddComponent<Wander>();
            states[(int)EnemyNormalStates.Pursuit]  = statesObject.AddComponent<Pursuit>();
            states[(int)EnemyNormalStates.Attack]   = statesObject.AddComponent<Attack>();

            /// 3. ���� �ӽ� �ʱ�ȭ
            stateMachine = new StateMachine<EnemyNormal>();
            stateMachine.Setup(this);
        }

        /// <summary>
        /// �ִϸ��̼� �̺�Ʈ �ʱ�ȭ(�׽�Ʈ)
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
            /// ���� ���°� "Attack"�� ��, �߻� ���� �̻����� �ִٸ� ������Ų��.
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
            /// Ÿ���� ����ְų� �ٲ� �� ���� �����̸� �����Ѵ�.
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
            /// ���� ���°� Idle��� Wander ���·� �������� �ʴ´�.
            else if (distance >= wanderDistance && currentState != EnemyNormalStates.Idle)
            {
                ChangeState(EnemyNormalStates.Wander);
            }
        }

        /// <summary>
        /// ���¸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="newState">������ ���ο� ����</param>
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
            
            /// "��ȸ" ������ �� �̵��� ��� ǥ��
            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position, navMeshAgentController.destination - transform.position);

            /// ��ǥ �ν� ����
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, pursuitDistance);

            /// ���� ����
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, wanderDistance);

            Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
            Gizmos.DrawWireSphere(transform.position, attackDistance);
        }
    }
}