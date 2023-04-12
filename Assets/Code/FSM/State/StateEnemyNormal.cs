using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

using WhalePark18.Manager;
using WhalePark18.Projectile;
using WhalePark18.Character.Enemy;

namespace WhalePark18.FSM.State.EnemyNoramlState
{
    public class Idle : StateBase<EnemyNormal>
    {
        public override void Enter(EnemyNormal owner)
        {
            // DEBUG
            LogManager.ConsoleDebugLog("Idle<Enemy1>", "Enter");

            /// �ִϸ��̼� ���� ��, Idle ���¸� �����Ѵ�.
            owner.AnimatorController.SetBool(EnemyNormalAnimParam.isMovement.ToString(), false);

            Execute(owner);
        }

        public override void Execute(EnemyNormal owner)
        {
            // DEBUG
            LogManager.ConsoleDebugLog("Idle<Enemy1>", "Execute");

            /// Idle �ڷ�ƾ ����
            coroutine = StartCoroutine(OnIdle(owner));
        }

        public override void Exit(EnemyNormal owner)
        {
            // DEBUG
            LogManager.ConsoleDebugLog("Idle<Enemy1>", "Exit");

            /// Idle �ڷ�ƾ ����
            if (coroutine != null)
                StopAllCoroutines();
            coroutine = null;
        }

        /// <summary>
        /// ���� Idle ���¸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="owner">���¸� ���� Enemy</param>
        /// <returns></returns>
        private IEnumerator OnIdle(EnemyNormal owner)
        {
            /// 0 ~ MaxIdleTime �ð� ���� ��ٸ� ��, Wander ���·� �����Ѵ�.
            float idleTime = Random.Range(1, owner.MaxIdleTime);
            StartCoroutine(AutoChangeFromIdleToWander(idleTime, owner));
            
            while(true)
            {
                owner.CalculateDistanceToTargetAndSelectState();

                yield return null;
            }
        }

        /// <summary>
        /// ��� ��, ��Ȳ ���·� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="waitTime">��� �ð�</param>
        /// <param name="owner">���¸� ���� Enemy</param>
        /// <returns>�ڷ�ƾ</returns>
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
            // DEBUG
            LogManager.ConsoleDebugLog("Wander<Enemy1>", "Enter");

            /// �ִϸ��̼� ���� ��, Wander ���¸� �����Ѵ�.
            owner.AnimatorController.SetBool(EnemyNormalAnimParam.isMovement.ToString(), true);
            owner.NavMeshAgentController.speed = owner.Status.WalkSpeed;

            Execute(owner);
        }

        public override void Execute(EnemyNormal owner)
        {
            // DEBUG
            LogManager.ConsoleDebugLog("Wander<Enemy1>", "Execute");

            coroutine = StartCoroutine(OnWander(owner));
        }

        public override void Exit(EnemyNormal owner)
        {
            // DEBUG
            LogManager.ConsoleDebugLog("Wander<Enemy1>", "Exit");

            /// Wander �ڷ�ƾ ����
            if (coroutine != null)
                StopAllCoroutines();
            coroutine = null;

            // DEBUG
            LogManager.ConsoleDebugLog("Wander<Enemy1>", $"coroutine is null: {coroutine == null}");

            /// ��� �ʱ�ȭ
            owner.NavMeshAgentController.ResetPath();
        }

        /// <summary>
        /// ���� Wander ���¸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="owner">���¸� ���� Enemy</param>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator OnWander(EnemyNormal owner)
        {
            Vector3 wanderPosition = owner.CalculateWanderPosition();
            owner.NavMeshAgentController.SetDestination(wanderPosition);

            // DEBUG
            LogManager.ConsoleDebugLog("Wander<Enemy1>", $"wanderPosition: {wanderPosition}");

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
            
                /// ������������ ��ó�� �����߰ų� �ʹ� �����ð� ���� 
                /// 'Wander' ���¿� �ӹ��� ������ 'Idle' ���·� �����Ѵ�.
                if((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
                {
                    owner.ChangeState(EnemyNormalStates.Idle);
                }
            
                /// Ÿ�ٰ��� �Ÿ��� ���� ���¸� �����Ѵ�.
                owner.CalculateDistanceToTargetAndSelectState();
            
                yield return null;
            }
        }
    }

    public class Pursuit : StateBase<EnemyNormal>
    {
        public override void Enter(EnemyNormal owner)
        {
            // DEBUG
            LogManager.ConsoleDebugLog("Pursuit<Enemy1>", "Enter");

            /// �ִϸ��̼� �Ķ����, �̵� �ӵ��� ������ ��, Pursuit ���� ����
            owner.AnimatorController.SetBool(EnemyNormalAnimParam.isMovement.ToString(), true);
            owner.NavMeshAgentController.speed = owner.Status.RunSpeed;

            Execute(owner);
        }

