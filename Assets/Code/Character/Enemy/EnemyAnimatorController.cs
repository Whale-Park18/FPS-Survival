using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using WhalePark18.Manager;

namespace WhalePark18.Character.Enemy
{

    public class EnemyAnimatorController : MonoBehaviour
    {
        public static string Movement = "isMovement";
        public static string Attack = "isAttack";

        private Animator animator;

        public float NormalizedTime => animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        /// <summary>
        /// 애니메이션 파라이미터를 설정하는 메소드
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetBool(string name, bool value)
        {
            animator.SetBool(name, value);
        }

        /// <summary>
        /// DoDie 애니메이션 트리거를 활성화 시키는 메소드
        /// </summary>
        public void DoDie()
        {
            animator.SetTrigger("doDie");
        }

        /// <summary>
        /// Attack 애니메이션이 종료되었는지 확인하는 메소드
        /// </summary>
        /// <returns>Attack 애니메이션 종료 여부</returns>
        public bool IsAttackAnimEnd()
        {
            /// 현재 진행중인 애니메이션 상태
            AnimatorStateInfo currentAnimInfo = animator.GetCurrentAnimatorStateInfo(0);

            /// 현재 진행중인 애니메이션이 "Attack" 애니메이션인지?
            bool isAttackAnim = currentAnimInfo.IsName("Attack");

            /// 애니메이션 진행률이 90 ~ 100% 사이인지?
            float currentAnimNormalizedTime = currentAnimInfo.normalizedTime - (int)currentAnimInfo.normalizedTime;
            bool isCurrentAnimNormalizedTimeValidRange = 0.98f <= currentAnimNormalizedTime && currentAnimNormalizedTime <= 1f;

            return isAttackAnim && isCurrentAnimNormalizedTimeValidRange;
        }

        public float CurrentAnimNormalizedTime()
        {
            /// 현재 진행중인 애니메이션 상태
            AnimatorStateInfo currentAnimInfo = animator.GetCurrentAnimatorStateInfo(0);
            return currentAnimInfo.normalizedTime;
        }
    }
}