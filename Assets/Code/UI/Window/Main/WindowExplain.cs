using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WhalePark18.UI.Window.Main
{
    public class WindowExplain : WindowBase
    {
        [SerializeField]
        Button buttonReturn;

        private void Awake()
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
            gameObject.SetActive(false);
        }
    }
}