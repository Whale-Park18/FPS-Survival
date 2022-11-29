using System.Collections;
using UnityEngine;

namespace WhalePark18.Character.Enemy
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private float fadeSpeed = 4f;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {
            StartCoroutine("OnFadeEffect");
        }

        private void OnDisable()
        {
            StopCoroutine("OnFadeEffect");
        }

        /// <summary>
        /// Fade 효과 메소드
        /// </summary>
        /// <returns>코루틴</returns>
        private IEnumerator OnFadeEffect()
        {
            while (true)
            {
                /// float f = Mathf.PingPong(float t, float length);
                /// t값에 따라 0부터 lenght 사이의 값이 반환된다.
                /// t값이 계속 증가할 때 lenght까지는 t값을 반환하고,
                /// t가 lenght보다 커졌을 때 순차적으로 0까지 감소, length까지 증가를 반복
                Color color = meshRenderer.material.color;
                color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time * fadeSpeed, 1));
                meshRenderer.material.color = color;

                yield return null;
            }
        }
    }
}