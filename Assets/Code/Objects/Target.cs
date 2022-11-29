using System.Collections;
using UnityEngine;

namespace WhalePark18.Objects
{
    public class Target : InteractionObject
    {
        [SerializeField]
        private AudioClip clipTargetUp;         // 타겟 Up 사운드
        [SerializeField]
        private AudioClip clipTargetDown;       // 타겟 Down 사운드
        [SerializeField]
        private float targetUpDelayTime = 3f;   // 타겟이 다시 올라오기 전, 딜레이 시간

        private bool isPossibleHit = true;      // 타격 가능한 상태
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// 피해 처리 메소드
        /// </summary>
        /// <param name="damage">피해량</param>
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
        /// 타겟 Up 처리 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator OnTargetUp()
        {
            yield return new WaitForSeconds(targetUpDelayTime);

            audioSource.clip = clipTargetUp;
            audioSource.Play();

            yield return StartCoroutine(OnAnimation(90, 0));

            isPossibleHit = true;
        }

        /// <summary>
        /// 타겟 Down 처리 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator OnTargetDown()
        {
            audioSource.clip = clipTargetDown;
            audioSource.Play();

            yield return StartCoroutine(OnAnimation(0, 90));

            StartCoroutine("OnTargetUp");
        }

        /// <summary>
        /// 타겟이 Up/Down 애니메이션 효과를 처리하는 메소드
        /// </summary>
        /// <param name="start">시각 각도</param>
        /// <param name="end">종료 각도</param>
        /// <returns>코루틴</returns>
        private IEnumerator OnAnimation(float start, float end)
        {
            float percent = 0;
            float current = 0;
            float time = 1f;

            /// Tip.
            ///  과녁이 아래 축을 기준으로 회전하도록 빈 오브젝트를 부모로 설정
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