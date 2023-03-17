using UnityEngine;

using WhalePark18;
using WhalePark18.Manager;

public class Impact : MonoBehaviour
{
    private ParticleSystem particle;
    private MemoryPool memoryPool;      // ������ƮǮ ���� ��, ����ϴ� ����
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
        /// ��ƼŬ�� ������� �ƴϸ� ��Ȱ��ȭ
        if (particle.isPlaying == false)
        {
            /// ������ƮǮ ���� ��
            //memoryPool.DeactivePoolItem(gameObject);

            /// ������ƮǮ ���� ��
            ImpactManager.Instance.ReturnImpact(impactType, this);
        }
    }
}