using UnityEngine;

namespace WhalePark18.Objects
{
    /// <summary>
    /// �ı� ������ �巳�� Ŭ����
    /// </summary>
    public class DesturctibleBarrel : InteractionObject
    {
        [Header("Destructible Barrel")]
        [SerializeField]
        private GameObject destructibleBarrelPieces;    // �ı��� ��ü�Ǵ� ������(���� ������Ʈ)

        private bool isDestroyed = false;               // �ı� ����

        public override void TakeDamage(int damage)
        {
            currentHP = -damage;

            if (currentHP <= 0 && isDestroyed == false)
            {
                isDestroyed = true;

                /// ���� ������Ʈ�� ���� ������Ʈ�� ����Ѵ�.
                Instantiate(destructibleBarrelPieces, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }
}