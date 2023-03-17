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
        [SerializeField, Tooltip("아이템 가중치")]
        private int weight;                     // 아이템 가중치

        private IEnumerator Start()
        {
            float y = transform.position.y;

            while (true)
            {
                /// y축을 기준으로 회전
                transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

                /// 처음 배치된 위치를 기준으로 y 위치를 위, 아래로 이동
                Vector3 position = transform.position;
                position.y = Mathf.Lerp(y, y + moveDistance, Mathf.PingPong(Time.time * pingpongSpeed, 1));
                transform.position = position;

                yield return null;
            }
        }

        /// <summary>
        /// 아이템 사용 인터페이스
        /// </summary>
        /// <param name="entity">아이템을 사용하는 주체</param>
        public abstract void Use(GameObject entity);
    }
}