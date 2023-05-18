using System;
using UnityEngine;

namespace WhalePark18.Character
{
    [System.Serializable]
    public struct Ability
    {
        [Tooltip("현재 능력치와 최대 능력치가 동일시 됨을 나타내는 플래그")]
        public bool     isCurrentAbilityEqualMaxAbility;
        [Tooltip("기본 능력치")]
        public float    baseAbility;
        [Tooltip("최대 능력치")]
        public float    maxAbility;
        [Tooltip("현재 능력치")]
        public float    currentAbility;

        [Tooltip("최대 특성 레벨")]
        public int      maxTraitLevel;
        [Tooltip("현재 특성 레벨")]
        public int      currentTraitLevel;

        [Tooltip("능력치 증가 연산 방식")]
        public bool     isOperationMultiplication;         // 연산이 곱연산인지 나타내는 플래그
        [Tooltip("능력치 증가 계수 표현 방식")]
        public bool     isIncrementFactorPercentage;       // 증가 계수가 백분율임을 나타내는 플래그
        [Tooltip("증가 계수")]
        public float    incrementFactor;                   // 증가 계수

        public void Reset()
        {
            /// 현재 능력치를 특성 레벨에 맞게 초기화 시킨다.
            currentAbility = baseAbility + CalculateIncrementForTraitLevel(currentTraitLevel);
        }

        public void OnInitialized()
        {
            maxAbility = currentAbility = baseAbility;
            currentTraitLevel = 0;
        }

        /// <summary>
        /// 특성 레벨에 대한 증가량을 계산하는 메소드
        /// </summary>
        /// <param name="level">구하고 싶은 레벨</param>
        /// <returns>특성의 레벨이 level일 때, 증가하는 수치</returns>
        /// <remarks>
        /// level 값이 옳바른 범위을 넘어간다면 0을 반환한다.
        /// </remarks>
        public float CalculateIncrementForTraitLevel(int level)
        {
            int inputLevel = 0 <= level && level <= maxTraitLevel ? level : 0;

            return incrementFactor * inputLevel;
        }

        /// <summary>
        /// 특성 레벨 업 메소드
        /// </summary>
        /// <remarks>
        /// 특성 레벨에 따라 능력치가 상승한다.
        /// </remarks>
        public void TraitLevelUp()
        {
            int nextLevel = currentTraitLevel + 1;

            /// 특성에 의한 능력치 향상 처리
            if(nextLevel <= maxTraitLevel)
            {
                float nextIncrement = CalculateIncrementForTraitLevel(nextLevel);

                /// 연산이 곱연산일 때
                if(isOperationMultiplication)
                {
                    /// 증가 계수가 백분율일 때 곱연산은 (1 + 증가 계수)로 계산해야 하며
                    /// 백분율이 아니라면 증가 계수로 계산해야 한다.
                    /// Ex) 기본 능력치 10일 때, 10% 증가하는 곱연산
                    ///     증가량 = 10 * (1.1) = 11
                    float increment = isIncrementFactorPercentage ? (1 + nextIncrement) : nextIncrement;
                    maxAbility = baseAbility * increment;

                    /// 반올림
                    maxAbility = (float)Math.Round(maxAbility);
                }
                /// 연산이 곱연산이 아닐 때(합연산)
                else
                {
                    maxAbility = baseAbility + nextIncrement;
                }

                /// 현재 능력치와 최대 능력치가 같아야 한다면
                /// currentAbility에 maxAbility를 대입한다.
                if (isCurrentAbilityEqualMaxAbility) currentAbility = maxAbility;
                
                /// 능력치 향상 처리가 끝났다면 현재 특성을 갱신한다.
                currentTraitLevel++;
            }
        }

        /// <summary>
        /// 능력치 향상 메소드
        /// </summary>
        /// <param name="inrementValue"></param>
        public void IncreaseAbility(float inrementValue)
        {
            currentAbility += inrementValue;
        }

        /// <summary>
        /// [인터페이스] 능력치 감소 메소드
        /// </summary>
        /// <param name="lossValue"></param>
        public void DisIncreaseAbility(float lossValue)
        {
            currentAbility -= lossValue;
        }
    }
}
