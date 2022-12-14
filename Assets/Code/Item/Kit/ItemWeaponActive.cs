using UnityEngine;
using WhalePark18.Weapon;

namespace WhalePark18.Item.Kit
{
    public class ItemWeaponActive : ItemBase
    {
        [SerializeField]
        private WeaponType weaponType;

        public override void Use(GameObject entity)
        {
            entity.GetComponent<WeaponSwitchSystem>().Weapons[(int)weaponType].UnLock();

            Destroy(gameObject);
        }
    }
}
