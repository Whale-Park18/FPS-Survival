using System.Collections;
using UnityEngine;
using UnityEngine.AI;

using WhalePark18.MemoryPool;
using WhalePark18.Projectile;

namespace WhalePark18.Character.Enemy
{
    public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack, }

    public class EnemyFSM : MonoBehaviour
    {
        [Header("Pursuit")]
        [SerializeField]
        private float targetRecognitionRange = 8f;    // �ν� ����(�� ���� �ȿ� ������ "Pursuit" ���·� ����
        [SerializeField]
        private float pursuitLimitRange = 10f;          // ���� ����(�� ���� ������ ������ "Wander" ���·� ����

        [Header("Attack")]
        [SerializeField]
        private GameObject projectilePrefab;       // �߻�ü ������
        [SerializeField]
        private Transform projectileSpawnPoint;   // �߻�ü ���� ��ġ
        [SerializeField]
        private float attackRange = 5f;       // ���� ����(�� ���� �ȿ� ������ "Attack"���·� ����)
        [SerializeField]
        private float attackRate = 1f;        // ���� �ӵ�

        private EnemyState enemyState = EnemyState.None;   // ���� �� �ൿ
        private float lastAttackTime = 0;

        private Status status;                         // �̵��ӵ� ���� ����
        private NavMeshAgent navMeshAgent;                   // �̵� ��� ���� NavMeshAgent
        private Transform target;                         // ���� ���� ���(�÷��̾�)
        private EnemyMemoryPool enemyMemoryPool;                // �� �޸� Ǯ(�� ������Ʈ ��Ȱ��ȭ�� ���)

        public void Setup(Transform target, EnemyMemoryPool enemyMemoryPool)
        {
            status = GetComponent<Status>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            this.target = target;
            this.enemyMemoryPool = enemyMemoryPool;

            /// NavMeshAgent ������Ʈ���� ȸ���� ������Ʈ���� �ʵ��� ����
            navMeshAgent.updateRotation = false;
        }

        private void OnEnable()
        {
            /// ���� Ȱ��ȭ�� �� ���� ���¸� "���"�� ����
            ChangeState(EnemyState.Idle);
        }

        private void OnDisable()
        {
            /// ���� ��Ȱ��ȭ�� �� ���� ������� ���¸� �����ϰ�, ���¸� "None"���� ����
            StopCoroutine(enemyState.ToString());

            enemyState = EnemyState.None;
        }

        public void ChangeState(EnemyState newState)
        {
            if (enemyState == newState) return;

            /// ������ ������̴� ���¸� �����ϰ�
            /// ���� ���� ���¸� newState�� ������ ��, ���� ���
            StopCoroutine(enemyState.ToString());
            enemyState = newState;
            StartCoroutine(enemyState.ToString());
        }

        private IEnumerator Idle()
        {
            /// n�� �Ŀ� "��ȸ" ���·� �����ϴ� �ڷ�ƾ ����
            StartCoroutine("AutoChangeFormIdleToWander");

            while (true)
            {
                /// "���" ������ �� �ϴ� �ൿ
                /// Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����(��ȸ, ����, ���Ÿ� ����)
                CalculateDistanceToTargetAndSelectState();

                yield return null;
            }
        }

        private IEnumerator AutoChangeFormIdleToWander()
        {
            /// 1~4�� ��� ��,
            /// ���¸� "��ȸ"�� ����
            int changeTime = Random.Range(1, 5);
            yield return new WaitForSeconds(changeTime);
            ChangeState(EnemyState.Wander);
        }

        private IEnumerator Wander()
        {
            float currentTime = 0;
            float maxTime = 10;

            /// �̵� �ӵ� ����
            navMeshAgent.speed = status.WalkSpeed;

            /// ��ǥ ��ġ�� ����
            navMeshAgent.SetDestination(CalculateWanderPosition());

            /// ��ǥ ��ġ�� ȸ��
            Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            Vector3 form = new Vector3(transform.position.x, 0, transform.position.z);
            transform.rotation = Quaternion.LookRotation(to - form);

            while (true)
            {
                currentTime += Time.deltaTime;

                /// ��ǥ ��ġ�� �����ϰ� �����ϰų� �ʹ� �����ð����� ��ȸ�ϱ� ���¿� �ӹ��� ������
                to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
                form = new Vector3(transform.position.x, 0, transform.position.z);

                if ((to - form).sqrMagnitude < 0.01f || currentTime >= maxTime)
                {
                    /// ���¸� "���"�� ����
                    ChangeState(EnemyState.Idle);
                }

                /// Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����(��ȸ, ����, ���Ÿ� ����)
                CalculateDistanceToTargetAndSelectState();

                yield return null;
            }
        }

        private Vector3 CalculateWanderPosition()
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
            wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
            Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

            /// ������ ��ǥ��ġ�� �ڽ��� �̵������� ����� �ʰ� ����
            targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
            targetPosition.y = 0f;
            targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

            return targetPosition;
        }

        private Vector3 SetAngle(float radius, int angle)
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Cos(angle) * radius;
            position.z = Mathf.Sin(angle) * radius;

            return position;
        }

        private IEnumerator Pursuit()
        {
            while (true)
            {
                /// �̵� �ӵ� ����(��ȸ�� ���� �ȴ� �ӵ�, ������ ���� �޸��� �ӵ�)
                /// ��ǥ ��ġ�� ���� �÷��̾� ��ġ�� ����
                /// Ÿ�� ������ ��� �ֽ��ϵ�����
                navMeshAgent.speed = status.RunSpeed;
                navMeshAgent.SetDestination(target.position);
                LookRotationToTarget();

                /// Ÿ�ٰ��� �Ÿ��� ���� �ൿ ����(��ȸ, ����, ���Ÿ� ����)
                CalculateDistanceToTargetAndSelectState();

                yield return null;
            }
        }

        private IEnumerator Attack()
        {
            navMeshAgent.ResetPath();

            while (true)
            {
                LookRotationToTarget();

                CalculateDistanceToTargetAndSelectState();

                if (Time.time - lastAttackTime > attackRate)
                {
                    lastAttackTime = Time.time;

                    GameObject clone = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
                    clone.GetComponent<EnemyProjectile>().Setup(target.position);
                }

                yield return null;
            }
        }

        private void LookRotationToTarget()
        {
            Vector3 to = new Vector3(target.position.x, 0, target.position.z);
            Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

            /// �ٷ� ȸ��
            transform.rotation = Quaternion.LookRotation(to - from);
            /// ������ ȸ��
            //rotation = Quaternion.LookRotation(to - from);
            //rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
        }

        private void CalculateDistanceToTargetAndSelectState()
        {
            if (target == null)
            {
                return;
            }

            float distance = Vector3.Distance(target.position, transform.position);

            if (distance <= attackRange)
            {
                ChangeState(EnemyState.Attack);
            }
            else if (distance <= targetRecognitionRange)
            {
                ChangeState(EnemyState.Pursuit);
            }
            else if (distance >= pursuitLimitRange)
            {
                ChangeState(EnemyState.Wander);
            }
        }

        private void OnDrawGizmos()
        {
            /// "��ȸ" ������ �� �̵��� ��� ǥ��
            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);

            /// ��ǥ �ν� ����
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

            /// ���� ����
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);

            Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        public void TakeDamage(int damage)
        {
            bool isDie = status.DecreaseHP(damage);

            if (isDie == true)
            {
                enemyMemoryPool.DeactiveEnemy(gameObject);
            }
        }
    }
}