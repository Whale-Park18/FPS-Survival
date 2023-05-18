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
        /// ��ź ������ ���
        /// </summary>
        /// <param name="status">�÷��̾� stats</param>
        /// <returns>��ź ������</returns>
        private int CalculateBombIncrease(PlayerStatus status)
        {
            int bombIncrease = (int)((status.ItemEfficiency.currentAbility - 1f) * 10);
            
            LogManager.ConsoleDebugLog("CalculateBombIncrease",
                $"������ ȿ��: {status.ItemEfficiency.currentAbility - 1f}, ������ ��ź: {bombIncrease}"
            );

            return bombIncrease;
        }

        /// <summary>
        /// ���� ��ġ ����
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
        /// ���� ������ ��ġ
        /// </summary>
        /// <param name="angle">����</param>
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