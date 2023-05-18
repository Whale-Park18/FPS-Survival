using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WhalePark18.Manager;

namespace WhalePark18.UI.Window.Game
{
    public class WindowScore : WindowBase
    {
        [Header("패널 관련")]
        [SerializeField]
        private GameObject background;
        [SerializeField]
        private GameObject PanelScore;

        [Header("점수 관련")]
        [SerializeField]
        private TextMeshProUGUI textTotalScore;
        [SerializeField]
        private TextMeshProUGUI textTimeScore;
        [SerializeField] 
        private TextMeshProUGUI textKillScore;
        [SerializeField]
        private Button buttonGameExit;

        private void Awake()
        {
            buttonGameExit.onClick.AddListener(OnClickGameExit);
            GameManager.Instance.gameOverEvent.AddListener(OnActive);
        }

        private void OnDisable()
        {
            // #DEBUG
            //Reset();
        }

        public override void Reset()
        {
            background.SetActive(false);
            PanelScore.transform.position = new Vector2(Camera.main.pixelWidth / 2, -Camera.main.pixelHeight);
        }

        private void OnActive(int totalScore, string timeString, int killScore)
        {
            StartCoroutine(OnActive());

            SetScore(totalScore, timeString, killScore);
        }

        protected override IEnumerator OnActive()
        {
            background.SetActive(true);

            Vector3 startPosition = PanelScore.transform.position;

            for (float runTime = 0, percent = 0; runTime < windowMoveTime; runTime += Time.unscaledDeltaTime, percent = runTime / windowMoveTime)
            {
                PanelScore.transform.position = Vector3.Lerp(startPosition, new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0), percent);
                yield return null;
            }
        }

        private void SetScore(int totalScore, string timeString, int killScore)
        {
            textTotalScore.text = totalScore.ToString();
            textTimeScore.text = timeString;
            textKillScore.text = killScore.ToString();
        }

        public void OnClickGameExit()
        {
            GameManager.Instance.ReturnMain();
        }
    }
}