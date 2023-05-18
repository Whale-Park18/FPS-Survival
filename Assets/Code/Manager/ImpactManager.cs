using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WhalePark18.ObjectPool;

namespace WhalePark18.Manager
{
    public enum ImpactType { Normal, Obstacle, Enemy, InteractionObject }

    public class ImpactManager : MonoSingleton<ImpactManager>
    {

        [SerializeField, Tooltip("임펙트 오브젝트풀 순서: Normal, Obstacle, Enemy, InteractionObject")]
        ImpactObjectPool[] impactObjectPoolGroup;

        /// <summary>
        /// 임펙트 생성 인터페이스(광선 전용)
        /// </summary>
        /// <param name="hit">Raycast 정보</param>
        public void SpawnImpact(RaycastHit hit)
        {
            LogManager.ConsoleDebugLog("SpawnImpact", $"HitInfo Name{hit.transform.name}, Tag{hit.transform.tag}");

            Impact impact;

            /// 부딪힌 오브젝트의 Tag 정보에 따라 다르게 처리
            if (hit.transform.CompareTag("ImpactNormal"))
            {
                impact = impactObjectPoolGroup[(int)ImpactType.Normal].GetObject(hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.CompareTag("ImpactObstacle"))
            {
                impact = impactObjectPoolGroup[(int)ImpactType.Obstacle].GetObject(hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.CompareTag("ImpactEnemy"))
            {
                impact = impactObjectPoolGroup[(int)ImpactType.Enemy].GetObject(hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.CompareTag("InteractionObject"))
            {
                Color color = hit.transform.GetComponentInChildren<MeshRenderer>().material.color;
                impact = impactObjectPoolGroup[(int)ImpactType.InteractionObject].GetObject(hit.point, Quaternion.LookRotation(hit.normal), color);
            }
        }

        /// <summary>
        /// 임펙트 생성 인터페이스(충돌체 전용)
        /// </summary>
        /// <param name="other">충돌체 정보</param>
        /// <param name="colliderTransform">충돌체 Transform</param>
        public void SpawnImpact(Collider other, Transform colliderTransform)
        {
            Impact impact;

            /// 부딪힌 오브젝트의 Tag 정보에 따라 다르게 처리
            if (other.CompareTag("ImpactNormal"))
            {
                impact = impactObjectPoolGroup[(int)ImpactType.Normal].GetObject(colliderTransform.position, Quaternion.Inverse(colliderTransform.rotation));
            }
            else if (other.CompareTag("ImpactObstacle"))
            {
                impact = impactObjectPoolGroup[(int)ImpactType.Obstacle].GetObject(colliderTransform.position, Quaternion.Inverse(colliderTransform.rotation));
            }
            else if (other.CompareTag("ImpactEnemy"))
            {
                impact = impactObjectPoolGroup[(int)ImpactType.Enemy].GetObject(colliderTransform.position, Quaternion.Inverse(colliderTransform.rotation));
            }
            else if (other.CompareTag("InteractionObject"))
            {
                Color color = other.transform.GetComponentInChildren<MeshRenderer>().material.color;
                impact = impactObjectPoolGroup[(int)ImpactType.InteractionObject].GetObject(colliderTransform.position, Quaternion.Inverse(colliderTransform.rotation), color);
            }
        }

        /// <summary>
        /// 임펙트 반환 인터페이스
        /// </summary>
        /// <param name="impactType">반환할 임펙트 타입</param>
        /// <param name="returnImpact">반환할 임펙트</param>
        public void ReturnImpact(ImpactType impactType, Impact returnImpact)
        {
            switch(impactType)
            {
                case ImpactType.Normal:
                    impactObjectPoolGroup[(int)ImpactType.Normal].ReturnObject(returnImpact);
                    break;

                case ImpactType.Obstacle:
                    impactObjectPoolGroup[(int)ImpactType.Obstacle].ReturnObject(returnImpact);
                    break;

                case ImpactType.Enemy:
                    impactObjectPoolGroup[(int)ImpactType.Enemy].ReturnObject(returnImpact);
                    break;

                case ImpactType.InteractionObject:
                    impactObjectPoolGroup[(int)ImpactType.InteractionObject].ReturnObject(returnImpact);
                    break;
            }
        }
    }
}