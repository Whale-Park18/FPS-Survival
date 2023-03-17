using UnityEngine;

using WhalePark18;
using WhalePark18.Manager;

public class Impact : MonoBehaviour
{
    private ParticleSystem particle;
    private MemoryPool memoryPool;      // 오브젝트풀 개선 전, 사용하는 변수
    [SerializeField]
    private ImpactType impactType;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void Setup(MemoryPool pool)
    {
        memoryPool = pool;
    }

    private void Update()
    {
        /// 파티클이 재생중이 아니면 비활성화
        if (particle.isPlaying == false)
        {
            /// 오브젝트풀 개선 전
            //memoryPool.DeactivePoolItem(gameObject);

            /// 오브젝트풀 개선 후
            ImpactManager.Instance.ReturnImpact(impactType, this);
        }
    }
}