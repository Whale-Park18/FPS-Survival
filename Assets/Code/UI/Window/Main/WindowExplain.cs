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
            gameObject.SetActive(false);
        }
    }
}