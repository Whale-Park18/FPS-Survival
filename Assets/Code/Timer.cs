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
         * ������Ƽ
         ****************************************/
        public float Time => time;

        /// <summary>
        /// Ÿ�̸� �۵� �������̽�
        /// </summary>
        public void Run()
        {
            /// �ߺ� ������ ����
            if (isRunning)
                return;

            isRunning = true;
            OnInitialized();
            coroutineHandle = StartCoroutine(OnTimer());
        }

        /// <summary>
        /// Ÿ�̸� ���� �������̽�
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
        /// �ʱ�ȭ �޼ҵ�
        /// </summary>
        private void OnInitialized()
        {
            time = 0;
        }

        /// <summary>
        /// Ÿ�̸� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator OnTimer()
        {
            while (true)
            {
                time += UnityEngine.Time.deltaTime;

                yield return null;
            }
        }

        /// <summary>
        /// ���� �ð��� ���ڿ��� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <returns>�ð� ���ڿ�</returns>
        /// <remarks>
        /// Ÿ�̸Ӱ� �۵� ���̸� ȣ�� ��� �ð�, ���� �Ķ�� ���� �ð��� ��ȯ�Ѵ�.
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