        public override void Execute(EnemyNormal owner)
        {
            // DEBUG
            LogManager.ConsoleDebugLog("Pursuit<Enemy1>", "Execute");

            coroutine = StartCoroutine(OnPursuit(owner));
        }

        public override void Exit(EnemyNormal owner)
        {
            LogManager.ConsoleDebugLog("Pursuit<Enemy1>", "Exit");

            /// Pursuit �ڷ�ƾ ����
            if (coroutine != null)
                StopAllCoroutines();
            coroutine = null;

            /// ��� �ʱ�ȭ
            owner.NavMeshAgentController.ResetPath();
            owner.NavMeshAgentController.speed = owner.Status.WalkSpeed;
        }

        /// <summary>
        /// ���� Pursuit ���¸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="owner">���¸� ���� Enemy</param>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator OnPursuit(EnemyNormal owner)
        {
            while(true)
            {
                /// ��ü�� �ӵ��� RunSpeed�� �ٲٰ� Ÿ���� ��ġ�� ��θ� �����Ѵ�.
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

        public override void Enter(EnemyNormal owner)
        {
            // DEBUG
            LogManager.ConsoleDebugLog("Attack<Enemy1>", "Enter");

            /// Attack ���� ���Խ� ����
            ///     * isMovement �ִϸ��̼� �Ķ���͸� false�� �����Ѵ�.
            ///       (Attack�� ������ ���¿��� ����Ǳ� ������ ���� �ֱ� ���̿� Walk �ִϸ��̼��� 
            ///       ������� �ʰ� �����Ѵ�.)
            ///     * Enemy�� NavMeshAgent�� ��带 Stop ���� �����Ѵ�.
            owner.AnimatorController.SetBool(EnemyNormalAnimParam.isMovement.ToString(), false);
            owner.NavMeshAgentController.StopMode(BreakMode.SuddenStop);

            Execute(owner);
        }

        public override void Execute(EnemyNormal owner)
        {
            // DEBUG
            LogManager.ConsoleDebugLog("Attack<Enemy1>", "Execute");

            /// Attack �ڷ�ƾ ����
            coroutine = StartCoroutine(OnAttack(owner));
        }

        public override void Exit(EnemyNormal owner)
        {
            // DEBUG
            LogManager.ConsoleDebugLog("Attack<Enemy1>", "Exit");

            /// Attack �ڷ�ƾ ����
            if (coroutine != null)
                StopAllCoroutines();
            coroutine = null;

            /// Attck ���� ���� �� ����
            ///     * isAttack �ִϸ��̼� �Ķ���͸� false�� �����Ѵ�.
            ///       (Attack �ִϸ��̼��� isMovement�� ���� ��� ���� isAttack ���� true�� ��,
            ///       ������ ����Ǳ� ������ ��Ȱ��ȭ ��Ų��.)
            ///     * Enemy�� NavMeshAgent�� ��带 Move ���� �����Ѵ�.
            owner.AnimatorController.SetBool(EnemyNormalAnimParam.isAttack.ToString(), false);
            owner.NavMeshAgentController.MoveMode();
            //owner.CanChangeState = true;
            //owner.SetCanChangeState(true);
        }

        /// <summary>
        /// ���� Wander ���¸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="owner">���¸� ���� Enemy</param>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator OnAttack(EnemyNormal owner)
        {
            WaitForSeconds attackDelay = new WaitForSeconds(0.5f);
            WaitUntil attackEndWait = new WaitUntil(() => owner.AnimatorController.CurrentAnimNormalizedTime() >= 0.9f);

            while (true)
            {
                owner.LookRotationToTarget();
                owner.CalculateDistanceToTargetAndSelectState();

                /// ���� �ð��� ������ ���� �ð��� ���̰� Enemy ���� �ֱ� �̻��̶��
                if(Time.time - lastAttackTime >= owner.AttackRate && owner.CurrentState == EnemyNormalStates.Attack)
                {
                    /// ���� �� ���� ����
                    lastAttackTime = Time.time;
                    //owner.CanChangeState = false;
                    owner.SetCanChangeState(false);
                    owner.AnimatorController.SetBool(EnemyNormalAnimParam.isAttack.ToString(), true);

                    ProjectileBase projectile = EnemyProjectileManager.Instance.GetEnemyNormalMissile(
                        owner.ProjectileSpawnPoint.position, owner.ProjectileSpawnPoint.rotation
                    );
                    yield return attackDelay;
                    
                    projectile.Fire(owner.Target.position);

                    /// ���� �� ���� ����
                    yield return attackEndWait;
                    //owner.CanChangeState = true;
                    owner.SetCanChangeState(true);
                    owner.AnimatorController.SetBool(EnemyNormalAnimParam.isAttack.ToString(), false);
                }

                yield return null;
            }
        }
    }
}