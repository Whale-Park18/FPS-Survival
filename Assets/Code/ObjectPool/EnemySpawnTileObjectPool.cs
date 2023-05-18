using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WhalePark18.Character.Enemy;

namespace WhalePark18.ObjectPool
{
    /// <summary>
    /// �� ��ȯ �˸� Ÿ�� ������ƮǮ
    /// </summary>
    /// <remarks>
    /// ������Ʈ ����
    /// </remarks>
    public class EnemySpawnTileObjectPool : MonoObjectPool<EnemySpawnPoint>
    {
        protected override EnemySpawnPoint CreateObject()
        {
            GameObject instance = GameObject.Instantiate(prefab);
            EnemySpawnPoint spawnPoint = instance.GetComponent<EnemySpawnPoint>();
            spawnPoint.Setup(idToAssignToTheObject++);

            return spawnPoint;
        }

       public override EnemySpawnPoint GetObject()
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
        public EnemySpawnPoint GetObject(Vector3 position)
        {
            if (objectPool.Count == 0)
                CreateObjects();

            EnemySpawnPoint enemySpawnPoint = objectPool.Dequeue();
            EnableObject(enemySpawnPoint, position);

            if(isTrackActiveObject)
            {
                activeObjectDictionary.Add(enemySpawnPoint.ID, enemySpawnPoint);
            }

            return enemySpawnPoint;
        }

        public override void ReturnObject(EnemySpawnPoint returnObject)
        {
            /// ��ȯ�� ������Ʈ�� ��Ȱ��ȭ �� ��, ������ƮǮ�� �ִ´�.
            base.ReturnObject(returnObject);

            /// trackActiveObject�� Ȱ��ȭ �Ǿ��� ��, Ȱ��ȭ ������Ʈ ���� �ڷ������� �����Ѵ�.
            if (isTrackActiveObject)
            {
                activeObjectDictionary.Remove(returnObject.ID);
            }
        }
    }
}