using UnityEngine;
using WhalePark18.Character;
using WhalePark18.Item.Active;

namespace WhalePark18.Item.Kit
{
    public class ItemTurret : ItemBase
    {
        [SerializeField]
        private GameObject turretPrefab;
        [SerializeField]
        private float turretDestroyTime = 30f;

        public override void Use(GameObject entity)
        {
            Vector3 createPosition = new Vector3(entity.transform.position.x, 0, entity.transform.position.z);
            Quaternion createRotation = new Quaternion(0, 0, 0, 1);
            Turret clone = Instantiate(turretPrefab, createPosition, createRotation).GetComponent<Turret>();
            clone.DestroyTime = CalculateDuration(entity.GetComponent<Status>().CurrentItemEfficiency);

            Destroy(gameObject);
        }

        private float CalculateDuration(float itemEfficiency)
        {
            return turretDestroyTime * itemEfficiency;
        }
    }
}