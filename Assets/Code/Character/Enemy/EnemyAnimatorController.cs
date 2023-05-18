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
        private Animator animator;

        /****************************************
         * 프로퍼티
         ****************************************/
        public float AnimatorSpeed
        {
            set => animator.speed = value;
            get => animator.speed;
        }

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        /// <summary>
        /// name 이름의 애니메이션 파라이미터의 값을 설정하는 메소드
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetBool(string name, bool value)
        {
            animator.SetBool(name, value);
        }

        /// <summary>
        /// name 이름의 애니메이션 파라미터를 반환하는 메소드
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetBool(string name)
        {
            return animator.GetBool(name);
        }

        /// <summary>
        /// DoDie 애니메이션 트리거를 활성화 시키는 메소드
        /// </summary>
        public void DoDie()
        {
            animator.SetTrigger("doDie");
        }

        /// <summary>
        /// 현재 재생 중인 애니메이션의 normailizedTime을 반환하는 메소드
        /// </summary>
        /// <returns></returns>
        public float CurrentAnimNormalizedTime()
        {
            /// 현재 진행중인 애니메이션 상태
            AnimatorStateInfo currentAnimInfo = animator.GetCurrentAnimatorStateInfo(0);
            return currentAnimInfo.normalizedTime;
        }


        public AnimationClip GetAnimationClip(string animName)
        {
            var animClipArray = animator.GetCurrentAnimatorClipInfo(0);
            AnimationClip clip = null;

            foreach(var animClip in animClipArray)
            {
                if (animClip.clip.name.Equals(animName))
                    clip = animClip.clip;
            }

            return clip;
        }
    }
}