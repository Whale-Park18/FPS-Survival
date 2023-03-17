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
        /// ��ư ���ε� �޼ҵ� 
        /// </summary>
        private void ButtonBinding()
        {
            buttonMenu[(int)TemporaryMenu.GameExit].onClick.AddListener(OnClickGameExit);
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