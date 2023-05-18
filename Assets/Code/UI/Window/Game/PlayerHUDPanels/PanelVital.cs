using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WhalePark18.UI.Window.Game.PlayerHUDPanels
{
    public class PanelVital : MonoBehaviour
    {
        [Header("Health Point")]
        [SerializeField]
        private Slider          sliderHealthPoint;
        [SerializeField]
        private TextMeshProUGUI textHealthPoint;

        [Header("Shield")]
        [SerializeField]
        private Slider          sliderShield;
        [SerializeField] 
        private TextMeshProUGUI textShield;

        [Header("Blood Effect")]
        [SerializeField]
        private Image           imageBloodEffect;       // ���� ȿ�� �̹���
        [SerializeField]
        private float           bloodEffectTime;        // ���� ȿ�� �ð�
        [SerializeField]
        private AnimationCurve  animCurveBloodEffect;   // imageBloodEffect�� ���İ� ���濡 ���� �׷���

        public void UpdateHealthPoint(int previousHealthPoint, int currentHealthPoint)
        {
            textHealthPoint.text = currentHealthPoint.ToString();
            sliderHealthPoint.value = (float)currentHealthPoint / 100;

            /// ü�� �������� ���
            if (currentHealthPoint >= previousHealthPoint)
            {
                return;
            }
            /// ü�� ���ҵ��� ���
            else
            {
                StopCoroutine("OnBloodEffect");
                StartCoroutine("OnBloodEffect");
            }
        }

        public void UpdateShield(int currentSheld)
        {
            textShield.text = currentSheld.ToString();

            if (currentSheld <= 0)
                sliderShield.value = 0;
            else
                sliderShield.value = (float)currentSheld / 100;
        }

        private IEnumerator OnBloodEffect()
        {
            for (float runTime = 0; runTime < bloodEffectTime; runTime += Time.deltaTime)
            {
                Color color = imageBloodEffect.color;
                color.a = Mathf.Lerp(1, 0, animCurveBloodEffect.Evaluate(runTime));
                imageBloodEffect.color = color;

                yield return null;
            }
        }
    }
}