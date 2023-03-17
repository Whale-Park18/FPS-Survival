using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.Character.Enemy
{

    public class EnemyAnimatorController : MonoBehaviour
    {
        public static string Movement = "isMovement";
        public static string Attack = "isAttack";

        private Animator animator;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        public void SetBool(string name, bool value)
        {
            animator.SetBool(name, value);
        }

        public void DoDie()
        {
            animator.SetTrigger("doDie");
        }
    }
}