using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using WhalePark18.Manager;

namespace WhalePark18.HUD.Window
{
    public enum MainMenu { Start, Explain, Exit }
    public enum MainMenuAlpha { ImageBackground, ImageButton, Text }

    public class WindowMain : WindowBase
    {
        [Header("Panel Main Menu")]
        [SerializeField]
        private TextMeshProUGUI textTitle;
        [SerializeField, Tooltip("Panel Main Menu�� Background")]
        private Image imageBackground;
        [SerializeField]
        private Button[] buttonMainMenuGroup;
        [SerializeField, Tooltip("ImageBackground, ImageButton, Text �ִ� Alpha��")]
        private float[] maxAlphaValues;

        [Header("Window Explain")]
        [SerializeField]
        GameObject windowGameExplain;
        [SerializeField]
        GameObject panelMainMenu;

        private void Awake()
        {
            ButtonBinding();
        }

        private void OnEnable()
        {
            StartCoroutine("OnActive");
        }

        /// <summary>
        /// ��ư ���ε� �޼ҵ�
        /// </summary>
        private void ButtonBinding()
        {
            buttonMainMenuGroup[(int)MainMenu.Start].onClick.AddListener(OnClickGameStart);
            buttonMainMenuGroup[(int)MainMenu.Explain].onClick.AddListener(OnClickGameExplain);
            buttonMainMenuGroup[(int)MainMenu.Exit].onClick.AddListener(OnClickGameExit);
        }

        /// <summary>
        /// '���� ����' ��ư �̺�Ʈ �޼ҵ�
        /// </summary>
        public void OnClickGameStart()
        {
            //SceneManager.LoadScene(SceneName.Game);
        }

        /// <summary>
        /// '���� ����' ��ư �̺�Ʈ �޼ҵ�
        /// </summary>
        public void OnClickGameExplain()
        {
            panelMainMenu.SetActive(false);
            windowGameExplain.SetActive(true);
        }

        /// <summary>
        /// '���� ����' ��ư �̺�Ʈ �޼ҵ�
        /// </summary>
        public void OnClickGameExit()
        {
            GameManager.Instance.GameExit();
        }

        protected override IEnumerator OnActive()
        {
            yield return new WaitForSeconds(2f);

            Color colorImageBackground = imageBackground.color;
            Color colorText = textTitle.color;
            Color colorImageButton = buttonMainMenuGroup[0].image.color;

            for (float runTime = 0, percent = 0; runTime < windowMoveTime; runTime += Time.unscaledDeltaTime, percent = runTime / windowMoveTime)
            {
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