using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhalePark18.Manager
{
    public class ScoreSystem
    {
        private int killScore;
        private int timeScore;
        private int totalScore;

        public int KillScore => killScore;
        public int TimeScore => timeScore;
        public int TotalScore => totalScore;

        /// <summary>
        /// 점수 계산 메소드
        /// </summary>
        /// <param name="killCountInfo">처치 정보</param>
        /// <param name="playTime">게임 플레이 시간</param>
        public void CalculateScore(int goalNumberOfKill, KillCountInfo killCountInfo, float playTime)
        {
            /// 1. 처치 점수 계산
            ///     * 처치 계수
            ///         * normal    : 1 
            ///         * elite     : 10
            ///         * boss      : 100
            ///     * 계산 방식
            ///         * 등급별 처치 수 * 계수의 합
            int normalFactor    = 1;
            int eliteFactor     = 10;
            int bossFactor      = 100;

            killScore = killCountInfo.normal * normalFactor + killCountInfo.elite * eliteFactor + killCountInfo.boss * bossFactor;

            /// 2. 시간 점수 계산
            ///     * 공식: (3600 - 생존 시간) * (normal 등급 적 처치 수 / 목표 normal 등급 적 처치 수)
            ///     * 이유:
            ///         * 기본적으로 시간 점수는 3600이라는 상수에서 생존 시간을 뺀 값으로 이 값이 클 수록 높은 점수를 얻은 것이다.
            ///         * 하지만 점수를 적을 처치하지 않고 빠르게 죽었을 경우 높은 시간 점수를 얻을 수 있다는 문제점이 존재
            ///         * 때문에 목표 처치 진행률을 시간 점수에 곱해 목표에 근접할 수록 정상적인 시간 점수를 얻을 수있도록 설계함
            ///         * (3600: 1시간을 초로 변환한 숫자)
            timeScore = (3600 - (int)playTime) > 0 ? 3600 - (int)playTime : 0;
            timeScore *= (killCountInfo.normal / goalNumberOfKill);

            totalScore = killScore + timeScore;
        }
    }
}