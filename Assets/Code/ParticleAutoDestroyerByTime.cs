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
            /// 맵에 많이 등장시킬 것이라면 메모리 풀 사용할 것
            Destroy(gameObject);
        }
    }
}
