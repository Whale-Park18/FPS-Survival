using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using WhalePark18.Item;
using WhalePark18.Manager;

namespace WhalePark18.Character.Enemy
{
    [RequireComponent(typeof(NavMeshAgent), typeof(CapsuleCollider), typeof(EnemyStatus))]
    [RequireComponent(typeof(EnemyAnimatorController), typeof(EnemyNavMeshAgentController))]
    public class EnemyBase : MonoBehaviour
    {
        /****************************************
         * EnemyBase
         ****************************************/
        [Header("Enemy Info")]
        [SerializeField]
        protected EnemyClass                    enemyClass;             // Enemy 등급
        protected EnemyStatus                   status;
        protected EnemyAnimatorController       animatorController;
        protected EnemyNavMeshAgentController   navMeshAgentController;
        //protected GameObject                    meshObject;

        protected Transform                     target;                 // 공격 타겟

        /****************************************
         * EnemyBase 관리용
         ****************************************/
        private int id;
        [SerializeField]
        private string baseName;

        /****************************************
         * 프로퍼티
         ****************************************/
        public EnemyClass Class => enemyClass;
        public EnemyStatus Status => status;
        public EnemyAnimatorController AnimatorController => animatorController;
        public EnemyNavMeshAgentController NavMeshAgentController => navMeshAgentController;

        public int ID => id;
        public Transform Target => target;

        /// <summary>
        /// Enemy 생성 후, 초기화하는 메소드
        /// </summary>
        /// <param name="id">Enemy ID</param>
        public virtual void Setup(int id, Transform target)
        {
            this.id = id;
            name = $"{baseName}_{id}";

            this.target = target;
        }

        public virtual void TakeDamage(int damage)
        {
            bool isDie = status.DecreaseHp(damage);

            LogManager.ConsoleDebugLog("Enemy.TakeDamge", $"피히량: {damage} / 피격 후 생명력: {status.Hp}");

            if (isDie == true)
            {
                GameObject createItem = ItemManager.Instance.GetRandomItem();
                if (createItem != null)
                {
                    Instantiate(createItem, transform.position, transform.rotation);
                }

                // TODO: EnemyNoraml용 MemoryPool 생성
                //EnemyManager.Instance.ReturnEnemy(this);
                LogManager.ConsoleDebugLog("Enemy", $"{gameObject.name} 사망");
            }
        }
    }
}