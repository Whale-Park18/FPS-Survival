using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TreeEditor;
using UnityEngine;
using WhalePark18.Character;

namespace WhalePark18.Item.Kit
{
    public class ItemBombing : ItemBase
    {
        [SerializeField]
        private GameObject bombPrefab;
        [SerializeField]
        private float bombingRadius = 5f;
        [SerializeField]
        private int numberOfBomb = 5;

        public override void Use(GameObject entity)
        {
            int maxCount = numberOfBomb + CalculateBombIncrease(entity.GetComponent<Status>());
            for(int i = 0; i < maxCount; i++)
            {
                GameObject bomb = Instantiate(bombPrefab);
                bomb.transform.position = CalculateBombingPosition();
                bomb.GetComponent<Bomb>().Use();
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// 폭탄 증가량 계산
        /// </summary>
        /// <param name="status">플레이어 stats</param>
        /// <returns>폭탄 증가량</returns>
        private int CalculateBombIncrease(Status status)
        {
            print("<color=green>" + (status.CurrentItemEfficiency - 1f) + "</color>");

            int bombIncrease = (int)((status.CurrentItemEfficiency - 1f) * 10);

            return bombIncrease;
        }

        /// <summary>
        /// 폭격 위치 계산기
        /// </summary>
        /// <returns></returns>
        private Vector3 CalculateBombingPosition()
        {
            int jitter =  Random.Range(0, 360);
            Vector3 targetPosotion = transform.position + RandomRadiusPosition(jitter);
            targetPosotion.y = 10;

            return targetPosotion;
        }

        /// <summary>
        /// 랜덤 반지름 위치
        /// </summary>
        /// <param name="angle">각도</param>
        /// <returns></returns>
        private Vector3 RandomRadiusPosition(int angle)
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Cos(angle) * Random.Range(0, bombingRadius);
            position.z = Mathf.Sin(angle) * Random.Range(0, bombingRadius);

            return position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, bombingRadius);
        }
    }
}