using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

using WhalePark18.Manager;
using WhalePark18.Projectile;
using WhalePark18.Character.Enemy;

namespace WhalePark18.FSM.State.StateEnemyNormal
{
    public class Idle : StateBase<EnemyNormal>
    {
        public override void Enter(EnemyNormal owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Idle<Enemy1>", "Enter");

            /// 애니메이션 설정 후, Idle 상태를 실행한다.
            owner.AnimatorController.SetBool(EnemyNormalAnimParam.isMovement.ToString(), false);

            Execute(owner);
        }

        public override void Execute(EnemyNormal owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Idle<Enemy1>", "Execute");

            /// Idle 코루틴 실행
            coroutine = StartCoroutine(OnIdle(owner));
        }

        public override void Exit(EnemyNormal owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Idle<Enemy1>", "Exit");

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
        private IEnumerator OnIdle(EnemyNormal owner)
        {
            /// 0 ~ MaxIdleTime 시간 동안 기다린 후, Wander 상태로 변경한다.
            float idleTime = Random.Range(owner.MinIdleTime, owner.MaxIdleTime);
            StartCoroutine(AutoChangeFromIdleToWander(idleTime, owner));
            
            while(true)
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
        private IEnumerator AutoChangeFromIdleToWander(float waitTime, EnemyNormal owner)
        {
            yield return new WaitForSeconds(waitTime);

            owner.ChangeState(EnemyNormalStates.Wander);
        }
    }

    public class Wander : StateBase<EnemyNormal>
    {
        public override void Enter(EnemyNormal owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander<Enemy1>", "Enter");

            /// 애니메이션 설정 후, Wander 상태를 실행한다.
            owner.AnimatorController.SetBool(EnemyNormalAnimParam.isMovement.ToString(), true);
            owner.NavMeshAgentController.speed = owner.Status.WalkSpeed;
            owner.NavMeshAgentController.stoppingDistance = 0;

            Execute(owner);
        }

        public override void Execute(EnemyNormal owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander<Enemy1>", "Execute");

            coroutine = StartCoroutine(OnWander(owner));
        }

        public override void Exit(EnemyNormal owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander<Enemy1>", "Exit");

            /// Wander 코루틴 정지
            if (coroutine != null)
                StopAllCoroutines();
            coroutine = null;

            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander<Enemy1>", $"coroutine is null: {coroutine == null}");

            /// 경로 초기화
            owner.NavMeshAgentController.ResetPath();
        }

        /// <summary>
        /// 실제 Wander 상태를 실행하는 메소드
        /// </summary>
        /// <param name="owner">상태를 가진 Enemy</param>
        /// <returns>코루틴</returns>
        private IEnumerator OnWander(EnemyNormal owner)
        {
            Vector3 wanderPosition = owner.CalculateWanderPosition();
            owner.NavMeshAgentController.SetDestination(wanderPosition);

            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander<Enemy1>", $"wanderPosition: {wanderPosition}");

            Vector3 to = new Vector3(owner.NavMeshAgentController.destination.x, 0, owner.NavMeshAgentController.destination.z);
            Vector3 from = new Vector3(owner.transform.position.x, 0, owner.transform.position.z);
            owner.transform.rotation = Quaternion.LookRotation(to - from);

            float currentTime = 0;
            float maxTime = 10f;
            while(true)
            {
                currentTime += Time.deltaTime;
            
                to = new Vector3(owner.NavMeshAgentController.destination.x, 0, owner.NavMeshAgentController.destination.z);
                from = new Vector3(owner.transform.position.x, 0, owner.transform.position.z);
            
                /// 목적지까지의 근처에 접근했거나 너무 오랜시간 동안 
                /// 'Wander' 상태에 머물러 있으면 'Idle' 상태로 변경한다.
                if((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
                {
                    owner.ChangeState(EnemyNormalStates.Idle);
                }
            
                /// 타겟과의 거리에 따라 상태를 선택한다.
                owner.CalculateDistanceToTargetAndSelectState();
            
                yield return null;
            }
        }
    }

    public class Pursuit : StateBase<EnemyNormal>
    {
        public override void Enter(EnemyNormal owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Pursuit<Enemy1>", "Enter");

            /// 애니메이션 파라미터, 이동 속도를 설정한 후, Pursuit 상태 실행
            owner.AnimatorController.SetBool(EnemyNormalAnimParam.isMovement.ToString(), true);
            owner.NavMeshAgentController.speed = owner.Status.RunSpeed;

            Execute(owner);
        }

        public override void Execute(EnemyNormal owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Pursuit<Enemy1>", "Execute");

            coroutine = StartCoroutine(OnPursuit(owner));
        }

        public override void Exit(EnemyNormal owner)
        {
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Pursuit<Enemy1>", "Exit");

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
        private IEnumerator OnPursuit(EnemyNormal owner)
        {
            while(true)
            {
                /// 개체의 속도를 RunSpeed로 바꾸고 타겟의 위치로 경로를 갱신한다.
                owner.NavMeshAgentController.SetDestination(owner.Target.position);
                owner.LookRotationToTarget();

                owner.CalculateDistanceToTargetAndSelectState();

                yield return null;
            }
        }
    }

    public class Attack : StateBase<EnemyNormal>
    {
        private float lastAttackTime = 0;
        private ProjectileBase missile;

        public ProjectileBase Missile => missile;

        public override void Enter(EnemyNormal owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Attack<Enemy1>", "Enter");

            /// "공격" 상태 진입시, 바로 멈추며
            /// 미사일 발사를 위해 일정 거리를 둘 수 있게 NavMeshAgent의 정지 거리를 설정
            owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Stop);
            owner.NavMeshAgentController.stoppingDistance = owner.AttackDistance - 2f;

            Execute(owner);
        }

        public override void Execute(EnemyNormal owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Attack<Enemy1>", "Execute");

            /// Attack 코루틴 실행
            coroutine = StartCoroutine(OnAttack(owner));
        }

        public override void Exit(EnemyNormal owner)
        {
            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Attack<Enemy1>", "Exit");

            /// Attack 코루틴 정지
            if (coroutine != null)
                StopAllCoroutines();
            coroutine = null;

            owner.AnimatorController.SetBool(EnemyNormalAnimParam.isAttack.ToString(), false);
            owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Move);

            if (IsBeforeMissileFired())
                missile.Stop();
        }

        /// <summary>
        /// 실제 Attack 상태를 실행하는 메소드
        /// </summary>
        /// <param name="owner">상태를 가진 Enemy</param>
        /// <returns>코루틴</returns>
        private IEnumerator OnAttack(EnemyNormal owner)
        {
            WaitForSeconds attackDelay = new WaitForSeconds(0.5f);
            WaitUntil attackEndWait = new WaitUntil(() => owner.AnimatorController.CurrentAnimNormalizedTime() >= 0.9f);

            while (true)
            {
                owner.LookRotationToTarget();
                owner.CalculateDistanceToTargetAndSelectState();

                /// 현재 시간과 마지막 공격 시간의 차이가 Enemy 공격 주기 이상이라면
                if(Time.time - lastAttackTime >= owner.AttackRate && owner.CurrentState == EnemyNormalStates.Attack)
                {
                    /// 공격 전 설정 변경
                    lastAttackTime = Time.time;
                    //owner.CanChangeState = false;
                    owner.SetCanChangeState(false);
                    owner.AnimatorController.SetBool(EnemyNormalAnimParam.isAttack.ToString(), true);

                    missile = EnemyProjectileManager.Instance.GetEnemyNormalMissile(
                        owner.ProjectileSpawnPoint.position, owner.ProjectileSpawnPoint.rotation
                    );
                    yield return attackDelay;
                    
                    missile.Fire(owner.Target.position);

                    /// 공격 후 설정 변경
                    yield return attackEndWait;
                    //owner.CanChangeState = true;
                    owner.SetCanChangeState(true);
                    owner.AnimatorController.SetBool(EnemyNormalAnimParam.isAttack.ToString(), false);
                }

                yield return null;
            }
        }

        public bool IsBeforeMissileFired()
        {
            return missile != null && missile.IsFired == false;
        }
    }
}