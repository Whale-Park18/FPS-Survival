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
        private Transform turretHead;               // �ͷ� ���
        [SerializeField]
        private float searchRadius = 15f;           // Ž�� �ݰ�
        [SerializeField]
        private float searchDelay = 0.5f;           // Ž�� ������
        [SerializeField]
        LayerMask layerMask;                        // Ž�� ���̾� ����ũ
        [SerializeField]
        private float guardRotationSpeed = 45f;     // ��� ȸ�� �ӵ�
        [SerializeField]
        private float destroyTime;                  // �ı� �ð�

        [Header("Attack")]
        [SerializeField]
        private float lockOnTime = 0.8f;            // ��ǥ�� ���� �Ϸ� �ð�
        [SerializeField]
        GameObject bulletPrefab;                    // �Ѿ� ������
        [SerializeField]
        private Transform bulletSpawnPoint;         // �Ѿ� �߻� ��ġ
        [SerializeField]
        private float attackDelay = 0.5f;           // ���� ������

        [Header("Audio Clip")]
        [SerializeField]
        private AudioClip buildClip;
        [SerializeField]
        private AudioClip shootCilp;

        private TurretState turretState = TurretState.None; // ��ž �ൿ ����
        private Transform target;                           // ��ž ��ǥ

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
        /// �� Ž�� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator SearchEnemy()
        {
            while(true)
            {
                /// turretHead�� �߽����� searchRadius �������� ���� ���� ���� Enemy ���̾� ���� ������Ʈ Ž��
                Collider[] colliders = Physics.OverlapSphere(turretHead.position, searchRadius, layerMask);
                Transform shortestEnemy = null;

                /// Ž���� ���� ������Ʈ�� �ִٸ�
                /// �� �߿��� �ͷ��� ���� �Ÿ��� ����� �� ���� ������Ʈ�� ����
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

                /// �ͷ��� �Ÿ��� ���� ����� ���� Ÿ������ ����
                target = shortestEnemy;

                /// ���� ������ ���� ���ٸ� �ͷ��� �ൿ�� ���(Guard)�� ����
                /// ������ ���� �ִٸ� �ͷ��� �ൿ�� �������� ����
                if (target == null) 
                    ChangeState(TurretState.Guard);
                else
                    ChangeState(TurretState.Attack);

                /// searchDelay �ʸ��� �� Ž��
                yield return new WaitForSeconds(searchDelay);
            }
        }

        /// <summary>
        /// �ൿ ���� �޼ҵ�
        /// </summary>
        /// <param name="newState">������ �ൿ</param>
        private void ChangeState(TurretState newState)
        {
            if(turretState == newState) return;

            StopCoroutine(turretState.ToString());
            turretState = newState;
            StartCoroutine(turretState.ToString());
        }

        /// <summary>
        /// ��� �ൿ �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator Guard()
        {
            /// guardRotationSpeed ��ŭ �� �����Ӹ��� y���� �߽����� ȸ��
            while (true)
            {
                turretHead.transform.Rotate(new Vector3(0f, guardRotationSpeed, 0f) * Time.deltaTime);

                yield return null;
            }
        }

        /// <summary>
        /// ���� �ൿ �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator Attack()
        {
            /// ó�� ��ǥ�� �ٶ� ��, �ڿ��������� �ֱ� ����
            yield return StartCoroutine("LookAt");

            float lastAttackTime = Time.deltaTime;
            while(true)
            {
                Vector3 targetPosition = target.position;
                targetPosition.y = turretHead.position.y;

                /// ���� ������
                /// ������ ���� �ð��� ���� �ð��� ���̷� ���� �����̸� Ȯ����
                if(Time.time - lastAttackTime > attackDelay)
                {
                    GameObject clone = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                    clone.GetComponent<ProjectileBase>().Fire(targetPosition);

                    AudioPlay(shootCilp);

                    lastAttackTime = Time.time;
                }

                /// ���� ������ ��, ��� Ÿ���� �ٶ�
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
