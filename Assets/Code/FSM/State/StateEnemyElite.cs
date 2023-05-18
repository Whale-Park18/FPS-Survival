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

            /// �ִϸ��̼� ���� ��, Idle ���¸� �����Ѵ�.
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
        private void OnIdle(EnemyElite owner)
        {
            /// 0 ~ MaxIdleTime �ð� ���� ��ٸ� ��, Wander ���·� �����Ѵ�.
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
        /// ��� ��, ��Ȳ ���·� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="waitTime">��� �ð�</param>
        /// <param name="owner">���¸� ���� Enemy</param>
        /// <returns>�ڷ�ƾ</returns>
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

            /// �ִϸ��̼� ���� ��, Wander ���¸� �����Ѵ�.
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

            /// Wander �ڷ�ƾ ����
            if (coroutine != null)
                StopAllCoroutines();
            coroutine = null;

            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander", $"coroutine is null: {coroutine == null}");

            /// ��� �ʱ�ȭ
            owner.NavMeshAgentController.ResetPath();
        }

        /// <summary>
        /// ���� Wander ���¸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="owner">���¸� ���� Enemy</param>
        /// <returns>�ڷ�ƾ</returns>
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

                /// ������������ ��ó�� �����߰ų� �ʹ� �����ð� ���� 
                /// 'Wander' ���¿� �ӹ��� ������ 'Idle' ���·� �����Ѵ�.
                if ((to - from).sqrMagnitude < 0.01f || currentTime >= maxTime)
                {
                    owner.ChangeState(EnemyEliteStates.Idle);
                }

                /// Ÿ�ٰ��� �Ÿ��� ���� ���¸� �����Ѵ�.
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

            /// �ִϸ��̼� �Ķ����, �̵� �ӵ��� ������ ��, Pursuit ���� ����
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
        private IEnumerator OnPursuit(EnemyElite owner)
        {
            while (true)
            {
                /// ��ü�� �ӵ��� RunSpeed�� �ٲٰ� Ÿ���� ��ġ�� ��θ� �����Ѵ�.
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
            /// NavMeshAgent ���¸� "Stop"���� ������ ��, stopTimeBeforeAttack�� ��ŭ ���
            LogManager.ConsoleDebugLog("Attack<Elite>", "���� �� ���");
            {
                owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Stop);
                owner.AnimatorController.SetBool(EnemyEliteAnimParam.isMovement.ToString(), false);
                yield return waitStopBeforeAttack;
            }


            /// "Attack" �ִϸ��̼� Ȱ��ȭ
            /// NavMeshAgent ���¿� �ӵ��� ������ ��, �������� ���� �������� �����Ѵ�.
            LogManager.ConsoleDebugLog("Attack<Elite>", "���� ��");
            {
                owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Move);
                owner.NavMeshAgentController.speed = owner.DashSpeed;
                owner.NavMeshAgentController.SetDestination(dashDestination);
                owner.AnimatorController.SetBool(EnemyEliteAnimParam.isAttack.ToString(), true);

                yield return StartCoroutine(OnMoveToDestination(owner));
            }

            /// stopTimeAfterAttack�� ��ŭ ��� ��, "Wander" ���·� ����
            LogManager.ConsoleDebugLog("Attack<Elite>", "���� �� ���");
            {
                owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Stop, BreakMode.SmoothStop);
                owner.NavMeshAgentController.speed = owner.Status.WalkSpeed;
                owner.AnimatorController.SetBool(EnemyEliteAnimParam.isAttack.ToString(), false);

                yield return waitStopAfterAttack;
            }

            LogManager.ConsoleDebugLog("Attack<Elite>", "���� ��ȯ");
            {
                owner.ChangeState(EnemyEliteStates.Wander);
            }
        }

        /// <summary>
        /// ���������� �̵��ϴ� �޼ҵ�
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