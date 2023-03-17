using UnityEngine;

namespace WhalePark18
{
    /// Normal: ��, �ٴ� ��
    /// Obstacle: ��ֹ�
    public enum TmpImpactType { Normal = 0, Obstacle, Enemy, InteractionObject, }

    public class ImpactMemoryPool : MonoBehaviour
    {

        [SerializeField]
        private GameObject[] impactPrefab;  // �ǰ� ����Ʈ
        private MemoryPool[] memoryPool;    // �ǰ� ����Ʈ �޸�Ǯ

        private void Awake()
        {
            /// �ǰ� �̺�Ʈ�� ���� �����̸� �������� memoryPool ����
            memoryPool = new MemoryPool[impactPrefab.Length];
            for (int i = 0; i < impactPrefab.Length; i++)
            {
                memoryPool[i] = new MemoryPool(impactPrefab[i]);
            }
        }

        /// <summary>
        /// ���� �̿�� ����Ʈ ���� �޼ҵ�
        /// </summary>
        /// <param name="hit"></param>
        public void SpawnImpact(RaycastHit hit)
        {
            /// �ε��� ������Ʈ�� Tag ������ ���� �ٸ��� ó��
            if (hit.transform.CompareTag("ImpactNormal"))
            {
                OnSpawnImpact(TmpImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.CompareTag("ImpactObstacle"))
            {
                OnSpawnImpact(TmpImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.CompareTag("ImpactEnemy"))
            {
                OnSpawnImpact(TmpImpactType.Enemy, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.CompareTag("InteractionObject"))
            {
                Color color = hit.transform.GetComponentInChildren<MeshRenderer>().material.color;

                OnSpawnImpact(TmpImpactType.InteractionObject, hit.point, Quaternion.LookRotation(hit.normal), color);
            }
        }

        /// <summary>
        /// �浹ü �̿�� ���彺 ���� �޼ҵ�
        /// </summary>
        /// <param name="other"></param>
        /// <param name="knifeTransform"></param>
        public void SpawnImpact(Collider other, Transform knifeTransform)
        {
            /// �ε��� ������Ʈ�� Tag ������ ���� �ٸ��� ó��
            if (other.CompareTag("ImpactNormal"))
            {
                OnSpawnImpact(TmpImpactType.Normal, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation));
            }
            else if (other.CompareTag("ImpactObstacle"))
            {
                OnSpawnImpact(TmpImpactType.Obstacle, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation));
            }
            else if (other.CompareTag("ImpactEnemy"))
            {
                OnSpawnImpact(TmpImpactType.Enemy, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation));
            }
            else if (other.CompareTag("InteractionObject"))
            {
                Color color = other.transform.GetComponentInChildren<MeshRenderer>().material.color;
                OnSpawnImpact(TmpImpactType.InteractionObject, knifeTransform.position, Quaternion.Inverse(knifeTransform.rotation));
            }
        }

        public void OnSpawnImpact(TmpImpactType type, Vector3 position, Quaternion rotation, Color color = new Color())
        {
            GameObject item = memoryPool[(int)type].ActivePoolItem();
            item.transform.position = position;
            item.transform.rotation = rotation;
            item.GetComponent<Impact>().Setup(memoryPool[(int)type]);

            if (type == TmpImpactType.InteractionObject)
            {
                ParticleSystem.MainModule main = item.GetComponent<ParticleSystem>().main;
                main.startColor = color;
            }
        }

    }
}