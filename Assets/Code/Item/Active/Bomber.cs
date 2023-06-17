using System.Collections;
using UnityEngine;
using WhalePark18.Manager;

namespace WhalePark18.Item.Active
{
    [RequireComponent(typeof(AudioSource))]
    public class Bomber : MonoBehaviour
    {
        [SerializeField]
        private float speed = 5f;           // 이동 속도;

        [SerializeField]
        private AudioClip[] engineClips;    // 제트 엔진 클립

        [SerializeField]
        private GameObject bombPrefab;      // 폭탄 프리팹
        private Transform target;           // 타겟

        private AudioData audioData;

        private void Awake()
        {
            audioData = new AudioData(GetComponent<AudioSource>(), Manager.AudioType.Item);
        }

        private void Start()
        {
            SoundManager.Instance.AddAudioSource(audioData);
        }

        /// <summary>
        /// Bomber 비행 인터페이스
        /// </summary>
        /// <param name="target">목표</param>
        public void Fly(Transform target)
        {
            this.target = target;
            LookAt(target.position);
            StartCoroutine("OnBomb");
        }

        /// <summary>
        /// Bomber가 target을 향해 바라보게 설정하는 메소드
        /// </summary>
        /// <param name="target"></param>
        private void LookAt(Vector3 target)
        {
            Vector3 relative = target - transform.position;
            float radian = Mathf.Atan2(relative.x, relative.z);
            float degree = radian * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, degree, transform.eulerAngles.z);
        }

        /// <summary>
        /// 폭격 메소드
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnBomb()
        {
            audioData.audioSource.clip = engineClips[Random.Range(0, engineClips.Length)];
            audioData.audioSource.loop = true;
            audioData.audioSource.Play();

            Vector3 targetPosition = target.position;
            targetPosition.y = transform.position.y;
            yield return StartCoroutine("Move", targetPosition);
            DropBomb();

            Vector3 retreatPosition = transform.forward * 150f;
            retreatPosition.y = transform.position.y;
            yield return StartCoroutine("Move", retreatPosition);

            SoundManager.Instance.RemoveAudioSource(audioData);
            Destroy(gameObject);
        }

        /// <summary>
        /// 이동 메소드
        /// </summary>
        /// <param name="targetPosition">이동 목표 위치</param>
        /// <returns>코루틴</returns>
        private IEnumerator Move(Vector3 targetPosition)
        {
            Vector3 startPosition = transform.position;
            float destinationDistance = Vector3.Distance(startPosition, targetPosition);
            float currentMoveDistance = 0f;
            float percent = 0f;

            while (true)
            {
                currentMoveDistance += speed * Time.deltaTime;
                percent = currentMoveDistance / destinationDistance;
                transform.position = Vector3.Lerp(startPosition, targetPosition, percent);

                if (percent >= 1f)
                {
                    yield break;
                }

                yield return null;
            }
        }

        private void DropBomb()
        {
            var bomb = Instantiate(bombPrefab, transform.position - Vector3.down * 2f, Quaternion.Euler(new Vector3(90, 0, 0)));
            bomb.GetComponent<Bomb>().Use();
        }
    }
}