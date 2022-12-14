using UnityEngine;

namespace WhalePark18.Item.Kit
{
    public class ItemTurret : ItemBase
    {
        [SerializeField]
        private GameObject turretPrefab;

        public override void Use(GameObject entity)
        {
            Vector3 createPosition = new Vector3(entity.transform.position.x, 0, entity.transform.position.z);
            Quaternion createRotation = new Quaternion(0, 0, 0, 1);
            Instantiate(turretPrefab, createPosition, createRotation);

            Destroy(gameObject);
        }
    }
}