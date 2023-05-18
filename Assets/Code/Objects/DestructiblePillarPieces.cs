using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.Objects
{
    /// <summary>
    /// ������Ʈ�� �ı��Ǹ鼭 �߻��� ���ó�� Ŭ����
    /// </summary>
    public class DestructiblePillarPieces : MonoBehaviour
    {
        [Header("�ı��� ����")]
        [SerializeField]
        private GameObject pieces1;
        [SerializeField]
        private GameObject pieces2;
        [SerializeField]
        private GameObject pieces3;
        [SerializeField]
        private GameObject pieces4;

        private Vector3 pieces1Position;
        private Vector3 pieces2Position;
        private Vector3 pieces3Position;
        private Vector3 pieces4Position;

        [Header("�ı� ȿ��")]
        [SerializeField, Tooltip("�ּ� ���߷�")]
        private float minExplosionPower = 500;
        [SerializeField, Tooltip("�ִ� ���߷�")]
        private float maxExplosionPower = 1000;
        [SerializeField, Tooltip("���� �ݰ�")]
        private float explosionRadius;
        [SerializeField, Tooltip("��ġ ������")]
        private Vector3 offset;
        [SerializeField, Tooltip("�ּ� ���� �ڱ�ġ�� ��")]
        private float minUpwardsPower = 500;
        [SerializeField, Tooltip("�ִ� ���� �ڱ�ġ�� ��")]
        private float maxUpwardsPower = 1000;

        private Vector3 explosionPosition;

        private void Awake()
        {
            OnInitialized();
        }

        private void OnInitialized()
        {
            /// �ʱ� ���� ��ġ ����ȭ
            pieces1Position = pieces1.transform.localPosition;
            pieces2Position = pieces2.transform.localPosition;
            pieces3Position = pieces3.transform.localPosition;
            pieces4Position = pieces4.transform.localPosition;

            /// ���� ��ġ �ʱ�ȭ
            explosionPosition = transform.position + offset;
        }

        private void OnEnable()
        {
            ResetPieces();
            Explosion();
        }

        /// <summary>
        /// ���� ��ġ�� �ʱ�ȭ ��Ű�� �޼ҵ�
        /// </summary>
        private void ResetPieces()
        {
            pieces1.transform.localPosition = pieces1Position;
            pieces2.transform.localPosition = pieces2Position;
            pieces3.transform.localPosition = pieces3Position;
            pieces4.transform.localPosition = pieces4Position;
        }

        private void Explosion()
        {
            Rigidbody[] childRigidbodyGruop = gameObject.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < childRigidbodyGruop.Length; i++)
            {
                print(childRigidbodyGruop[i].name);
                childRigidbodyGruop[i].AddExplosionForce(Random.Range(minExplosionPower, maxExplosionPower), explosionPosition, Random.Range(minUpwardsPower, maxUpwardsPower));
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(transform.position + offset, explosionRadius);
        }
    }
}