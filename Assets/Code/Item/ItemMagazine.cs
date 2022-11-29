using System.Collections;
using UnityEngine;

using WhalePark18.Weapon;

namespace WhalePark18.Item
{
    public class ItemMagazine : ItemBase
    {
        [SerializeField]
        private GameObject magazineEffectPreab;
        [SerializeField]
        private int increaseMagazine = 2;
        [SerializeField]
        private float rotateSpeed = 50f;

        private IEnumerator Start()
        {
            while (true)
            {
                transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

                yield return null;
            }
        }

        public override void Use(GameObject entity)
        {
            /// Main ����(AssaultRifle)�� źâ ���� increaseMagazine ��ŭ ����
            entity.GetComponent<WeaponSwitchSystem>().IncreaseMagazine(WeaponType.Main, increaseMagazine);

            Instantiate(magazineEffectPreab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}