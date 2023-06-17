using System.Collections;
using UnityEngine;
using WhalePark18.Manager;

namespace WhalePark18.Item.Active
{
    [RequireComponent(typeof(AudioSource))]
    public class Bomber : MonoBehaviour
    {
        [SerializeField]
        private float speed = 5f;           // �̵� �ӵ�;

        [SerializeField]
        private AudioClip[] engineClips;    // ��Ʈ ���� Ŭ��

        [SerializeField]
        private GameObject bombPrefab;      // ��ź ������
        private Transform target;           // Ÿ��

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
        /// Bomber ���� �������̽�
        /// </summary>
        /// <param name="target">��ǥ</param>
        public void Fly(Transform target)
        {
            this.target = target;
            LookAt(target.position);
            StartCoroutine("OnBomb");
        }

        /// <summary>
        /// Bomber�� target�� ���� �ٶ󺸰� �����ϴ� �޼ҵ�
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
        /// ���� �޼ҵ�
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
        /// �̵� �޼ҵ�
        /// </summary>
        /// <param name="targetPosition">�̵� ��ǥ ��ġ</param>
        /// <returns>�ڷ�ƾ</returns>
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