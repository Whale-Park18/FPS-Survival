using System;
using System.Collections;
using UnityEngine;

namespace WhalePark18
{
    public class Timer : MonoBehaviour
    {
        /****************************************
         * Timer
         ****************************************/
        private bool        isRunning = false;
        private float       time;
        private Coroutine   coroutineHandle;

        /****************************************
         * 프로퍼티
         ****************************************/
        public float Time => time;

        /// <summary>
        /// 타이머 작동 인터페이스
        /// </summary>
        public void Run()
        {
            /// 중복 실행을 방지
            if (isRunning)
                return;

            isRunning = true;
            OnInitialized();
            coroutineHandle = StartCoroutine(OnTimer());
        }

        /// <summary>
        /// 타이머 중지 인터페이스
        /// </summary>
        public void Stop()
        {
            if (isRunning == false && coroutineHandle == null)
                return;

            isRunning = false;
            StopCoroutine(coroutineHandle);
            coroutineHandle = null;
        }

        /// <summary>
        /// 초기화 메소드
        /// </summary>
        private void OnInitialized()
        {
            time = 0;
        }

        /// <summary>
        /// 타이머 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator OnTimer()
        {
            while (true)
            {
                time += UnityEngine.Time.deltaTime;

                yield return null;
            }
        }

        /// <summary>
        /// 현재 시간을 문자열로 반환하는 메소드
        /// </summary>
        /// <returns>시간 문자열</returns>
        /// <remarks>
        /// 타이머가 작동 중이면 호출 당시 시간, 멈춘 후라면 최종 시간을 반환한다.
        /// </remarks>
        public override string ToString()
        {
            int hour = (int)time / 3600;
            int minute = (int)time / 60 % 60;
            int second = (int)time % 60;

            return String.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
        }
    }
}