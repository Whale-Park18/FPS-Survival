using System.Collections;
using UnityEngine;

using WhalePark18.Manager;

namespace WhalePark18.HUD.Window
{
    public class WindowBase : MonoBehaviour
    {
        [SerializeField]
        protected float windowMoveTime = 1f;
        protected bool windowActive = false;

        /// <summary>
        /// window 사용 인터페이스
        /// </summary>
        public virtual void OnWindowPower()
        {
            windowActive = !windowActive;

            if (windowActive)
            {
                GameManager.Instance.Pause();
                GameManager.Instance.SetCursorActive(GameManager.Instance.IsPause);

                StopCoroutine("OnDisactive");
                StartCoroutine("OnActive");
            }
            else
            {
                GameManager.Instance.Resume();
                GameManager.Instance.SetCursorActive(GameManager.Instance.IsPause);

                StopCoroutine("OnActive");
                StartCoroutine("OnDisactive");
            }
        }

        /// <summary>
        /// window 활성화 메소드
        /// </summary>
        /// <returns>코루틴</returns>
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
        /// window 비활성화 메소드
        /// </summary>
        /// <returns>코루틴</returns>
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