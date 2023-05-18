using System;
using System.Collections;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

using WhalePark18.Manager;

namespace WhalePark18.UI.Window
{
    public class WindowBase : MonoBehaviour
    {
        [SerializeField]
        protected float windowMoveTime = 1f;
        protected bool active = false;

        public virtual void Reset()
        {
            transform.position = new Vector3(Camera.main.pixelWidth / 2, -Camera.main.pixelHeight, 0);
        }

        /// <summary>
        /// window ��� �������̽�
        /// </summary>
        public virtual void OnWindowPower()
        {
            active = !active;

            if (active)
                Active();
            else
                DisActive();
        }

        /// <summary>
        /// ������ Ȱ��ȭ �޼ҵ�
        /// </summary>
        protected void Active()
        {
            GameManager.Instance.Pause();
            GameManager.Instance.SetCursorActive(GameManager.Instance.IsPause);

            StopCoroutine("OnDisactive");
            StartCoroutine("OnActive");
        }

        /// <summary>
        /// ������ ��Ȱ��ȭ �޼ҵ�
        /// </summary>
        protected void DisActive()
        {
            GameManager.Instance.Resume();
            GameManager.Instance.SetCursorActive(GameManager.Instance.IsPause);

            StopCoroutine("OnActive");
            StartCoroutine("OnDisactive");
        }

        /// <summary>
        /// window Ȱ��ȭ �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        protected virtual IEnumerator OnActive()
        {
            Vector3 startPosition = gameObject.transform.position;

            for (float runTime = 0, percent = 0; runTime < windowMoveTime; runTime += Time.unscaledDeltaTime, percent = runTime / windowMoveTime)
            {
                gameObject.transform.position = Vector3.Lerp(startPosition, new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0), percent);
                yield return null;
            }
        }

        /// <summary>
        /// window ��Ȱ��ȭ �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        protected virtual IEnumerator OnDisactive()
        {
            Vector3 startPosition = gameObject.transform.position;

            for (float runTime = 0, percent = 0; runTime <= windowMoveTime; runTime += Time.unscaledDeltaTime, percent = runTime / windowMoveTime)
            {
                gameObject.transform.position = Vector3.Lerp(startPosition, new Vector3(Camera.main.pixelWidth / 2, -Camera.main.pixelHeight / 2, 0), percent);
                yield return null;
            }
        }
    }
}