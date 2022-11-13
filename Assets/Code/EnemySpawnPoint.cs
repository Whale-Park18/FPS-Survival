using System.Collections;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField]
    private float           fadeSpeed = 4f;
    private MeshRenderer    meshRenderer;

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

    private IEnumerator OnFadeEffect()
    {
        while(true)
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
