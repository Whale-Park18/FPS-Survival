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
        /// 버튼 바인딩 메소드
        /// </summary>
        private void ButtonBinding()
        {
            buttonReturn.onClick.AddListener(OnClickReturn);
        }

        /// <summary>
        /// 되돌아가기 버튼 메소드
        /// </summary>
        public void OnClickReturn()
        {
            panelMainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}