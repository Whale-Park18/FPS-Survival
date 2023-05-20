using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using WhalePark18.Item;
using WhalePark18.Manager;

namespace WhalePark18.Character.Enemy
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BoxCollider), typeof(EnemyStatus))]
    [RequireComponent(typeof(EnemyAnimatorController), typeof(EnemyNavMeshAgentController))]
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Enemy Info")]
        private int                             id;
        [SerializeField]
        private string                          baseName;
        [SerializeField]
        protected EnemyClass                    enemyClass;             // Enemy ���
        [SerializeField]
        protected Transform                     target;                 // ���� Ÿ��

        protected EnemyStatus                   status;
        protected EnemyAnimatorController       animatorController;
        protected EnemyNavMeshAgentController   navMeshAgentController;

        [Header("Idle")]
        [SerializeField]
        protected float     minIdleTime = 1f;
        [SerializeField]
        protected float     maxIdleTime = 5f;

        [Header("Wander")]
        [SerializeField] 
        protected float     wanderDistance = 10f;   // ��Ȳ �Ÿ�

        [Header("Pursuit")]
        [SerializeField]
        protected float     pursuitDistance = 8f;   // ���� �Ÿ�



        /****************************************
         * ������Ƽ
         ****************************************/
        public int ID => id;
        public EnemyClass Class => enemyClass;

        public EnemyStatus Status => status;
        public EnemyAnimatorController AnimatorController => animatorController;
        public EnemyNavMeshAgentController NavMeshAgentController => navMeshAgentController;

        public float MinIdleTime => minIdleTime;
        public float MaxIdleTime => maxIdleTime;

        public Transform Target
        {
            set { target = value; }
            get { return target; }
        }

        protected virtual void Awake()
        {
            OnVariableInitialized();
            OnStateInitialized();
        }

        /// <summary>
        /// ��� ���� �� ������Ʈ �ʱ�ȭ �޼ҵ�
        /// </summary>
        protected virtual void OnVariableInitialized()
        {
            status                  = GetComponent<EnemyStatus>();
            animatorController      = GetComponent<EnemyAnimatorController>();
            navMeshAgentController  = GetComponent<EnemyNavMeshAgentController>();
        }

        /// <summary>
        /// ���� �ʱ�ȭ �޼ҵ�
        /// </summary>
        protected abstract void OnStateInitialized();

        /// <summary>
        /// Enemy ���� ��, �ʱ�ȭ�ϴ� �޼ҵ�
        /// </summary>
        /// <param name="id">Enemy ID</param>
        public virtual void Setup(int id)
        {
            this.id = id;
            name = $"{baseName}_{id}";
        }

        public abstract void Run();

        public virtual void Reset()
        {
            status.Reset();
        }

        /// <summary>
        /// ���� ó�� �޼ҵ�
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            LogManager.ConsoleDebugLog($"{gameObject.name}.TakeDamge", $"���� �����: {status.Hp.currentAbility} / ���ط�: {damage} / �ǰ� �� �����: {status.Hp.currentAbility - damage}");
            bool isDie = status.DecreaseHp(damage);
            LogManager.ConsoleDebugLog($"{gameObject.name}", $"isDie: {isDie}");

            if (isDie)
            {
                Die();
            }
        }

        /// <summary>
        /// Enemy ����� ȣ��Ǵ� �޼ҵ�
        /// </summary>
        protected virtual void Die()
        {
            LogManager.ConsoleDebugLog($"{gameObject.name}", $"���");
            GameObject createItem = ItemManager.Instance.GetRandomItem();
            if (createItem != null)
            {
                Instantiate(createItem, transform.position, transform.rotation);
            }

            EnemyManager.Instance.ReturnEnemy(this);
        }

        /// <summary>
        /// Ÿ�ٰ��� �Ÿ��� ������ ���¸� ����ϴ� �޼ҵ�
        /// </summary>
        public abstract void CalculateDistanceToTargetAndSelectState();

        /// <summary>
        /// Ž���� ��ġ�� ����ϴ� �޼ҵ�
        /// </summary>
        /// <returns>Ž�� ��ġ</returns>
        public virtual Vector3 CalculateWanderPosition()
        {
            /// ���� �� ĳ���Ͱ� �ִ� ������ �߽� ��ġ�� ũ��(������ ��� �ൿ�� ���� �ʵ���)
            Vector3 rangePosition = Vector3.zero;
            Vector3 rangeScale = Vector3.one * 100f;

            /// �ڽ��� ��ġ�� �߽����� ������(wanderDistance) �Ÿ�,
            /// ���õ� ����(randomAngle)�� ��ġ�� ��ǥ�� ��ǥ�������� ����
            int randomAngle = Random.Range(0, 360);
            Vector3 targetPosition = transform.position + SetAngle(wanderDistance, randomAngle);

            /// ������ ��ǥ��ġ�� �ڽ��� �̵������� ����� �ʰ� ����
            targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
            targetPosition.y = 0f;
            targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

            return targetPosition;
        }

        /// <summary>
        /// ������ ���� ������ �Ÿ��� ��ġ�� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <param name="radius">������</param>
        /// <param name="angle">����</param>
        /// <returns>������ ���� ������ �Ÿ��� ��ġ</returns>
        public virtual Vector3 SetAngle(float radius, int angle)
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Cos(angle) * radius;
            position.z = Mathf.Sin(angle) * radius;

            return position;
        }

        /// <summary>
        /// Ÿ���� ���� �ٶ󺸰� ȸ����Ű�� �޼ҵ�
        /// </summary>
        public virtual void LookRotationToTarget()
        {
            Vector3 to = new Vector3(target.position.x, 0, target.position.z);
            Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

            /// �ٷ� ȸ��
            transform.rotation = Quaternion.LookRotation(to - from);
        }
    }
}