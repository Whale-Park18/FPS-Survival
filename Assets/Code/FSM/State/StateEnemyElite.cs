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
            owner.AnimatorController.SetFloat(EnemyNormalAnimParam.Speed.ToString(), 0);

            Execute(owner);
        }

        public override void Execute(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Idle", "Execute");

            coroutineHandle = StartCoroutine(OnIdle(owner));
        }

        public override void Exit(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Idle", "Exit");

            /// Idle 코루틴 정지
            if (coroutineHandle != null)
                StopAllCoroutines();
            coroutineHandle = null;
        }

        /// <summary>
        /// 실제 Idle 상태를 실행하는 메소드
        /// </summary>
        /// <param name="owner">상태를 가진 Enemy</param>
        /// <returns></returns>
        private IEnumerator OnIdle(EnemyElite owner)
        {
            /// 0 ~ MaxIdleTime 시간 동안 기다린 후, Wander 상태로 변경한다.
            float idleTime = Random.Range(1, owner.MaxIdleTime);
            StartCoroutine(AutoChangeFromIdleToWander(idleTime, owner));

            while (true)
            {
                owner.CalculateDistanceToTargetAndSelectState();
            
                yield return null;
            }
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
            owner.AnimatorController.SetFloat(EnemyNormalAnimParam.Speed.ToString(), owner.Status.WalkSpeed);
            owner.NavMeshAgentController.speed = owner.Status.WalkSpeed;
            owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Move);

            Execute(owner);
        }

        public override void Execute(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander", "Execute");

            coroutineHandle = StartCoroutine(OnWander(owner));
        }

        public override void Exit(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander", "Exit");

            /// Wander 코루틴 정지
            if (coroutineHandle != null)
                StopAllCoroutines();
            coroutineHandle = null;

            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander", $"coroutine is null: {coroutineHandle == null}");

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
            if (GameManager.Instance.DebugMode && GameManager.Instance.DevelopingEnemy)
                owner.NavMeshAgentController.SetDestination(wanderPosition, owner.NavDestinationShowObject.transform);
            else
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
            owner.AnimatorController.SetFloat(EnemyNormalAnimParam.Speed.ToString(), owner.Status.RunSpeed);
            owner.NavMeshAgentController.speed = owner.Status.RunSpeed;
            owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Move);

            Execute(owner);
        }

        public override void Execute(EnemyElite owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Pursuit", "Execute");

            coroutineHandle = StartCoroutine(OnPursuit(owner));
        }

        public override void Exit(EnemyElite owner)
        {
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Pursuit", "Exit");

            /// Pursuit 코루틴 정지
            if (coroutineHandle != null)
                StopAllCoroutines();
            coroutineHandle = null;

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
                if (GameManager.Instance.DebugMode && GameManager.Instance.DevelopingEnemy)
                    owner.NavMeshAgentController.SetDestination(owner.Target.position, owner.NavDestinationShowObject.transform);
                else
                    owner.NavMeshAgentController.SetDestination(owner.Target.position);
                owner.LookRotationToTarget();

                owner.CalculateDistanceToTargetAndSelectState();

                yield return null;
            }
        }
    }

    public class Attack : StateBase<EnemyElite>
    {
        private WaitForSeconds _waitStopBeforeAttack;
        private WaitForSeconds _waitStopAfterAttack;

        public override void Enter(EnemyElite owner)
        {
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Attack", "Enter");

            _waitStopBeforeAttack = new WaitForSeconds(owner.WaitTimeBeforeAttack);
            _waitStopAfterAttack = new WaitForSeconds(owner.WaitTimeAfterAttack);

            owner.SetCanChangeState(false);
            owner.NavMeshAgentController.speed = owner.RushSpeed;
            owner.SetDamage(owner.RushDamage);

            Execute(owner);
        }

        public override void Execute(EnemyElite owner)
        {
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Attack", "Execute");

            coroutineHandle = StartCoroutine(OnAttack(owner));
        }

        public override void Exit(EnemyElite owner)
        {
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Attack", "Exit");

            if (coroutineHandle != null)
                StopAllCoroutines();
            coroutineHandle = null;

            owner.SetDamage(owner.NormalDamage);
        }

        private IEnumerator OnAttack(EnemyElite owner)
        {
            LogManager.ConsoleDebugLog($"{name} Attack", "Part. 1 공격 전 대기");
            {
                owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Stop);
                owner.AnimatorController.SetFloat(EnemyEliteAnimParam.Speed.ToString(), 0);
                yield return _waitStopBeforeAttack;
            }
            
            LogManager.ConsoleDebugLog($"{name} Attack", "Part. 2 공격");
            {
                owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Move);
                owner.AnimatorController.SetBool(EnemyEliteAnimParam.isAttack.ToString(), true);
                yield return StartCoroutine(OnMoveToDestination(owner));
                owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Stop, BreakMode.SmoothStop);
            }
            
            LogManager.ConsoleDebugLog($"{name} Attack", "Part. 3 공격 후 대기");
            {
                owner.AnimatorController.SetBool(EnemyEliteAnimParam.isAttack.ToString(), false);
                yield return _waitStopAfterAttack;
            }
            
            LogManager.ConsoleDebugLog($"{name} Attack", "Part. 4 상태 변경");
            {
                owner.SetCanChangeState(true);
                owner.CalculateDistanceToTargetAndSelectState();
            }
        }

        /// <summary>
        /// 목적지까지 이동하는 메소드
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        private IEnumerator OnMoveToDestination(EnemyElite owner)
        {
            if (GameManager.Instance.DebugMode && GameManager.Instance.DevelopingEnemy)
                owner.NavMeshAgentController.SetDestination(owner.Target.position, owner.NavDestinationShowObject.transform);
            else
                owner.NavMeshAgentController.SetDestination(owner.Target.position);

            var waitUntilOneArrive = new WaitUntil(
                () => { return owner.NavMeshAgentController.IsArrived(); } 
            );
            yield return waitUntilOneArrive;
        }
    }
}