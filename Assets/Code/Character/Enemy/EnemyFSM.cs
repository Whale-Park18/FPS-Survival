using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

using WhalePark18.Item;
using WhalePark18.Manager;
using WhalePark18.Projectile;

namespace WhalePark18.Character.Enemy
{
    public enum EnemyState { None = -1, Idle = 0, Wander, Pursuit, Attack, }

    public class EnemyFSM : MonoBehaviour
    {
        [Header("Pursuit")]
        [SerializeField]
        private float targetRecognitionRange = 8f;          // 인식 범위(이 범위 안에 들어오면 "Pursuit" 상태로 변경
        [SerializeField]
        private float pursuitLimitRange = 10f;              // 추적 범위(이 범위 밖으로 나가면 "Wander" 상태로 변경

        [Header("Attack")]
        [SerializeField]
        private GameObject projectilePrefab;                // 발사체 프리팹
        [SerializeField]
        private Transform projectileSpawnPoint;             // 발사체 생성 위치
        [SerializeField]
        private float attackRange = 5f;                     // 공격 범위(이 범위 안에 들어오면 "Attack"상태로 변경)
        [SerializeField]
        private float attackRate = 1f;                      // 공격 속도

        private EnemyState enemyState = EnemyState.None;    // 현재 적 행동
        private float lastAttackTime = 0;

        private EnemyStatus status;                              // 이동속도 등의 정보
        private NavMeshAgent navMeshAgent;                  // 이동 제어를 위한 NavMeshAgent
        private Transform target;                           // 적의 공격 대상(플레이어)

