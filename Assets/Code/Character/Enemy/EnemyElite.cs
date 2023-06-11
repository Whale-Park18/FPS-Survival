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
         * 상태 관련
         ****************************************/
        private StateBase<EnemyElite>[] states;
        private StateMachine<EnemyElite> stateMachine;
        private EnemyEliteStates currentState;
        protected bool canChangeState = true;

        [Header("Attack")]

        [Tooltip("일반 공격력")]
        public int NormalDamage;

        [Tooltip("돌진 공격력")]
        public int RushDamage;

        [Tooltip("일반 공격 주기(초)")]
        public float NormalAttackCycle;

        [Tooltip("돌진 공격 주기(초)")]
        public float RushAttackCycle;

        [Tooltip("최대 공격 거리(m)")]
        public float MaxAttackDistance;

        [Tooltip("돌진 속도(m/s)")]
        public float RushSpeed;

        [Tooltip("돌진 전 대기시간(초)")]
        public float WaitTimeBeforeAttack;

        [Tooltip("돌진 후 대기시간(초)")]
        public float WaitTimeAfterAttack;

        [HideInInspector]
        public Vector3 AttackDestination;

        [Space(10)]
        [Tooltip("공격 콜라이더 오프셋(m)")]
        public float AttackColliderOffset;

        [Tooltip("공격 콜라이더 반지름(m)")]
        public float AttackColliderRadius;

        [Tooltip("플레이어 레이어 마스크")]
        public LayerMask PlayerMask;

        [Tooltip("몬스터 중심")]
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

        [Tooltip("행동 정지 플래그")]
        public bool DontAction;

        [Tooltip("Nav 에이전트 목적지 표시 오브젝트")]
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
            /// 1. 상태 컴포넌트를 붙일 오브젝트 생성 및 초기화
            GameObject statesObject = new GameObject("States");
            statesObject.transform.parent = transform;
            statesObject.transform.localPosition = Vector3.zero;

            /// 2. 상태 컴포넌트 부착
            states = new StateBase<EnemyElite>[4];
            states[(int)EnemyEliteStates.Idle] = statesObject.AddComponent<Idle>();
            states[(int)EnemyEliteStates.Wander] = statesObject.AddComponent<Wander>();
            states[(int)EnemyEliteStates.Pursuit] = statesObject.AddComponent<Pursuit>();
            states[(int)EnemyEliteStates.Attack] = statesObject.AddComponent<Attack>();

            /// 3. 상태 머신 초기화
            stateMachine = new StateMachine<EnemyElite>();
            stateMachine.Setup(this);
        }

        /// <summary>
        /// 플레이어 스캔 메소드
        /// </summary>
        private void ScanPlayer()
        {
            _scanRay = new Ray(Center.position, transform.forward);
            _scanHit = Physics.SphereCast(_scanRay, AttackColliderRadius, out _scanRayHitInfo, MaxAttackDistance, PlayerMask);

            /// 스캔 성공 && 공격 주기 && 현재 상태가 공격이 아니라면 공격 실행
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
        /// 근접 공격
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
            /// 타겟이 비어있거나 바꿀 수 없는 상태이면 종료한다.
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
            /// 현재 상태가 Idle라면 Wander 상태로 변경하지 않는다.
            else if (distance > pursuitDistance && currentState != EnemyEliteStates.Idle)
            {
                ChangeState(EnemyEliteStates.Wander);
            }
        }

        /// <summary>
        /// 상태 변경 메소드
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(EnemyEliteStates newState)
        {
            if (DontAction) return;

            currentState = newState;
            stateMachine.ChangeState(states[(int)newState]);
        }

        /// <summary>
        /// 상태 변경 가능 플래그 설정 메소드
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
            /// 공격 범위
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(Center.position + transform.forward * AttackColliderOffset, AttackColliderRadius);
            }

            /// 돌진
            {
                Gizmos.color = Color.blue;

                var gizmoDistance = _scanHit ? _scanRayHitInfo.distance : MaxAttackDistance;
                Gizmos.DrawRay(Center.position, transform.forward * gizmoDistance);
                Gizmos.DrawWireSphere(Center.position + transform.forward * gizmoDistance, AttackColliderRadius);
            }
        }

        private void OnDrawGizmosSelected()
        {
            /// 추적 범위
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, pursuitDistance);
            }

            /// 수색 범위
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, wanderDistance);
            }

            /// 현재 위치에서 도착지까지의 직선 거리
            {
                if (navMeshAgentController == null) return;

                Gizmos.color = Color.black;
                Gizmos.DrawRay(transform.position, navMeshAgentController.destination - transform.position);
            }
        }
    }
}