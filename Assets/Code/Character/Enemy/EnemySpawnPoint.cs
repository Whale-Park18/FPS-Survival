using System.Collections;
using UnityEngine;

namespace WhalePark18.Character.Enemy
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        /****************************************
         * EnemySpawnPoint
         ****************************************/
        [SerializeField]
        private float fadeSpeed = 4f;
        private MeshRenderer meshRenderer;

        /****************************************
         * EnemySpawnPoint ������
         ****************************************/
        private int id;
        [SerializeField]
        private string baseName;

        /****************************************
         * ������Ƽ
         ****************************************/
        public int ID => id;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public void Setup(int id)
        {
            this.id = id;
            gameObject.name = $"{baseName}_{id}";
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
        /// Fade ȿ�� �޼ҵ�
        /// </summary>
        /// <returns>�ڷ�ƾ</returns>
        private IEnumerator OnFadeEffect()
        {
            while (true)
            {
                /// float f = Mathf.PingPong(float t, float length);
                /// t���� ���� 0���� lenght ������ ���� ��ȯ�ȴ�.
                /// t���� ��� ������ �� lenght������ t���� ��ȯ�ϰ�,
                /// t�� lenght���� Ŀ���� �� ���������� 0���� ����, length���� ������ �ݺ�
                Color color = meshRenderer.material.color;
                color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time * fadeSpeed, 1));
                meshRenderer.material.color = color;

                yield return null;
            }
        }
    }
}