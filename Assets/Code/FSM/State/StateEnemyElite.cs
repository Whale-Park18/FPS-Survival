using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using WhalePark18.Character.Enemy;
using WhalePark18.Manager;

namespace WhalePark18.FSM.State.StateEnemyElite
{
    public class Idle : StateBase<EnemyElite>
    {
        public override void Enter(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Idle", "Enter");

            /// 애니메이션 설정 후, Idle 상태를 실행한다.
            owner.AnimatorController.SetBool(EnemyNormalAnimParam.isMovement.ToString(), false);

            Execute(owner);
        }

        public override void Execute(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Idle", "Execute");
            
            OnIdle(owner);
        }

        public override void Exit(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Idle", "Exit");

            /// Idle 코루틴 정지
            if (coroutine != null)
                StopAllCoroutines();
            coroutine = null;
        }

        /// <summary>
        /// 실제 Idle 상태를 실행하는 메소드
        /// </summary>
        /// <param name="owner">상태를 가진 Enemy</param>
        /// <returns></returns>
        private void OnIdle(EnemyElite owner)
        {
            /// 0 ~ MaxIdleTime 시간 동안 기다린 후, Wander 상태로 변경한다.
            float idleTime = Random.Range(1, owner.MaxIdleTime);
            StartCoroutine(AutoChangeFromIdleToWander(idleTime, owner));

            //while (true)
            //{
            //    owner.CalculateDistanceToTargetAndSelectState();
            //
            //    yield return null;
            //}
        }

        /// <summary>
        /// 대기 후, 방황 상태로 변경하는 메소드
        /// </summary>
        /// <param name="waitTime">대기 시간</param>
        /// <param name="owner">상태를 가진 Enemy</param>
        /// <returns>코루틴</returns>
        private IEnumerator AutoChangeFromIdleToWander(float waitTime, EnemyElite owner)
        {
            yield return new WaitForSeconds(waitTime);

            owner.ChangeState(EnemyEliteStates.Wander);
        }
    }

    public class Wander : StateBase<EnemyElite>
    {
        public override void Enter(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander", "Enter");

            /// 애니메이션 설정 후, Wander 상태를 실행한다.
            owner.AnimatorController.SetBool(EnemyNormalAnimParam.isMovement.ToString(), true);
            owner.NavMeshAgentController.speed = owner.Status.WalkSpeed;
            owner.NavMeshAgentController.stoppingDistance = 0;

            Execute(owner);
        }

        public override void Execute(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander", "Execute");

            coroutine = StartCoroutine(OnWander(owner));
        }

        public override void Exit(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander", "Exit");

            /// Wander 코루틴 정지
            if (coroutine != null)
                StopAllCoroutines();
            coroutine = null;

            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander", $"coroutine is null: {coroutine == null}");

            /// 경로 초기화
            owner.NavMeshAgentController.ResetPath();
        }

        /// <summary>
        /// 실제 Wander 상태를 실행하는 메소드
        /// </summary>
        /// <param name="owner">상태를 가진 Enemy</param>
        /// <returns>코루틴</returns>
        private IEnumerator OnWander(EnemyElite owner)
        {
            Vector3 wanderPosition = owner.CalculateWanderPosition();
            owner.NavMeshAgentController.SetDestination(wanderPosition);

            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander", $"wanderPosition: {wanderPosition}");

            Vector3 to = new Vector3(owner.NavMeshAgentController.destination.x, 0, owner.NavMeshAgentController.destination.z);
            Vector3 from = new Vector3(owner.transform.position.x, 0, owner.transform.position.z);
            owner.transform.rotation = Quaternion.LookRotation(to - from);

            float currentTime = 0;
            float maxTime = 10f;
            while (true)
            {
                currentTime += Time.deltaTime;

                to = new Vector3(owner.NavMeshAgentController.destination.x, 0, owner.NavMeshAgentController.destination.z);
                from = new Vector3(owner.transform.position.x, 0, owner.transform.position.z);

                /// 목적지까지의 근처에 접근했거나 너무 오랜시간 동안 
                /// 'Wander' 상태에 머물러 있으면 'Idle' 상태로 변경한다.
                if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
                {
                    owner.ChangeState(EnemyEliteStates.Idle);
                }

                /// 타겟과의 거리에 따라 상태를 선택한다.
                owner.CalculateDistanceToTargetAndSelectState();

                yield return null;
            }
        }
    }

    public class Pursuit : StateBase<EnemyElite>
    {
        public override void Enter(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Pursuit", "Enter");

            /// 애니메이션 파라미터, 이동 속도를 설정한 후, Pursuit 상태 실행
            owner.AnimatorController.SetBool(EnemyNormalAnimParam.isMovement.ToString(), true);
            owner.NavMeshAgentController.speed = owner.Status.RunSpeed;

            Execute(owner);
        }

