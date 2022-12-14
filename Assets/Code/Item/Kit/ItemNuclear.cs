using UnityEngine;

namespace WhalePark18.Item.Kit
{
    public class ItemNuclear : ItemBase
    {
        [Header("Plane")]
        [SerializeField]
        private GameObject Bomber;                  // ���ݱ� ������
        [SerializeField]
        private float minHorizontalRange = 100f;    // �ּ� ���� ����
        [SerializeField]
        private float maxHorizontalRange = 150f;    // �ִ� ���� ����
        [SerializeField]
        private float minVerticalRange = 80f;       // �ּ� ����(���� ��) ����
        [SerializeField]
        private float maxVerticalRange = 100f;      // �ִ� ���� ����

        public override void Use(GameObject entity)
        {
            GameObject clone = Instantiate(Bomber);
            clone.transform.position = transform.position + CalculateSpawnPosition();
            clone.GetComponent<Bomber>().Fly(transform);

            Destroy(gameObject);
        }

        /// <summary>
        /// Bomber ���� ��Ҹ� ����ϴ� �޼ҵ�
        /// </summary>
        /// <returns>Bomber ���� ��ġ</returns>
        private Vector3 CalculateSpawnPosition()
        {
            float jitter = Random.Range(0, 360);
            Vector3 spawnPosition = transform.position + CalculatePositionByAngle(jitter);
            spawnPosition.y = Random.Range(minVerticalRange, maxVerticalRange);

            return spawnPosition;
        }

        /// <summary>
        /// angle ������ ���� ��ġ ���
        /// </summary>
        /// <param name="angle">����</param>
        /// <returns>angle ������ ���� ��ġ</returns>
        private Vector3 CalculatePositionByAngle(float angle)
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Cos(angle) * Random.Range(minHorizontalRange, maxHorizontalRange);
            position.z = Mathf.Sin(angle) * Random.Range(minHorizontalRange, maxHorizontalRange);

            return position;
        }
    }
}