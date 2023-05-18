using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WhalePark18.Manager;

namespace WhalePark18.UI.Window.Game.PlayerHUDPanels
{
    public class PanelPlayTime : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textPlayTime;

        public void StartTimer()
        {
            StartCoroutine("OnTimer");
        }

        public void StopTimer()
        {
            StopCoroutine("OnTimer");
        }

        private IEnumerator OnTimer()
        {
            while(true)
            {
                textPlayTime.text = GameManager.Instance.TimeToString();
                yield return null;
            }
        }
    }
}