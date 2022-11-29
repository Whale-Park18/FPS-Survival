using System.Collections;
using UnityEngine;

namespace WhalePark18.Objects
{
    public class Target : InteractionObject
    {
        [SerializeField]
        private AudioClip clipTargetUp;         // Ÿ�� Up ����
        [SerializeField]
        private AudioClip clipTargetDown;       // Ÿ�� Down ����
        [SerializeField]
        private float targetUpDelayTime = 3f;   // Ÿ���� �ٽ� �ö���� ��, ������ �ð�

        private bool isPossibleHit = true;      // Ÿ�� ������ ����
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// ���� ó�� �޼ҵ�
        /// </summary>
        /// <param name="damage">���ط�</param>
        public override void TakeDamage(int damage)
        {
            //print("Hit Target");
            currentHP -= damage;

            if (currentHP <= 0 && isPossibleHit)
            {
                isPossibleHit = false;
                StartCoroutine("OnTargetDown");
            }
        }

        /// <summary>
        /// Ÿ�� Up ó�� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator OnTargetUp()
        {
            yield return new WaitForSeconds(targetUpDelayTime);

            audioSource.clip = clipTargetUp;
            audioSource.Play();

            yield return StartCoroutine(OnAnimation(90, 0));

            isPossibleHit = true;
        }

        /// <summary>
        /// Ÿ�� Down ó�� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator OnTargetDown()
        {
            audioSource.clip = clipTargetDown;
            audioSource.Play();

            yield return StartCoroutine(OnAnimation(0, 90));

            StartCoroutine("OnTargetUp");
        }

        /// <summary>
        /// Ÿ���� Up/Down �ִϸ��̼� ȿ���� ó���ϴ� �޼ҵ�
        /// </summary>
        /// <param name="start">�ð� ����</param>
        /// <param name="end">���� ����</param>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator OnAnimation(float start, float end)
        {
            float percent = 0;
            float current = 0;
            float time = 1f;

            /// Tip.
            ///  ������ �Ʒ� ���� �������� ȸ���ϵ��� �� ������Ʈ�� �θ�� ����
            while (percent < 1)
            {
                current += Time.deltaTime;
                percent = current / time;

                transform.rotation = Quaternion.Slerp(Quaternion.Euler(start, 0, 0), Quaternion.Euler(end, 0, 0), percent);

                yield return null;
            }
        }
    }
}