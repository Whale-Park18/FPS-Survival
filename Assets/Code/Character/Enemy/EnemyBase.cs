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
        protected EnemyClass                    enemyClass;             // Enemy 등급
        [SerializeField]
        protected Transform                     target;                 // 공격 타겟

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
        protected float     wanderDistance = 10f;   // 방황 거리

        [Header("Pursuit")]
        [SerializeField]
        protected float     pursuitDistance = 8f;   // 추적 거리



        /****************************************
         * 프로퍼티
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
        /// 멤버 변수 및 컴포넌트 초기화 메소드
        /// </summary>
        protected virtual void OnVariableInitialized()
        {
            status                  = GetComponent<EnemyStatus>();
            animatorController      = GetComponent<EnemyAnimatorController>();
            navMeshAgentController  = GetComponent<EnemyNavMeshAgentController>();
        }

        /// <summary>
        /// 상태 초기화 메소드
        /// </summary>
        protected abstract void OnStateInitialized();

        /// <summary>
        /// Enemy 생성 후, 초기화하는 메소드
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
        /// 피해 처리 메소드
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            LogManager.ConsoleDebugLog($"{gameObject.name}.TakeDamge", $"현재 생명력: {status.Hp.currentAbility} / 피해량: {damage} / 피격 후 생명력: {status.Hp.currentAbility - damage}");
            bool isDie = status.DecreaseHp(damage);
            LogManager.ConsoleDebugLog($"{gameObject.name}", $"isDie: {isDie}");

            if (isDie)
            {
                Die();
            }
        }

        /// <summary>
        /// Enemy 사망시 호출되는 메소드
        /// </summary>
        protected virtual void Die()
        {
            LogManager.ConsoleDebugLog($"{gameObject.name}", $"사망");
            GameObject createItem = ItemManager.Instance.GetRandomItem();
            if (createItem != null)
            {
                Instantiate(createItem, transform.position, transform.rotation);
            }

            EnemyManager.Instance.ReturnEnemy(this);
        }

        /// <summary>
        /// 타겟과의 거리와 선택할 상태를 계산하는 메소드
        /// </summary>
        public abstract void CalculateDistanceToTargetAndSelectState();

        /// <summary>
        /// 탐색할 위치를 계산하는 메소드
        /// </summary>
        /// <returns>탐색 위치</returns>
        public virtual Vector3 CalculateWanderPosition()
        {
            /// 현재 적 캐릭터가 있는 월드의 중심 위치와 크기(구역을 벗어난 행동을 하지 않도록)
            Vector3 rangePosition = Vector3.zero;
            Vector3 rangeScale = Vector3.one * 100f;

            /// 자신의 위치를 중심으로 반지름(wanderDistance) 거리,
            /// 선택된 각도(randomAngle)에 위치한 좌표를 목표지점으로 설정
            int randomAngle = Random.Range(0, 360);
            Vector3 targetPosition = transform.position + SetAngle(wanderDistance, randomAngle);

            /// 생성된 목표위치가 자신의 이동구역을 벗어나지 않게 조절
            targetPosition.x = Mathf.Clamp(targetPosition.x, rangePosition.x - rangeScale.x * 0.5f, rangePosition.x + rangeScale.x * 0.5f);
            targetPosition.y = 0f;
            targetPosition.z = Mathf.Clamp(targetPosition.z, rangePosition.z - rangeScale.z * 0.5f, rangePosition.z + rangeScale.z * 0.5f);

            return targetPosition;
        }

        /// <summary>
        /// 각도에 따른 반지름 거리의 위치를 반환하는 메소드
        /// </summary>
        /// <param name="radius">반지름</param>
        /// <param name="angle">각도</param>
        /// <returns>각도에 따른 반지름 거리의 위치</returns>
        public virtual Vector3 SetAngle(float radius, int angle)
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Cos(angle) * radius;
            position.z = Mathf.Sin(angle) * radius;

            return position;
        }

        /// <summary>
        /// 타겟을 향해 바라보게 회전시키는 메소드
        /// </summary>
        public virtual void LookRotationToTarget()
        {
            Vector3 to = new Vector3(target.position.x, 0, target.position.z);
            Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

            /// 바로 회전
            transform.rotation = Quaternion.LookRotation(to - from);
        }
    }
}