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
        protected Ability hp;       // [�ɷ�ġ] �����

        /// <summary>
        /// Walk, Run ������Ƽ
        /// </summary>
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;

        /// <summary>
        /// HP ������Ƽ
        /// </summary>
        public ref Ability Hp => ref hp;

        virtual protected void Awake()
        {
            Reset();
        }

        public virtual void Reset()
        {
            /// �����
            hp.Reset();
        }

        /// <summary>
        /// [�������̽�] ����� ���� �޼ҵ�
        /// </summary>
        /// <param name="incrementValue">ȸ����</param>
        public abstract void IncreaseHp(int incrementValue);

        /// <summary>
        /// [�������̽�] ����� ���� �޼ҵ�
        /// </summary>
        /// <param name="lossValue">���ط�</param>
        /// <returns>��� ����</returns>
        public abstract bool DecreaseHp(int lossValue);
    }
}