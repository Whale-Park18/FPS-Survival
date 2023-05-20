using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using WhalePark18.Manager;

namespace WhalePark18.UI.Window.Game
{
    public enum TemporaryMenu
    {
        ReturnMain = 0, GameExit
    }

    public class WindowGameMenu : WindowBase
    {
        [SerializeField]
        private Button[] buttonMenu;

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
        /// ��ư ���ε� �޼ҵ� 
        /// </summary>
        private void ButtonBinding()
        {
            buttonMenu[(int)TemporaryMenu.ReturnMain].onClick.AddListener(OnClickReturnMain);
            buttonMenu[(int)TemporaryMenu.GameExit].onClick.AddListener(OnClickGameExit);
        }

        /// <summary>
        /// ���� ��ư �޼ҵ�
        /// </summary>
        public void OnClickReturnMain()
        {
            GameManager.Instance.ReturnMain();
        }

        /// <summary>
        /// ���� ���� ��ư �޼ҵ�
        /// </summary>
        public void OnClickGameExit()
        {
            GameManager.Instance.GameExit();
        }
    }
}