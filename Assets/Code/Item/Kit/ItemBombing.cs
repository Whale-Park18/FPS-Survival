using UnityEngine;
using System.Reflection;
using WhalePark18.Character.Player;
using WhalePark18.Item.Active;
using WhalePark18.Manager;

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
            int maxCount = numberOfBomb + CalculateBombIncrease(entity.GetComponent<PlayerStatus>());
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
        private int CalculateBombIncrease(PlayerStatus status)
        {
            int bombIncrease = (int)((status.ItemEfficiency.currentAbility - 1f) * 10);
            
            LogManager.ConsoleDebugLog("CalculateBombIncrease",
                $"아이템 효율: {status.ItemEfficiency.currentAbility - 1f}, 증가된 폭탄: {bombIncrease}"
            );

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