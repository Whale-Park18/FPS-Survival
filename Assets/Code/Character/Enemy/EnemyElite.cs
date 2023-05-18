using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WhalePark18.FSM;
using WhalePark18.FSM.State;
using WhalePark18.FSM.State.StateEnemyElite;
using WhalePark18.Manager;

namespace WhalePark18.Character.Enemy
{
    public enum EnemyEliteStates { Idle = 0, Wander, Pursuit, Attack, }
    public enum EnemyEliteAnimParam { isMovement, isAttack, doDie, }
    public enum EnemyEliteAnimName {  Idle, Walk, Attack, Die }

    public class EnemyElite : EnemyBase
    {
        /****************************************
         * ���� ����
         ****************************************/
        private StateBase<EnemyElite>[] states;
        private StateMachine<EnemyElite> stateMachine;
        private EnemyEliteStates currentState;
        protected bool canChangeState = true;

        [Header("Attack")]
        [SerializeField]
        private float attackRate = 1f;                      // ���� �ֱ�
        private float laskAttackTime = 0;
        [SerializeField]
        private float attackDistance = 5f;                  // ���� ����(�� ���� �ȿ� ������ "Attack"���·� ����)
        [SerializeField]
        private float dashSpeed;
        private Ray forwardRay;
        private RaycastHit forwardRayHit;
        [SerializeField]
        private float stopTimeBeforeAttack;
        [SerializeField]
        private float stopTimeAfterAttack;

        [SerializeField] private Transform colliderTransform;
        [SerializeField] private float colliderRadius = 1f;
        [SerializeField] private LayerMask playerLayer = 0;

        /****************************************
         * ������Ƽ
         ****************************************/
        public float AttackDistance => attackDistance;
        public float Radius => colliderRadius;
        public LayerMask PlayerLayer => playerLayer;
        public float DashSpeed => dashSpeed;

        private void Start()
        {
            // #DEBUG
            //Setup(0);
            //ChangeState(EnemyEliteStates.Idle);
        }

        private void Update()
        {
            forwardRay = new Ray(colliderTransform.position, transform.forward);
            var isHit = Physics.SphereCast(forwardRay, colliderRadius, out forwardRayHit, attackDistance, playerLayer);

            if (isHit && Time.time - laskAttackTime > attackRate && currentState != EnemyEliteStates.Attack)
            {
                var dashDestination = forwardRayHit.point;
                dashDestination.y = 0;
                LogManager.ConsoleDebugLog("Elite", $"dashDestination: {dashDestination}");
                ((Attack)states[(int)EnemyEliteStates.Attack]).dashDestination = dashDestination;

                ChangeState(EnemyEliteStates.Attack);
            }
        }

        protected override void OnVariableInitialized()
        {
            base.OnVariableInitialized();
        }

        protected override void OnStateInitialized()
        {
            /// 1. ���� ������Ʈ�� ���� ������Ʈ ���� �� �ʱ�ȭ
            GameObject statesObject = new GameObject("States");
            statesObject.transform.parent = transform;
            statesObject.transform.localPosition = Vector3.zero;

            /// 2. ���� ������Ʈ ����
            states = new StateBase<EnemyElite>[4];
            states[(int)EnemyEliteStates.Idle] = statesObject.AddComponent<Idle>();
            states[(int)EnemyEliteStates.Wander] = statesObject.AddComponent<Wander>();
            states[(int)EnemyEliteStates.Pursuit] = statesObject.AddComponent<Pursuit>();
            states[(int)EnemyEliteStates.Attack] = statesObject.AddComponent<Attack>();

            ((Attack)states[(int)EnemyEliteStates.Attack]).Setup(
                stopTimeBeforeAttack, stopTimeAfterAttack, dashSpeed
            );

            /// 3. ���� �ӽ� �ʱ�ȭ
            stateMachine = new StateMachine<EnemyElite>();
            stateMachine.Setup(this);
        }

        public override void Setup(int id)
        {
            base.Setup(id);
        }

        public override void Run()
        {
            navMeshAgentController.NavMeshAgentEnabled = true;
            ChangeState(EnemyEliteStates.Idle);
        }

        public override void CalculateDistanceToTargetAndSelectState()
        {
            /// Ÿ���� ����ְų� �ٲ� �� ���� �����̸� �����Ѵ�.
            if (target == null)
            {
                LogManager.ConsoleErrorLog("CalculateDistanceToTargetAndSelectState",
                    $"target is null"
                );
                return;
            }

            float distance = Vector3.Distance(target.position, transform.position);
            if (distance <= pursuitDistance)
            {
                ChangeState(EnemyEliteStates.Pursuit);
            }
            /// ���� ���°� Idle��� Wander ���·� �������� �ʴ´�.
            else if (distance > pursuitDistance && currentState != EnemyEliteStates.Idle)
            {
                ChangeState(EnemyEliteStates.Wander);
            }
        }

        public void ChangeState(EnemyEliteStates newState)
        {
            currentState = newState;
            stateMachine.ChangeState(states[(int)newState]);
        }

        private void OnDrawGizmos()
        {
            /// ����
            {
                Gizmos.color = Color.blue;

                Gizmos.DrawRay(colliderTransform.position, transform.forward * forwardRayHit.distance);
                Gizmos.DrawWireSphere(colliderTransform.position + transform.forward * forwardRayHit.distance, colliderRadius);
            }

            /// ���� ����
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, pursuitDistance);
            }

            /// ���� ����
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, wanderDistance);
            }

            /// ���� ��ġ���� ������������ ���� �Ÿ�
            {
                Gizmos.color = Color.black;
                Gizmos.DrawRay(transform.position, navMeshAgentController.destination - transform.position);
            }
        }
    }
}