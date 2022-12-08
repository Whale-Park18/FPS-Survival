using System.Collections;
using UnityEngine;

using WhalePark18.Character;

namespace WhalePark18.Item.Kit
{
    public class ItemMedicBag : ItemBase
    {
        [SerializeField]
        private GameObject hpEffectPrefab;
        [SerializeField]
        private int increaseHP = 50;

        public override void Use(GameObject entity)
        {
            entity.GetComponent<Status>().IncreaseHP(increaseHP);

            Instantiate(hpEffectPrefab, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}