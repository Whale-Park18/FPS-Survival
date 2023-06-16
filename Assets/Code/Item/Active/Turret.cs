using System.Collections;
using UnityEngine;
using WhalePark18.Manager;
using WhalePark18.Projectile;

namespace WhalePark18.Item.Active
{
    public enum TurretState { None = -1, Guard = 0, Attack }

    public class Turret : MonoBehaviour
    {
        [Header("Turret")]
        [SerializeField]
        private Transform turretHead;               // 터렛 헤드
        [SerializeField]
        private float searchRadius = 15f;           // 탐색 반경
        [SerializeField]
        private float searchDelay = 0.5f;           // 탐색 딜레이
        [SerializeField]
        LayerMask layerMask;                        // 탐색 레이어 마스크
        [SerializeField]
        private float guardRotationSpeed = 45f;     // 경계 회전 속도
        [SerializeField]
        private float destroyTime;                  // 파괴 시간

        [Header("Attack")]
        [SerializeField]
        private float lockOnTime = 0.8f;            // 목표물 조준 완료 시간
        [SerializeField]
        GameObject bulletPrefab;                    // 총알 프리팹
        [SerializeField]
        private Transform bulletSpawnPoint;         // 총알 발사 위치
        [SerializeField]
        private float attackDelay = 0.5f;           // 공격 딜레이

        [Header("Audio Clip")]
        [SerializeField]
        private AudioClip buildClip;
        [SerializeField]
        private AudioClip shootCilp;

        private TurretState turretState = TurretState.None; // 포탑 행동 상태
        private Transform target;                           // 포탑 목표

        private AudioData audioData;

        public float DestroyTime
        {
            set => destroyTime = value;
        }

        private void Awake()
        {
            audioData = new AudioData(GetComponent<AudioSource>(), Manager.AudioType.Item);
        }

        private void Start()
        {
            SoundManager.Instance.AddAudioSource(audioData);

            AudioPlay(buildClip);
            StartCoroutine("SearchEnemy");
            StartCoroutine("AutoDestroyByTime", destroyTime);
        }

        /// <summary>
        /// 적 탐색 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator SearchEnemy()
        {
            while(true)
            {
                /// turretHead를 중심으로 searchRadius 반지름의 구형 범위 내에 Enemy 레이어 게임 오브젝트 탐색
                Collider[] colliders = Physics.OverlapSphere(turretHead.position, searchRadius, layerMask);
                Transform shortestEnemy = null;

                /// 탐색된 게임 오브젝트가 있다면
                /// 그 중에서 터렛과 가장 거리가 가까운 적 게임 오브젝트를 선정
                if(colliders.Length > 0)
                {
                    float shortestDistance = Mathf.Infinity;
                    foreach(Collider collider in colliders)
                    {
                        float distance = Vector3.SqrMagnitude(transform.position - collider.transform.position);
                        if(distance < shortestDistance)
                        {
                            shortestDistance = distance;
                            shortestEnemy = collider.transform;
                        }
                    }
                }

                /// 터렛과 거리가 가장 가까운 적을 타겟으로 설정
                target = shortestEnemy;

                /// 만약 설정된 적이 없다면 터렛의 행동을 경계(Guard)로 변경
                /// 설정된 적이 있다면 터렛의 행동을 공격으로 변경
                if (target == null) 
                    ChangeState(TurretState.Guard);
                else
                    ChangeState(TurretState.Attack);

                /// searchDelay 초마다 적 탐색
                yield return new WaitForSeconds(searchDelay);
            }
        }

        /// <summary>
        /// 행동 변경 메소드
        /// </summary>
        /// <param name="newState">변경할 행동</param>
        private void ChangeState(TurretState newState)
        {
            if(turretState == newState) return;

            StopCoroutine(turretState.ToString());
            turretState = newState;
            StartCoroutine(turretState.ToString());
        }

        /// <summary>
        /// 경계 행동 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator Guard()
        {
            /// guardRotationSpeed 만큼 매 프레임마다 y축을 중심으로 회전
            while (true)
            {
                turretHead.transform.Rotate(new Vector3(0f, guardRotationSpeed, 0f) * Time.deltaTime);

                yield return null;
            }
        }

        /// <summary>
        /// 공격 행동 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator Attack()
        {
            /// 처음 목표를 바라볼 때, 자연스러움을 주기 위해
            yield return StartCoroutine("LookAt");

            float lastAttackTime = Time.deltaTime;
            while(true)
            {
                Vector3 targetPosition = target.position;
                targetPosition.y = turretHead.position.y;

                /// 공격 딜레이
                /// 마지막 공격 시간과 현재 시간의 차이로 공격 딜레이를 확인함
                if(Time.time - lastAttackTime > attackDelay)
                {
                    GameObject clone = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                    clone.GetComponent<ProjectileBase>().Fire(targetPosition);

                    AudioPlay(shootCilp);

                    lastAttackTime = Time.time;
                }

                /// 공격 상태일 때, 계속 타겟을 바라봄
                turretHead.LookAt(targetPosition);

                yield return null;
            }
        }

        private IEnumerator LookAt()
        {
            Quaternion startRoatation = turretHead.rotation;
            for (float currentTime = 0f, percent = 0f ; currentTime < lockOnTime; currentTime += Time.deltaTime, percent = currentTime / lockOnTime)
            {
                Vector3 relativePosition = target.position - transform.position;
                relativePosition.y = transform.position.y;
                turretHead.rotation = Quaternion.Slerp(startRoatation, Quaternion.LookRotation(relativePosition), percent);
                yield return null;
            }
        }

        private void AudioPlay(AudioClip clip)
        {
            audioData.audioSource.clip = clip;
            audioData.audioSource.Play();
        }

        private IEnumerator AutoDestroyByTime(float duration)
        {
            yield return new WaitForSeconds(duration);

            SoundManager.Instance.RemoveAudioSource(audioData);
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(turretHead.position, searchRadius);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(bulletSpawnPoint.position, bulletSpawnPoint.forward * 15f);
        }
    }
}
