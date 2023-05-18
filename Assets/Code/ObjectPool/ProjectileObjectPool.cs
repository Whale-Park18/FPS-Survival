using UnityEngine;
using WhalePark18.Character.Enemy;
using WhalePark18.Projectile;

namespace WhalePark18.ObjectPool
{
    /// <summary>
    /// 투사체 오브젝트풀
    /// </summary>
    /// <remarks>
    /// 경우에 따라 오브젝트 추적
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
        /// 투사체를 반환하는 메소드
        /// </summary>
        /// <returns>Vector3(0, 0, 0) 위치, 정면을 바라보는 투사체</returns>
        public override ProjectileBase GetObject()
        {
            return GetObject(Vector3.zero, Quaternion.Euler(Vector3.forward));
        }

        /// <summary>
        /// position 위치에 투사체를 반환하는 메소드
        /// </summary>
        /// <param name="position">투사체 반환 위치</param>
        /// <returns>positon 위치 투사체</returns>
        public ProjectileBase GetObject(Vector3 position)
        {
            /// 1. 오브젝트풀이 비었는 경우 오브젝트 생성
            if (objectPool.Count == 0)
                CreateObjects();

            /// 2. 오브젝트풀에서 오브젝트를 받고 활성화 시킨다.
            ProjectileBase projectile = objectPool.Dequeue();
            EnableObject(projectile, position);

            /// 3. trackActiveObject가 활성화 되었을 때, 오브젝트를 활성화 오브젝트 자료형에 추가한다.
            if (isTrackActiveObject)
            {
                activeObjectDictionary.Add(projectile.ID, projectile);
            }

            return projectile;
        }

        /// <summary>
        /// position 위치에 rotation으로 회전한 투사체를 반환하는 메소드
        /// </summary>
        /// <param name="position">투사체 반환 위치</param>
        /// <param name="rotation">투사체 회전 값</param>
        /// <returns>position 위차, rotation으로 회전한 투사체</returns>
        public ProjectileBase GetObject(Vector3 position, Quaternion rotation)
        {
            ProjectileBase projectile = GetObject(position);
            projectile.transform.rotation = rotation;

            return projectile;
        }

        public override void ReturnObject(ProjectileBase returnObject)
        {
            /// 반환된 오브젝트를 비활성화 한 후, 오브젝트풀에 넣는다.
            base.ReturnObject(returnObject);

            /// trackActiveObject가 활성화 되었을 때, 활성화 오브젝트 추적 자료형에서 제거한다.
            if (isTrackActiveObject)
            {
                activeObjectDictionary.Remove(returnObject.ID);
            }
        }
    }
}