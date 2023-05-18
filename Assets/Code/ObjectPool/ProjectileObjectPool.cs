using UnityEngine;
using WhalePark18.Character.Enemy;
using WhalePark18.Projectile;

namespace WhalePark18.ObjectPool
{
    /// <summary>
    /// ����ü ������ƮǮ
    /// </summary>
    /// <remarks>
    /// ��쿡 ���� ������Ʈ ����
    /// </remarks>
    public class ProjectileObjectPool : MonoObjectPool<ProjectileBase>
    {
        protected override ProjectileBase CreateObject()
        {
            GameObject instance = GameObject.Instantiate(prefab);
            ProjectileBase projectile = instance.GetComponent<ProjectileBase>();
            projectile.Setup(idToAssignToTheObject++);

            return projectile;
        }

        /// <summary>
        /// ����ü�� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <returns>Vector3(0, 0, 0) ��ġ, ������ �ٶ󺸴� ����ü</returns>
        public override ProjectileBase GetObject()
        {
            return GetObject(Vector3.zero, Quaternion.Euler(Vector3.forward));
        }

        /// <summary>
        /// position ��ġ�� ����ü�� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <param name="position">����ü ��ȯ ��ġ</param>
        /// <returns>positon ��ġ ����ü</returns>
        public ProjectileBase GetObject(Vector3 position)
        {
            /// 1. ������ƮǮ�� ����� ��� ������Ʈ ����
            if (objectPool.Count == 0)
                CreateObjects();

            /// 2. ������ƮǮ���� ������Ʈ�� �ް� Ȱ��ȭ ��Ų��.
            ProjectileBase projectile = objectPool.Dequeue();
            EnableObject(projectile, position);

            /// 3. trackActiveObject�� Ȱ��ȭ �Ǿ��� ��, ������Ʈ�� Ȱ��ȭ ������Ʈ �ڷ����� �߰��Ѵ�.
            if (isTrackActiveObject)
            {
                activeObjectDictionary.Add(projectile.ID, projectile);
            }

            return projectile;
        }

        /// <summary>
        /// position ��ġ�� rotation���� ȸ���� ����ü�� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <param name="position">����ü ��ȯ ��ġ</param>
        /// <param name="rotation">����ü ȸ�� ��</param>
        /// <returns>position ����, rotation���� ȸ���� ����ü</returns>
        public ProjectileBase GetObject(Vector3 position, Quaternion rotation)
        {
            ProjectileBase projectile = GetObject(position);
            projectile.transform.rotation = rotation;

            return projectile;
        }

        public override void ReturnObject(ProjectileBase returnObject)
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