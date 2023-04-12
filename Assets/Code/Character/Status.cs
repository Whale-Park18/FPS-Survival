using System.Collections;
using System.Dynamic;
using System.Reflection;
using UnityEngine;

namespace WhalePark18.Character
{
    public abstract class Status : MonoBehaviour
    {
        [Header("Walk, Run Speed")]
        [SerializeField]
        protected float walkSpeed;
        [SerializeField]
        protected float runSpeed;

        [Header("Ability")]
        [SerializeField]
        protected Ability hp;       // [능력치] 생명력

        /// <summary>
        /// Walk, Run 프로퍼티
        /// </summary>
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;

        /// <summary>
        /// HP 프로퍼티
        /// </summary>
        public ref Ability Hp => ref hp;

        virtual protected void Awake()
        {
            Reset();
        }

        public virtual void Reset()
        {
            /// 생명력
            hp.Reset();
        }

        /// <summary>
        /// [인터페이스] 생명력 증가 메소드
        /// </summary>
        /// <param name="incrementValue">회복량</param>
        public abstract void IncreaseHp(int incrementValue);

        /// <summary>
        /// [인터페이스] 생명력 감소 메소드
        /// </summary>
        /// <param name="lossValue">피해량</param>
        /// <returns>사망 여부</returns>
        public abstract bool DecreaseHp(int lossValue);
    }
}