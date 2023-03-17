using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18
{
    public class Timer : MonoBehaviour
    {
        private bool isRunning = false;
        private float time;

        public float Time => time;

        public void Run()
        {
            isRunning = true;
            OnInitialized();
            StartCoroutine(OnTimer());
        }

        public void Stop()
        {
            isRunning = false;
            StopCoroutine(OnTimer());
        }

        private void OnInitialized()
        {
            time = 0;
        }

        private IEnumerator OnTimer()
        {
            while (true)
            {
                time += UnityEngine.Time.deltaTime;

                yield return null;
            }
        }

        public override string ToString()
        {
            int hour = (int)time / 3600;
            int minute = (int)time / 60 % 60;
            int second = (int)time % 60;

            //string result = isRunning ? String.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second) : "00:00:00";

            return String.Format("{0:D2}:{1:D2}:{2:D2}", hour, minute, second);
        }
    }
}