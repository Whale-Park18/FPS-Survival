using UnityEngine;

namespace WhalePark18.Item.Kit
{
    public class ItemNuclear : ItemBase
    {
        [Header("Plane")]
        [SerializeField]
        private GameObject Bomber;                  // 폭격기 프리팹
        [SerializeField]
        private float minHorizontalRange = 100f;    // 최소 수평 범위
        [SerializeField]
        private float maxHorizontalRange = 150f;    // 최대 수평 범위
        [SerializeField]
        private float minVerticalRange = 80f;       // 최소 수직(비행 고도) 높이
        [SerializeField]
        private float maxVerticalRange = 100f;      // 최대 수직 높이

        public override void Use(GameObject entity)
        {
            GameObject clone = Instantiate(Bomber);
            clone.transform.position = transform.position + CalculateSpawnPosition();
            clone.GetComponent<Bomber>().Fly(transform);

            Destroy(gameObject);
        }

        /// <summary>
        /// Bomber 스폰 장소를 계산하는 메소드
        /// </summary>
        /// <returns>Bomber 스폰 위치</returns>
        private Vector3 CalculateSpawnPosition()
        {
            float jitter = Random.Range(0, 360);
            Vector3 spawnPosition = transform.position + CalculatePositionByAngle(jitter);
            spawnPosition.y = Random.Range(minVerticalRange, maxVerticalRange);

            return spawnPosition;
        }

        /// <summary>
        /// angle 각도의 랜덤 위치 계산
        /// </summary>
        /// <param name="angle">각도</param>
        /// <returns>angle 각도의 랜덤 위치</returns>
        private Vector3 CalculatePositionByAngle(float angle)
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Cos(angle) * Random.Range(minHorizontalRange, maxHorizontalRange);
            position.z = Mathf.Sin(angle) * Random.Range(minHorizontalRange, maxHorizontalRange);

            return position;
        }
    }
}