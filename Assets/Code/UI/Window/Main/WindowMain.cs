using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using WhalePark18.Manager;
using System.Dynamic;

namespace WhalePark18.UI.Window.Main
{
    public enum MainMenu { Start, Explain, Setting , Exit }
    public enum MainMenuAlpha { ImageBackground, ImageButton, Text }

    public class WindowMain : WindowBase
    {
        private Coroutine coroutineHandle;

        [Header("Panel Main Menu")]
        [SerializeField]
        private TextMeshProUGUI textTitle;
        [SerializeField, Tooltip("Panel Main Menu의 Background")]
        private Image imageBackground;
        [SerializeField]
        private Button[] buttonMainMenuGroup;
        [SerializeField, Tooltip("ImageBackground, ImageButton, Text 최대 Alpha값")]
        private float[] maxAlphaValues;

        private AudioData audioData;

        [Header("Window Explain")]
        [SerializeField]
        private GameObject windowGameExplain;
        [SerializeField]
        private GameObject panelMainMenu;

        [Header("Window Setting")]
        [SerializeField]
        private GameObject windowSetting;

        private void Awake()
        {
            audioData = new AudioData(GetComponent<AudioSource>(), Manager.AudioType.Music);
            ButtonBinding();
        }

        private void Start()
        {
            /// 오디오 소스 매니저에 등록
            SoundManager.Instance.AddAudioSource(audioData);

            audioData.audioSource.Play();
            coroutineHandle = StartCoroutine(OnActive());
        }

        private void OnDisable()
        {
            if (audioData.audioSource.isPlaying)
                audioData.audioSource.Stop();
            SoundManager.Instance.RemoveAudioSource(audioData);

            if(coroutineHandle != null)
            {
                StopCoroutine(coroutineHandle);
                coroutineHandle = null;
            }
        }

        /// <summary>
        /// 버튼 바인딩 메소드
        /// </summary>
        private void ButtonBinding()
        {
            buttonMainMenuGroup[(int)MainMenu.Start].onClick.AddListener(OnClickGameStart);
            buttonMainMenuGroup[(int)MainMenu.Explain].onClick.AddListener(OnClickGameExplain);
            buttonMainMenuGroup[(int)MainMenu.Setting].onClick.AddListener(OnclickGameSetting);
            buttonMainMenuGroup[(int)MainMenu.Exit].onClick.AddListener(OnClickGameExit);
        }

        /// <summary>
        /// '게임 시작' 버튼 이벤트 메소드
        /// </summary>
        public void OnClickGameStart()
        {
            GameManager.Instance.GameStart();
        }

        public override void Reset()
        {
            Color colorImageBackground = imageBackground.color;
            colorImageBackground.a = 0f;
            imageBackground.color = colorImageBackground;

            Color colorText = textTitle.color;
            colorText.a = 0f;
            textTitle.color = colorText;

            Color colorImageButton = buttonMainMenuGroup[0].image.color;
            colorImageButton.a = 0f;
            foreach (Button button in buttonMainMenuGroup)
            {
                button.image.color = colorImageButton;
                button.GetComponentInChildren<TextMeshProUGUI>().color = colorText;
            }
        }

        /// <summary>
        /// '게임 설명' 버튼 이벤트 메소드
        /// </summary>
        public void OnClickGameExplain()
        {
            windowGameExplain.SetActive(true);
        }

        public void OnclickGameSetting()
        {
            windowSetting.SetActive(true);
        }

        /// <summary>
        /// '게임 종료' 버튼 이벤트 메소드
        /// </summary>
        public void OnClickGameExit()
        {
            GameManager.Instance.GameExit();
        }

        protected override IEnumerator OnActive()
        {
            LogManager.ConsoleDebugLog("WindowMain", "OnActive Run");

            yield return new WaitForSeconds(2f);

            Color colorImageBackground = imageBackground.color;
            Color colorText = textTitle.color;
            Color colorImageButton = buttonMainMenuGroup[0].image.color;

            for (float runTime = 0, percent = 0; runTime < windowMoveTime; runTime += Time.unscaledDeltaTime, percent = runTime / windowMoveTime)
            {
                //LogManager.ConsoleDebugLog("WindowMain", $"OnActive - percent: {percent}");

                float currentImageBackgroundAlpha = Mathf.Lerp(0, maxAlphaValues[(int)MainMenuAlpha.ImageBackground], percent);
                colorImageBackground.a = currentImageBackgroundAlpha;
                imageBackground.color = colorImageBackground;

                float currentTextAlpha = Mathf.Lerp(0, maxAlphaValues[(int)MainMenuAlpha.Text], percent);
                colorText.a = currentTextAlpha;
                textTitle.color = colorText;

                foreach(Button button in buttonMainMenuGroup)
                {
                    float currentImageButtonAlpha = Mathf.Lerp(0, maxAlphaValues[(int)MainMenuAlpha.ImageButton], percent);
                    colorImageButton.a = currentImageButtonAlpha;

                    button.image.color = colorImageButton;
                    button.GetComponentInChildren<TextMeshProUGUI>().color = colorText;
                }

                yield return null;
            }
        }
    }
}