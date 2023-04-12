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
        private float targetRecognitionRange = 8f;          // �ν� ����(�� ���� �ȿ� ������ "Pursuit" ���·� ����
        [SerializeField]
        private float pursuitLimitRange = 10f;              // ���� ����(�� ���� ������ ������ "Wander" ���·� ����

        [Header("Attack")]
        [SerializeField]
        private GameObject projectilePrefab;                // �߻�ü ������
        [SerializeField]
        private Transform projectileSpawnPoint;             // �߻�ü ���� ��ġ
        [SerializeField]
        private float attackRange = 5f;                     // ���� ����(�� ���� �ȿ� ������ "Attack"���·� ����)
        [SerializeField]
        private float attackRate = 1f;                      // ���� �ӵ�


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
            /// 1. ������Ʈ �ʱ�ȭ
            status                  = GetComponent<EnemyStatus>();
            navMeshAgentController  = GetComponent<EnemyNavMeshAgentController>();
            animatorController      = GetComponent<EnemyAnimatorController>();

            /// 2. ���� �ʱ�ȭ
            /// 2.1. ���� ������Ʈ�� ���� ������Ʈ ���� �� �ʱ�ȭ
            GameObject statesObject = new GameObject("States");
            statesObject.transform.parent = transform;
            statesObject.transform.localPosition = Vector3.zero;

            /// 2.2. ���� ������Ʈ ����
            states = new StateBase<EnemyNormal>[4];
            states[(int)EnemyNormalStates.Idle]     = statesObject.AddComponent<Idle>();
            states[(int)EnemyNormalStates.Wander]   = statesObject.AddComponent<Wander>();
            states[(int)EnemyNormalStates.Pursuit]  = statesObject.AddComponent<Pursuit>();
            states[(int)EnemyNormalStates.Attack]   = statesObject.AddComponent<Attack>();

            /// 3. �ִϸ��̼� �̺�Ʈ ������Ʈ ����
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
        /// ���¸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="newState">������ ���ο� ����</param>
        public void ChangeState(EnemyNormalStates newState)
        {
            currentState = newState;
            stateMachine.ChangeState(states[(int)newState]);
        }

        /// <summary>
        /// Ž���� ��ġ�� ����ϴ� �޼ҵ�
        /// </summary>
        /// <returns>Ž�� ��ġ</returns>
        public Vector3 CalculateWanderPosition()
        {
            float wanderRadius = 10f;   // ���� ��ġ�� �������� �ϴ� ���� ������
            int wanderJitter = 0;       // ���õ� ���� (wanderJitterMin ~ wanderJitterMax)
            int wanderJitterMin = 0;    // �ּ� ����
            int wanderJitterMax = 360;  // �ִ� ����

            /// ���� �� ĳ���Ͱ� �ִ� ������ �߽� ��ġ�� ũ��(������ ��� �ൿ�� ���� �ʵ���)
            Vector3 rangePosition = Vector3.zero;
            Vector3 rangeScale = Vector3.one * 100f;

            /// �ڽ��� ��ġ�� �߽����� ������(wanderRadius) �Ÿ�,
            /// ���õ� ����(wanderJitter)�� ��ġ�� ��ǥ�� ��ǥ�������� ����
            wanderJitter = UnityEngine.Random.Range(wanderJitterMin, wanderJitterMax);
            Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

            /// ������ ��ǥ��ġ�� �ڽ��� �̵������� ����� �ʰ� ����
            targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
            targetPosition.y = 0f;
            targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

            return targetPosition;
        }

        /// <summary>
        /// ������ ���� ������ �Ÿ��� ��ġ�� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <param name="radius">������</param>
        /// <param name="angle">����</param>
        /// <returns>������ ���� ������ �Ÿ��� ��ġ</returns>
        public Vector3 SetAngle(float radius, int angle)
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Cos(angle) * radius;
            position.z = Mathf.Sin(angle) * radius;

            return position;
        }

        /// <summary>
        /// Ÿ���� ���� �ٶ󺸰� ȸ����Ű�� �޼ҵ�
        /// </summary>
        public void LookRotationToTarget()
        {
            Vector3 to = new Vector3(target.position.x, 0, target.position.z);
            Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

            /// �ٷ� ȸ��
            transform.rotation = Quaternion.LookRotation(to - from);
            /// ������ ȸ��
            //rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
        }

        /// <summary>
        /// Ÿ�ٰ��� �Ÿ��� ������ ���¸� ����ϴ� �޼ҵ�
        /// </summary>
        public void CalculateDistanceToTargetAndSelectState()
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

            if (distance <= attackRange)
            {
                ChangeState(EnemyNormalStates.Attack);
            }
            else if (distance <= targetRecognitionRange)
            {
                ChangeState(EnemyNormalStates.Pursuit);
            }
            /// ���� ���°� Idle��� Wander ���·� �������� �ʴ´�.
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
                /// "��ȸ" ������ �� �̵��� ��� ǥ��
                Gizmos.color = Color.black;
                Gizmos.DrawRay(transform.position, navMeshAgentController.destination - transform.position);

                /// ��ǥ �ν� ����
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

                /// ���� ����
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