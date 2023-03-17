using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.Character.Enemy
{
    public class EnemyStatus : Status
    {
        /// <summary>
        /// [�������̽�] ����� ���� �޼ҵ�
        /// </summary>
        /// <param name="incrementValue">ȸ����</param>
        public override void IncreaseHp(int incrementValue)
        {
            hp.currentAbility = hp.currentAbility + incrementValue > hp.maxAbility ? hp.maxAbility : hp.currentAbility + incrementValue;
        }

        /// <summary>
        /// [�������̽�] ����� ���� �޼ҵ�
        /// </summary>
        /// <param name="lossValue">���ط�</param>
        /// <returns>��� ����</returns>
        public override bool DecreaseHp(int lossValue)
        {
            hp.currentAbility = hp.currentAbility - lossValue > 0 ? hp.currentAbility - lossValue : 0;

            if (hp.currentAbility <= 0)
            {
                return true;
            }

            return false;
        }
    }
}