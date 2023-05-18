using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WhalePark18.UI.Window.Game.PlayerHUDPanels
{
    public class PanelCoin : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textCoin;

        public void UpdateCoin(int currentCoin)
        {
            textCoin.text = currentCoin.ToString();
        }
    }
}