        public override void Execute(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Pursuit", "Execute");

            coroutine = StartCoroutine(OnPursuit(owner));
        }

        public override void Exit(EnemyElite owner)
        {
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Pursuit", "Exit");

            /// Pursuit 코루틴 정지
            if (coroutine != null)
                StopAllCoroutines();
            coroutine = null;

            /// 경로 초기화
            owner.NavMeshAgentController.ResetPath();
            owner.NavMeshAgentController.speed = owner.Status.WalkSpeed;
        }

        /// <summary>
        /// 실제 Pursuit 상태를 실행하는 메소드
        /// </summary>
        /// <param name="owner">상태를 가진 Enemy</param>
        /// <returns>코루틴</returns>
        private IEnumerator OnPursuit(EnemyElite owner)
        {
            while (true)
            {
                /// 개체의 속도를 RunSpeed로 바꾸고 타겟의 위치로 경로를 갱신한다.
                owner.NavMeshAgentController.SetDestination(owner.Target.position);
                owner.LookRotationToTarget();

                owner.CalculateDistanceToTargetAndSelectState();

                yield return null;
            }
        }
    }

    public class Attack : StateBase<EnemyElite>
    {
        public float stopTimeBeforeAttack;
        public float stopTimeAfterAttack;
        public Vector3 dashDestination;
        public float dashSpeed;

        private WaitForSeconds waitStopBeforeAttack;
        private WaitForSeconds waitStopAfterAttack;

        public void Setup(float stopTimeBeforeAttack, float stopTimeAfterAttack, float attackMoveSpeed)
        {
            this.stopTimeBeforeAttack = stopTimeBeforeAttack;
            this.stopTimeAfterAttack = stopTimeAfterAttack;
            this.dashSpeed = attackMoveSpeed;
        }

        public override void Enter(EnemyElite owner)
        {
            LogManager.ConsoleDebugLog("Attack<Elite>", "Enter");

            waitStopBeforeAttack = new WaitForSeconds(stopTimeBeforeAttack);
            waitStopAfterAttack = new WaitForSeconds(stopTimeAfterAttack);

            owner.AnimatorController.GetAnimationClip(EnemyEliteAnimName.Attack.ToString());

            Execute(owner);
        }

        public override void Execute(EnemyElite owner)
        {
            LogManager.ConsoleDebugLog("Attack<Elite>", "Execute");

            StartCoroutine(OnAttack(owner));
        }

        public override void Exit(EnemyElite owner)
        {
            LogManager.ConsoleDebugLog("Attack<Elite>", "Exit");

            owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Move);
        }

        private IEnumerator OnAttack(EnemyElite owner)
        {
            /// NavMeshAgent 상태를 "Stop"으로 변경한 후, stopTimeBeforeAttack초 만큼 대기
            LogManager.ConsoleDebugLog("Attack<Elite>", "공격 전 대기");
            {
                owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Stop);
                owner.AnimatorController.SetBool(EnemyEliteAnimParam.isMovement.ToString(), false);
                yield return waitStopBeforeAttack;
            }


            /// "Attack" 애니메이션 활성화
            /// NavMeshAgent 상태와 속도를 변경한 후, 목적지를 돌진 목적지로 설정한다.
            LogManager.ConsoleDebugLog("Attack<Elite>", "공격 중");
            {
                owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Move);
                owner.NavMeshAgentController.speed = owner.DashSpeed;
                owner.NavMeshAgentController.SetDestination(dashDestination);
                owner.AnimatorController.SetBool(EnemyEliteAnimParam.isAttack.ToString(), true);

                yield return StartCoroutine(OnMoveToDestination(owner));
            }

            /// stopTimeAfterAttack초 만큼 대기 후, "Wander" 상태로 변경
            LogManager.ConsoleDebugLog("Attack<Elite>", "공격 후 대기");
            {
                owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Stop, BreakMode.SmoothStop);
                owner.NavMeshAgentController.speed = owner.Status.WalkSpeed;
                owner.AnimatorController.SetBool(EnemyEliteAnimParam.isAttack.ToString(), false);

                yield return waitStopAfterAttack;
            }

            LogManager.ConsoleDebugLog("Attack<Elite>", "상태 전환");
            {
                owner.ChangeState(EnemyEliteStates.Wander);
            }
        }

        /// <summary>
        /// 목적지까지 이동하는 메소드
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        private IEnumerator OnMoveToDestination(EnemyElite owner)
        {
            owner.NavMeshAgentController.SetDestination(dashDestination);

            var waitUntilOneArrive = new WaitUntil(
                () => { return owner.NavMeshAgentController.IsArrived(); } 
            );
            yield return waitUntilOneArrive;
        }

        private void ChangeSpeed(EnemyElite owner, float speed)
        {
            owner.NavMeshAgentController.speed = speed;
        }
    }
}