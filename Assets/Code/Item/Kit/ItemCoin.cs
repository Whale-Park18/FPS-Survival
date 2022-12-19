using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.Character;

namespace WhalePark18.Item.Kit
{
    public class ItemCoin : ItemBase
    {
        [SerializeField]
        private int coin = 1;

        public override void Use(GameObject entity)
        {
            entity.GetComponent<Status>().IncreaseCoin(coin);

            Destroy(gameObject);
        }
    }
}
