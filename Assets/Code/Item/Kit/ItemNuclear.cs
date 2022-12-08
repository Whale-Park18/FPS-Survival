using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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
            //Vector3 createPosition = new Vector3(entity.transform.position.x, 0, entity.transform.position.z);
            //Quaternion createRotation = new Quaternion(0, 0, 0, 1);
            //GameObject clone = Instantiate(ExplosionParticlePrefab, createPosition, createRotation);

            GameObject clone = Instantiate(Bomber);
            clone.transform.position = transform.position + CalculateSpawnPosition();
            clone.GetComponent<Bomber>().Fly(entity.transform);

            Destroy(gameObject);
        }

        private Vector3 CalculateSpawnPosition()
        {
            float jitter = Random.Range(0, 360);
            Vector3 spawnPosition = transform.position + CalculatePositionByAngle(jitter);
            spawnPosition.y = Random.Range(minVerticalRange, maxVerticalRange);

            return spawnPosition;
        }

        private Vector3 CalculatePositionByAngle(float angle)
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Cos(angle) * Random.Range(minHorizontalRange, maxHorizontalRange);
            position.z = Mathf.Sin(angle) * Random.Range(minHorizontalRange, maxHorizontalRange);

            return position;
        }
    }
}