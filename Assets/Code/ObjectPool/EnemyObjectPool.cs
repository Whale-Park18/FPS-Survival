using UnityEngine;

using WhalePark18.Character.Enemy;
using WhalePark18.Manager;

namespace WhalePark18.ObjectPool
{
    /// <summary>
    /// Enemy ������ƮǮ
    /// </summary>
    /// <remarks>
    /// ������Ʈ ����
    /// </remarks>
    public class EnemyObjectPool : MonoObjectPool<EnemyBase>
    {
        private Transform target;

        public Transform Target { get; set; }

        protected override EnemyBase CreateObject()
        {
            GameObject instance = GameObject.Instantiate(prefab);
            EnemyBase enemy = instance.GetComponent<EnemyBase>();
            enemy.Setup(idToAssignToTheObject++, target);

            return enemy;
        }

        /// <summary>
        /// ������Ʈ ��ȯ �������̽�
        /// </summary>
        /// <returns>������Ʈ</returns>
        /// <remarks>
        /// Vector3(0, 0, 0) ��ġ ������Ʈ
        /// </remarks>
        public override EnemyBase GetObject()
        {
            return GetObject(Vector3.zero);
        }

        /// <summary>
        /// ������Ʈ ��ȯ �������̽�
        /// </summary>
        /// <param name="position">������ ��ġ</param>
        /// <returns>������Ʈ</returns>
        /// <remarks>
        /// position ��ġ ������Ʈ
        /// </remarks>
        public EnemyBase GetObject(Vector3 position)
        {
            /// 1. ������ƮǮ�� ����� ��� ������Ʈ ����
            if (objectPool.Count == 0)
                CreateObjects();

            /// 2. ������ƮǮ���� ������Ʈ�� �ް� Ȱ��ȭ ��Ų��.
            EnemyBase enemy = objectPool.Dequeue();
            EnableObject(enemy, position);

            /// 3. trackActiveObject�� Ȱ��ȭ �Ǿ��� ��, ������Ʈ�� Ȱ��ȭ ������Ʈ �ڷ����� �߰��Ѵ�.
            if (isTrackActiveObject)
            {
                activeObjects.Add(enemy.ID, enemy);
            }

            return enemy;
        }

        public override void ReturnObject(EnemyBase returnObject)
        {
            /// ��ȯ�� ������Ʈ�� ��Ȱ��ȭ �� ��, ������ƮǮ�� �ִ´�.
            base.ReturnObject(returnObject);

            /// trackActiveObject�� Ȱ��ȭ �Ǿ��� ��, Ȱ��ȭ ������Ʈ ���� �ڷ������� �����Ѵ�.
            if(isTrackActiveObject)
            {
                activeObjects.Remove(returnObject.ID);
            }
        }
    }
}