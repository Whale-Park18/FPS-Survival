using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.Character.Enemy
{
    public class EnemyStatus : Status
    {
        /// <summary>
        /// [인터페이스] 생명력 증가 메소드
        /// </summary>
        /// <param name="incrementValue">회복량</param>
        public override void IncreaseHp(int incrementValue)
        {
            hp.currentAbility = hp.currentAbility + incrementValue > hp.maxAbility ? hp.maxAbility : hp.currentAbility + incrementValue;
        }

        /// <summary>
        /// [인터페이스] 생명력 감소 메소드
        /// </summary>
        /// <param name="lossValue">피해량</param>
        /// <returns>사망 여부</returns>
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