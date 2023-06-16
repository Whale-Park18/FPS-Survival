using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using WhalePark18.Manager;

namespace WhalePark18.UI.Window.Game
{
    public enum TemporaryMenu
    {
        ReturnMain = 0, GameSetting, GameExit
    }

    public class WindowGameMenu : WindowBase
    {
        [SerializeField]
        private Button[] buttonMenu;

        [SerializeField]
        private GameObject windowSetting;

        private void Start()
        {
            ButtonBinding();
        }

        private void OnDisable()
        {
            // #DEBUG
            //Reset();
        }

        /// <summary>
        /// 버튼 바인딩 메소드 
        /// </summary>
        private void ButtonBinding()
        {
            buttonMenu[(int)TemporaryMenu.ReturnMain].onClick.AddListener(OnClickReturnMain);
            buttonMenu[(int)TemporaryMenu.GameSetting].onClick.AddListener(OnClickGameSetting);
            buttonMenu[(int)TemporaryMenu.GameExit].onClick.AddListener(OnClickGameExit);
        }

        /// <summary>
        /// 메인 버튼 메소드
        /// </summary>
        public void OnClickReturnMain()
        {
            GameManager.Instance.ReturnMain();
        }

        public void OnClickGameSetting()
        {
            windowSetting.SetActive(true);
        }

        /// <summary>
        /// 게임 종료 버튼 메소드
        /// </summary>
        public void OnClickGameExit()
        {
            GameManager.Instance.GameExit();
        }
    }
}