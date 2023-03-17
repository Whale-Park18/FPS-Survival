using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.Character.Player;

namespace WhalePark18.Item.Kit
{
    public class ItemCoin : ItemBase
    {
        [SerializeField]
        private int minCoin = 10;
        [SerializeField]
        private int maxCoin = 100;

        public override void Use(GameObject entity)
        {
            entity.GetComponent<PlayerStatus>().IncreaseCoin(Random.Range(minCoin, maxCoin));

            Destroy(gameObject);
        }
    }
}
