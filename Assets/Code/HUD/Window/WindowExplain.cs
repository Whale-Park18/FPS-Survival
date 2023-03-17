using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WhalePark18.HUD.Window
{
    public class WindowExplain : WindowBase
    {
        [SerializeField]
        Button buttonReturn;

        [SerializeField]
        GameObject panelMainMenu;

        private void Start()
        {
            ButtonBinding();
        }

        /// <summary>
        /// ��ư ���ε� �޼ҵ�
        /// </summary>
        private void ButtonBinding()
        {
            buttonReturn.onClick.AddListener(OnClickReturn);
        }

        /// <summary>
        /// �ǵ��ư��� ��ư �޼ҵ�
        /// </summary>
        public void OnClickReturn()
        {
            panelMainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}