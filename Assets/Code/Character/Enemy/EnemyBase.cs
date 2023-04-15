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
        protected EnemyClass                    enemyClass;             // Enemy ���
        protected EnemyStatus                   status;
        protected EnemyAnimatorController       animatorController;
        protected EnemyNavMeshAgentController   navMeshAgentController;
        //protected GameObject                    meshObject;

        protected Transform                     target;                 // ���� Ÿ��

        /****************************************
         * EnemyBase ������
         ****************************************/
        private int id;
        [SerializeField]
        private string baseName;

        /****************************************
         * ������Ƽ
         ****************************************/
        public EnemyClass Class => enemyClass;
        public EnemyStatus Status => status;
        public EnemyAnimatorController AnimatorController => animatorController;
        public EnemyNavMeshAgentController NavMeshAgentController => navMeshAgentController;

        public int ID => id;
        public Transform Target => target;

        /// <summary>
        /// Enemy ���� ��, �ʱ�ȭ�ϴ� �޼ҵ�
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

            LogManager.ConsoleDebugLog("Enemy.TakeDamge", $"������: {damage} / �ǰ� �� �����: {status.Hp}");

            if (isDie == true)
            {
                GameObject createItem = ItemManager.Instance.GetRandomItem();
                if (createItem != null)
                {
                    Instantiate(createItem, transform.position, transform.rotation);
                }

                // TODO: EnemyNoraml�� MemoryPool ����
                //EnemyManager.Instance.ReturnEnemy(this);
                LogManager.ConsoleDebugLog("Enemy", $"{gameObject.name} ���");
            }
        }
    }
}