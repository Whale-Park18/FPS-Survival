using UnityEngine;

namespace WhalePark18.Objects
{
    public abstract class InteractionObject : MonoBehaviour
    {
        [Header("Interaction Object")]
        [SerializeField]
        protected int maxHP = 100;
        protected int currentHP;

        private void Awake()
        {
            currentHP = maxHP;
        }

        /// <summary>
        /// 피해 처리 메소드
        /// </summary>
        /// <param name="damage">피해량</param>
        public abstract void TakeDamage(int damage);
    }
}