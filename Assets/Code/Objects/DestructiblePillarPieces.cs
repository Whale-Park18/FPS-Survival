using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.Objects
{
    /// <summary>
    /// 오브젝트가 파괴되면서 발생한 충격처리 클래스
    /// </summary>
    public class DestructiblePillarPieces : MonoBehaviour
    {
        [Header("파괴된 조각")]
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

        [Header("파괴 효과")]
        [SerializeField, Tooltip("최소 폭발력")]
        private float minExplosionPower = 500;
        [SerializeField, Tooltip("최대 폭발력")]
        private float maxExplosionPower = 1000;
        [SerializeField, Tooltip("폭발 반경")]
        private float explosionRadius;
        [SerializeField, Tooltip("위치 보정값")]
        private Vector3 offset;
        [SerializeField, Tooltip("최소 위로 솟구치는 힘")]
        private float minUpwardsPower = 500;
        [SerializeField, Tooltip("최대 위로 솟구치는 힘")]
        private float maxUpwardsPower = 1000;

        private Vector3 explosionPosition;

        private void Awake()
        {
            OnInitialized();
        }

        private void OnInitialized()
        {
            /// 초기 조각 위치 초지화
            pieces1Position = pieces1.transform.localPosition;
            pieces2Position = pieces2.transform.localPosition;
            pieces3Position = pieces3.transform.localPosition;
            pieces4Position = pieces4.transform.localPosition;

            /// 폭발 위치 초기화
            explosionPosition = transform.position + offset;
        }

        private void OnEnable()
        {
            ResetPieces();
            Explosion();
        }

        /// <summary>
        /// 조각 위치를 초기화 시키는 메소드
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