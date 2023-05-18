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
         * ������Ƽ
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
        /// name �̸��� �ִϸ��̼� �Ķ��̹����� ���� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetBool(string name, bool value)
        {
            animator.SetBool(name, value);
        }

        /// <summary>
        /// name �̸��� �ִϸ��̼� �Ķ���͸� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetBool(string name)
        {
            return animator.GetBool(name);
        }

        /// <summary>
        /// DoDie �ִϸ��̼� Ʈ���Ÿ� Ȱ��ȭ ��Ű�� �޼ҵ�
        /// </summary>
        public void DoDie()
        {
            animator.SetTrigger("doDie");
        }

        /// <summary>
        /// ���� ��� ���� �ִϸ��̼��� normailizedTime�� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <returns></returns>
        public float CurrentAnimNormalizedTime()
        {
            /// ���� �������� �ִϸ��̼� ����
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