        private void Awake()
        {
            status = GetComponent<EnemyStatus>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void Setup(Transform target)
        {
            this.target = target;

            /// NavMeshAgent 컴포넌트에서 회전을 업데이트하지 않도록 설정
            navMeshAgent.updateRotation = false;
        }

        public void Setup(Transform target, EnemyMemoryPool enemyMemoryPool)
        {
            this.target = target;
            //this.enemyMemoryPool = enemyMemoryPool;

            /// NavMeshAgent 컴포넌트에서 회전을 업데이트하지 않도록 설정
            navMeshAgent.updateRotation = false;
        }

        private void OnEnable()
        {
            /// 적이 활성화될 떄 적의 상태를 "대기"로 설정
            ChangeState(EnemyState.Idle);
        }

        private void OnDisable()
        {
            /// 적이 비활성화될 때 현재 재생중인 상태를 종료하고, 상태를 "None"으로 설정
            StopCoroutine(enemyState.ToString());

            enemyState = EnemyState.None;
        }

        /// <summary>
        /// 상태를 변경 인터페이스
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(EnemyState newState)
        {
            if (enemyState == newState) return;

            /// 이전에 재생중이던 상태를 종료하고
            /// 현재 적의 상태를 newState로 설정한 후, 상태 재생
            StopCoroutine(enemyState.ToString());
            enemyState = newState;
            StartCoroutine(enemyState.ToString());
        }

        /// <summary>
        /// 대기 상태 메소드
        /// </summary>
        /// <returns></returns>
        private IEnumerator Idle()
        {
            /// n초 후에 "배회" 상태로 변경하는 코루틴 실행
            StartCoroutine("AutoChangeFormIdleToWander");

            while (true)
            {
                /// "대기" 상태일 때 하는 행동
                /// 타겟과의 거리에 따라 행동 선택(배회, 추적, 원거리 공격)
                CalculateDistanceToTargetAndSelectState();

                yield return null;
            }
        }

        /// <summary>
        /// 자동으로 대기 상태에서 탐색 상태로 전환하는 메소드
        /// </summary>
        /// <returns></returns>
        private IEnumerator AutoChangeFormIdleToWander()
        {
            /// 1~4초 대기 후,
            /// 상태를 "배회"로 변경
            int changeTime = Random.Range(1, 5);
            yield return new WaitForSeconds(changeTime);
            ChangeState(EnemyState.Wander);
        }

        /// <summary>
        /// 탐색 상태 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator Wander()
        {
            float currentTime = 0;
            float maxTime = 10;

            /// 이동 속도 설정
            navMeshAgent.speed = status.WalkSpeed;

            /// 목표 위치로 설정
            navMeshAgent.SetDestination(CalculateWanderPosition());

            /// 목표 위치로 회전
            Vector3 to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
            Vector3 form = new Vector3(transform.position.x, 0, transform.position.z);
            transform.rotation = Quaternion.LookRotation(to - form);

            while (true)
            {
                currentTime += Time.deltaTime;

                /// 목표 위치에 근접하게 도달하거나 너무 오랜시간동안 배회하기 상태에 머물러 있으면
                to = new Vector3(navMeshAgent.destination.x, 0, navMeshAgent.destination.z);
                form = new Vector3(transform.position.x, 0, transform.position.z);

                if ((to - form).sqrMagnitude < 0.01f || currentTime >= maxTime)
                {
                    /// 상태를 "대기"로 변경
                    ChangeState(EnemyState.Idle);
                }

                /// 타겟과의 거리에 따라 행동 선택(배회, 추적, 원거리 공격)
                CalculateDistanceToTargetAndSelectState();

                yield return null;
            }
        }

        /// <summary>
        /// 탐색할 위치를 계산하는 메소드
        /// </summary>
        /// <returns>탐색 위치</returns>
        private Vector3 CalculateWanderPosition()
        {
            float wanderRadius = 10f;   // 현재 위치를 원점으로 하는 원의 반지름
            int wanderJitter = 0;       // 선택된 각도 (wanderJitterMin ~ wanderJitterMax)
            int wanderJitterMin = 0;    // 최소 각도
            int wanderJitterMax = 360;  // 최대 각도

            /// 현재 적 캐릭터가 있는 월드의 중심 위치와 크기(구역을 벗어난 행동을 하지 않도록)
            Vector3 rangePosition = Vector3.zero;
            Vector3 rangeScale = Vector3.one * 100f;

            /// 자신의 위치를 중심으로 반지금(wanderRadius) 거리,
            /// 선택된 각도(wanderJitter)에 위치한 좌표를 목표지점으로 설정
            wanderJitter = Random.Range(wanderJitterMin, wanderJitterMax);
            Vector3 targetPosition = transform.position + SetAngle(wanderRadius, wanderJitter);

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
        private Vector3 SetAngle(float radius, int angle)
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Cos(angle) * radius;
            position.z = Mathf.Sin(angle) * radius;

            return position;
        }

        /// <summary>
        /// 추적 상태 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator Pursuit()
        {
            while (true)
            {
                /// 이동 속도 설정(배회할 때는 걷는 속도, 추적할 때는 달리기 속도)
                /// 목표 위치를 현재 플레이어 위치로 설정
                /// 타겟 방향을 계속 주시하도록함
                navMeshAgent.speed = status.RunSpeed;
                navMeshAgent.SetDestination(target.position);
                LookRotationToTarget();

                /// 타겟과의 거리에 따라 행동 선택(배회, 추적, 원거리 공격)
                CalculateDistanceToTargetAndSelectState();

                yield return null;
            }
        }

        /// <summary>
        /// 공격 상태 메소드
        /// </summary>
        /// <returns>코루틴</returns>
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

        /// <summary>
        /// 타겟을 향해 바라보게 회전시키는 메소드
        /// </summary>
        private void LookRotationToTarget()
        {
            Vector3 to = new Vector3(target.position.x, 0, target.position.z);
            Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);

            /// 바로 회전
            transform.rotation = Quaternion.LookRotation(to - from);
            /// 서서히 회전
            //rotation = Quaternion.LookRotation(to - from);
            //rotation = Quaternion.Slerp(transform.rotation, rotation, 0.01f);
        }

        /// <summary>
        /// 타겟과의 거리와 선택할 상태를 계산하는 메소드
        /// </summary>
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
            /// "배회" 상태일 떄 이동할 경로 표시
            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position, navMeshAgent.destination - transform.position);

            /// 목표 인식 범위
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, targetRecognitionRange);

            /// 추적 범위
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, pursuitLimitRange);

            Gizmos.color = new Color(0.39f, 0.04f, 0.04f);
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        public void TakeDamage(int damage)
        {
            UnityEngine.Debug.LogFormat("<color=green>{0}\n피해량:</color> {1}", MethodBase.GetCurrentMethod().Name, damage);

            bool isDie = status.DecreaseHp(damage);

            if (isDie == true)
            {
                GameObject createItem = ItemManager.Instance.GetRandomItem();
                if(createItem != null)
                {
                    Instantiate(createItem, transform.position, transform.rotation);
                }

                EnemyManager.Instance.ReturnEnemy(this);
            }
        }
    }
}