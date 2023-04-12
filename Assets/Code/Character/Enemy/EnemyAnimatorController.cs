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
        /// �ִϸ��̼� �Ķ��̹��͸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetBool(string name, bool value)
        {
            animator.SetBool(name, value);
        }

        /// <summary>
        /// DoDie �ִϸ��̼� Ʈ���Ÿ� Ȱ��ȭ ��Ű�� �޼ҵ�
        /// </summary>
        public void DoDie()
        {
            animator.SetTrigger("doDie");
        }

        /// <summary>
        /// Attack �ִϸ��̼��� ����Ǿ����� Ȯ���ϴ� �޼ҵ�
        /// </summary>
        /// <returns>Attack �ִϸ��̼� ���� ����</returns>
        public bool IsAttackAnimEnd()
        {
            /// ���� �������� �ִϸ��̼� ����
            AnimatorStateInfo currentAnimInfo = animator.GetCurrentAnimatorStateInfo(0);

            /// ���� �������� �ִϸ��̼��� "Attack" �ִϸ��̼�����?
            bool isAttackAnim = currentAnimInfo.IsName("Attack");

            /// �ִϸ��̼� ������� 90 ~ 100% ��������?
            float currentAnimNormalizedTime = currentAnimInfo.normalizedTime - (int)currentAnimInfo.normalizedTime;
            bool isCurrentAnimNormalizedTimeValidRange = 0.98f <= currentAnimNormalizedTime && currentAnimNormalizedTime <= 1f;

            return isAttackAnim && isCurrentAnimNormalizedTimeValidRange;
        }

        public float CurrentAnimNormalizedTime()
        {
            /// ���� �������� �ִϸ��̼� ����
            AnimatorStateInfo currentAnimInfo = animator.GetCurrentAnimatorStateInfo(0);
            return currentAnimInfo.normalizedTime;
        }
    }
}