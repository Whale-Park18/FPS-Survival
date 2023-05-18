using System.Collections;
using System.Reflection;
using UnityEngine;
using WhalePark18.Manager;

namespace WhalePark18.Objects
{
    public class DestructiblePillar : InteractionObject
    {
        [Header("Destructible Paillar")]
        [SerializeField,Tooltip("파괴 가능한 기둥 게임오브젝트")]
        private GameObject destructiblePillar;
        [SerializeField, Tooltip("기둥이 파괴되었을 때, 대체될 게임오브젝트")]
        private GameObject destructiblePillarPieces;
        [SerializeField, Tooltip("복구 시간")]
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
            LogManager.ConsoleDebugLog("DestructiblePillar.TakeDamage", $"현재 생명력: {currentHP} / 피해량: {damage} / 피격후 생명령: {currentHP - damage}");

            currentHP = currentHP - damage < 0 ? 0 : currentHP - damage;

            if (currentHP <= 0 && isDestroyed == false)
            {
                isDestroyed = true;

                boxCollider.enabled = false;

                /// 파편 오브젝트가 현재 오브젝트를 대신한다.
                destructiblePillar.SetActive(false);
                destructiblePillarPieces.SetActive(true);

                StartCoroutine("Repair");
            }
        }

        /// <summary>
        /// 기둥 상태를 복구하는 메소드
        /// </summary>
        /// <returns></returns>
        private IEnumerator Repair()
        {
            yield return new WaitForSeconds(repairTime);

            Reset();
        }

        public void Reset()
        {
            /// 멤버 변수 초기화
            currentHP = maxHP;
            isDestroyed = false;
            
            /// 오브젝트 상태 초기화
            destructiblePillarPieces.SetActive(false);
            destructiblePillar.SetActive(true);
            boxCollider.enabled = true;
        }
    }
}