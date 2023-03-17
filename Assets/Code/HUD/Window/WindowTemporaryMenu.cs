using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using WhalePark18.Manager;

namespace WhalePark18.HUD.Window
{
    public enum TemporaryMenu
    {
        GameExit
    }

    public class WindowTemporaryMenu : WindowBase
    {
        [SerializeField]
        private Button[] buttonMenu;

        private void Start()
        {
            ButtonBinding();
        }

        /// <summary>
        /// 버튼 바인딩 메소드 
        /// </summary>
        private void ButtonBinding()
        {
            buttonMenu[(int)TemporaryMenu.GameExit].onClick.AddListener(OnClickGameExit);
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