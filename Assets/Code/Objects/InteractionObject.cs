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
        /// ���� ó�� �޼ҵ�
        /// </summary>
        /// <param name="damage">���ط�</param>
        public abstract void TakeDamage(int damage);
    }
}