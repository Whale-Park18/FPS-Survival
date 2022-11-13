using UnityEngine;

public class ParticleAutoDestroyerByTime : MonoBehaviour
{
    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if(particle.isPlaying == false)
        {
            /// �ʿ� ���� �����ų ���̶�� �޸� Ǯ ����� ��
            Destroy(gameObject);
        }
    }
}
