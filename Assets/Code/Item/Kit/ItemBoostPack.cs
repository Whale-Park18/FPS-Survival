using System.Collections;
using UnityEngine;
using WhalePark18.Character.Player;

using System.Reflection;

namespace WhalePark18.Item.Kit
{ 
    public class ItemBoostPack : ItemBase
    {
        [SerializeField]
        private float time = 30f;
        [SerializeField]
        private float attackSpeedIncrease = 0.15f;

        public override void Use(GameObject entity)
        {
            print(name + "use");
            StartCoroutine("OnEffect", entity.GetComponent<PlayerStatus>());

            transform.GetChild(0).gameObject.SetActive(false);
        }

        private IEnumerator OnEffect(PlayerStatus status)
        {
            status.IncreaseAttackSpeed(attackSpeedIncrease);
            UnityEngine.Debug.LogFormat("<color=yellow>" + MethodBase.GetCurrentMethod().Name +
                            " currentAttackSpeed: </color>" + status.AttackSpeed.currentAbility);

            yield return new WaitForSeconds(time);

            status.DisincreaseAttackSpeed(attackSpeedIncrease);
            UnityEngine.Debug.LogFormat("<color=yellow>" + MethodBase.GetCurrentMethod().Name +
                            " currentAttackSpeed: </color>" + status.AttackSpeed.currentAbility);
            
            Destroy(gameObject);
        }
    }
}
