using System.Collections;
using UnityEngine;

namespace WhalePark18.Item.Kit
{
    public abstract class ItemBase : MonoBehaviour
    {
        [Header("Item Animation")]
        [SerializeField]
        private float moveDistance = 0.2f;
        [SerializeField]
        private float pingpongSpeed = 0.5f;
        [SerializeField]
        private float rotateSpeed = 50;
        [SerializeField, Tooltip("������ ����ġ")]
        private int weight;                     // ������ ����ġ

        private IEnumerator Start()
        {
            float y = transform.position.y;

            while (true)
            {
                /// y���� �������� ȸ��
                transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

                /// ó�� ��ġ�� ��ġ�� �������� y ��ġ�� ��, �Ʒ��� �̵�
                Vector3 position = transform.position;
                position.y = Mathf.Lerp(y, y + moveDistance, Mathf.PingPong(Time.time * pingpongSpeed, 1));
                transform.position = position;

                yield return null;
            }
        }

        /// <summary>
        /// ������ ��� �������̽�
        /// </summary>
        /// <param name="entity">�������� ����ϴ� ��ü</param>
        public abstract void Use(GameObject entity);
    }
}