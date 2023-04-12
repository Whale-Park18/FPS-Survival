using System.Collections;
using System.Reflection;
using UnityEngine;

namespace WhalePark18.Objects
{
    public class DestructiblePillar : InteractionObject
    {
        [Header("Destructible Paillar")]
        [SerializeField,Tooltip("�ı� ������ ��� ���ӿ�����Ʈ")]
        private GameObject destructiblePillar;
        [SerializeField, Tooltip("����� �ı��Ǿ��� ��, ��ü�� ���ӿ�����Ʈ")]
        private GameObject destructiblePillarPieces;
        [SerializeField, Tooltip("���� �ð�")]
        private float repairTime = 20;

        private bool isDestroyed = false;

        private BoxCollider boxCollider;

        protected override void Awake()
        {
            base.Awake();
            boxCollider = GetComponent<BoxCollider>();
        }

        public override void TakeDamage(int damage)
        {
            WhalePark18.Debug.Log(DebugCategory.Debug, MethodBase.GetCurrentMethod().Name, "���� �����: {0} / ���ط�: {1} / �ǰ��� �����: {2}", currentHP, damage, currentHP - damage);

            currentHP = currentHP - damage < 0 ? 0 : currentHP - damage;

            if (currentHP <= 0 && isDestroyed == false)
            {
                isDestroyed = true;

                boxCollider.enabled = false;

                /// ���� ������Ʈ�� ���� ������Ʈ�� ����Ѵ�.
                destructiblePillar.SetActive(false);
                destructiblePillarPieces.SetActive(true);

                StartCoroutine("Repair");
            }
        }

        /// <summary>
        /// ��� ���¸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <returns></returns>
        private IEnumerator Repair()
        {
            yield return new WaitForSeconds(repairTime);

            Reset();
        }

        public void Reset()
        {
            /// ��� ���� �ʱ�ȭ
            currentHP = maxHP;
            isDestroyed = false;
            
            /// ������Ʈ ���� �ʱ�ȭ
            destructiblePillarPieces.SetActive(false);
            destructiblePillar.SetActive(true);
            boxCollider.enabled = true;
        }
    }
}