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

            /// Idle �ڷ�ƾ ����
            if (coroutineHandle != null)
                StopAllCoroutines();
            coroutineHandle = null;
        }

        /// <summary>
        /// ���� Idle ���¸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="owner">���¸� ���� Enemy</param>
        /// <returns></returns>
        private IEnumerator OnIdle(EnemyElite owner)
        {
            /// 0 ~ MaxIdleTime �ð� ���� ��ٸ� ��, Wander ���·� �����Ѵ�.
            float idleTime = Random.Range(1, owner.MaxIdleTime);
            StartCoroutine(AutoChangeFromIdleToWander(idleTime, owner));

            while (true)
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

            /// Wander �ڷ�ƾ ����
            if (coroutineHandle != null)
                StopAllCoroutines();
            coroutineHandle = null;

            // #DEBUG
            LogManager.ConsoleDebugLog($"[{gameObject.transform.parent.name}] Wander", $"coroutine is null: {coroutineHandle == null}");

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

            /// Pursuit �ڷ�ƾ ����
            if (coroutineHandle != null)
                StopAllCoroutines();
            coroutineHandle = null;

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
            LogManager.ConsoleDebugLog($"{name} Attack", "Part. 1 ���� �� ���");
            {
                owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Stop);
                owner.AnimatorController.SetFloat(EnemyEliteAnimParam.Speed.ToString(), 0);
                yield return _waitStopBeforeAttack;
            }
            
            LogManager.ConsoleDebugLog($"{name} Attack", "Part. 2 ����");
            {
                owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Move);
                owner.AnimatorController.SetBool(EnemyEliteAnimParam.isAttack.ToString(), true);
                yield return StartCoroutine(OnMoveToDestination(owner));
                owner.NavMeshAgentController.ChangeState(EnemyNavMeshAgentStates.Stop, BreakMode.SmoothStop);
            }
            
            LogManager.ConsoleDebugLog($"{name} Attack", "Part. 3 ���� �� ���");
            {
                owner.AnimatorController.SetBool(EnemyEliteAnimParam.isAttack.ToString(), false);
                yield return _waitStopAfterAttack;
            }
            
            LogManager.ConsoleDebugLog($"{name} Attack", "Part. 4 ���� ����");
            {
                owner.SetCanChangeState(true);
                owner.CalculateDistanceToTargetAndSelectState();
            }
        }

        /// <summary>
        /// ���������� �̵��ϴ� �޼ҵ�
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