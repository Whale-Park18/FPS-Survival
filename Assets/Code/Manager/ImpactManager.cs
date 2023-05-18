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

        [SerializeField, Tooltip("����Ʈ ������ƮǮ ����: Normal, Obstacle, Enemy, InteractionObject")]
        ImpactObjectPool[] impactObjectPoolGroup;

        /// <summary>
        /// ����Ʈ ���� �������̽�(���� ����)
        /// </summary>
        /// <param name="hit">Raycast ����</param>
        public void SpawnImpact(RaycastHit hit)
        {
            LogManager.ConsoleDebugLog("SpawnImpact", $"HitInfo Name{hit.transform.name}, Tag{hit.transform.tag}");

            Impact impact;

            /// �ε��� ������Ʈ�� Tag ������ ���� �ٸ��� ó��
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
        /// ����Ʈ ���� �������̽�(�浹ü ����)
        /// </summary>
        /// <param name="other">�浹ü ����</param>
        /// <param name="colliderTransform">�浹ü Transform</param>
        public void SpawnImpact(Collider other, Transform colliderTransform)
        {
            Impact impact;

            /// �ε��� ������Ʈ�� Tag ������ ���� �ٸ��� ó��
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
        /// ����Ʈ ��ȯ �������̽�
        /// </summary>
        /// <param name="impactType">��ȯ�� ����Ʈ Ÿ��</param>
        /// <param name="returnImpact">��ȯ�� ����Ʈ</param>
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