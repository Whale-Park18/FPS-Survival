using UnityEngine;

using WhalePark18.Weapon;

namespace WhalePark18.Item.Kit
{
    public class ItemMagazine : ItemBase
    {
        [SerializeField]
        private GameObject magazineEffectPreab;
        [SerializeField]
        private int increaseMagazine = 2;

        public override void Use(GameObject entity)
        {
            /// Main ����(AssaultRifle)�� źâ ���� increaseMagazine ��ŭ ����
            entity.GetComponent<WeaponSwitchSystem>().IncreaseMagazine(WeaponType.Main, increaseMagazine);

            Instantiate(magazineEffectPreab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}