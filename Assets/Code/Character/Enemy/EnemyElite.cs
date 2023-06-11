using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.Character.Player;
using WhalePark18.FSM;
using WhalePark18.FSM.State;
using WhalePark18.FSM.State.StateEnemyElite;
using WhalePark18.Manager;

namespace WhalePark18.Character.Enemy
{
    public enum EnemyEliteStates { Idle = 0, Wander, Pursuit, Attack, }
    public enum EnemyEliteAnimParam { Speed, isAttack, doDie, }
    public enum EnemyEliteAnimName { Idle, Walk, Attack, Die }

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

        [Tooltip("�Ϲ� ���ݷ�")]
        public int NormalDamage;

        [Tooltip("���� ���ݷ�")]
        public int RushDamage;

        [Tooltip("�Ϲ� ���� �ֱ�(��)")]
        public float NormalAttackCycle;

        [Tooltip("���� ���� �ֱ�(��)")]
        public float RushAttackCycle;

        [Tooltip("�ִ� ���� �Ÿ�(m)")]
        public float MaxAttackDistance;

        [Tooltip("���� �ӵ�(m/s)")]
        public float RushSpeed;

        [Tooltip("���� �� ���ð�(��)")]
        public float WaitTimeBeforeAttack;

        [Tooltip("���� �� ���ð�(��)")]
        public float WaitTimeAfterAttack;

        [HideInInspector]
        public Vector3 AttackDestination;

        [Space(10)]
        [Tooltip("���� �ݶ��̴� ������(m)")]
        public float AttackColliderOffset;

        [Tooltip("���� �ݶ��̴� ������(m)")]
        public float AttackColliderRadius;

        [Tooltip("�÷��̾� ���̾� ����ũ")]
        public LayerMask PlayerMask;

        [Tooltip("���� �߽�")]
        public Transform Center;

        // Attack
        private int     _damage;
        private float   _lastAttackTime;
        private float   _lastRushTime = 0;

        // Scan
        private Ray         _scanRay;
        private RaycastHit  _scanRayHitInfo;
        private bool        _scanHit;

        [Header("Debug")]

        [Tooltip("�ൿ ���� �÷���")]
        public bool DontAction;

        [Tooltip("Nav ������Ʈ ������ ǥ�� ������Ʈ")]
        public GameObject NavDestinationShowObject;


        private void Start()
        {
            // #DEBUG
            if(GameManager.Instance.DebugMode && GameManager.Instance.DevelopingEnemy)
            {
                Setup(0);
                ChangeState(EnemyEliteStates.Idle);
            }
        }

        private void Update()
        {
            MeleeAttack();
            ScanPlayer();
        }

        protected override void OnVariableInitialized()
        {
            base.OnVariableInitialized();

            _damage = NormalDamage;
            _lastRushTime -= RushAttackCycle;
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

            /// 3. ���� �ӽ� �ʱ�ȭ
            stateMachine = new StateMachine<EnemyElite>();
            stateMachine.Setup(this);
        }

        /// <summary>
        /// �÷��̾� ��ĵ �޼ҵ�
        /// </summary>
        private void ScanPlayer()
        {
            _scanRay = new Ray(Center.position, transform.forward);
            _scanHit = Physics.SphereCast(_scanRay, AttackColliderRadius, out _scanRayHitInfo, MaxAttackDistance, PlayerMask);

            /// ��ĵ ���� && ���� �ֱ� && ���� ���°� ������ �ƴ϶�� ���� ����
            if (_scanHit && Time.time - _lastRushTime > RushAttackCycle && currentState != EnemyEliteStates.Attack)
            {
                _lastRushTime = Time.time;

                AttackDestination = _scanRayHitInfo.point;
                AttackDestination.y = 0;
                LogManager.ConsoleDebugLog($"{name}", $"AttackDestination: {AttackDestination}");

                ChangeState(EnemyEliteStates.Attack);
            }
        }

        /// <summary>
        /// ���� ����
        /// </summary>
        private void MeleeAttack()
        {
            Collider[] colliders = Physics.OverlapSphere(Center.position + transform.forward * AttackColliderOffset, AttackColliderRadius, PlayerMask);
   
            if ( colliders.Length > 0 && Time.time - _lastAttackTime > NormalAttackCycle)
            {
                _lastAttackTime = Time.time;
            
                var player = colliders[0].GetComponent<PlayerController>();
                player.TakeDamage(_damage);
            }
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

            if (canChangeState == false)
            {
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

        /// <summary>
        /// ���� ���� �޼ҵ�
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(EnemyEliteStates newState)
        {
            if (DontAction) return;

            currentState = newState;
            stateMachine.ChangeState(states[(int)newState]);
        }

        /// <summary>
        /// ���� ���� ���� �÷��� ���� �޼ҵ�
        /// </summary>
        /// <param name="canChange"></param>
        /// <param name="callerName"></param>
        public void SetCanChangeState(bool canChange, [System.Runtime.CompilerServices.CallerMemberName] string callerName = "")
        {
            LogManager.ConsoleDebugLog("SetCanChangeState", $"Caller: {callerName}, Set: {canChange}");
            this.canChangeState = canChange;
        }

        public void SetDamage(int damage)
        {
            _damage = damage;
        }

        private void OnDrawGizmos()
        {
            /// ���� ����
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(Center.position + transform.forward * AttackColliderOffset, AttackColliderRadius);
            }

            /// ����
            {
                Gizmos.color = Color.blue;

                var gizmoDistance = _scanHit ? _scanRayHitInfo.distance : MaxAttackDistance;
                Gizmos.DrawRay(Center.position, transform.forward * gizmoDistance);
                Gizmos.DrawWireSphere(Center.position + transform.forward * gizmoDistance, AttackColliderRadius);
            }
        }

        private void OnDrawGizmosSelected()
        {
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
                if (navMeshAgentController == null) return;

                Gizmos.color = Color.black;
                Gizmos.DrawRay(transform.position, navMeshAgentController.destination - transform.position);
            }
        }
    }
}