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
        /// ���� ��� �޼ҵ�
        /// </summary>
        /// <param name="killCountInfo">óġ ����</param>
        /// <param name="playTime">���� �÷��� �ð�</param>
        public void CalculateScore(int goalNumberOfKill, KillCountInfo killCountInfo, float playTime)
        {
            /// 1. óġ ���� ���
            ///     * óġ ���
            ///         * normal    : 1 
            ///         * elite     : 10
            ///         * boss      : 100
            ///     * ��� ���
            ///         * ��޺� óġ �� * ����� ��
            int normalFactor    = 1;
            int eliteFactor     = 10;
            int bossFactor      = 100;

            killScore = killCountInfo.normal * normalFactor + killCountInfo.elite * eliteFactor + killCountInfo.boss * bossFactor;

            /// 2. �ð� ���� ���
            ///     * ����: (3600 - ���� �ð�) * (normal ��� �� óġ �� / ��ǥ normal ��� �� óġ ��)
            ///     * ����:
            ///         * �⺻������ �ð� ������ 3600�̶�� ������� ���� �ð��� �� ������ �� ���� Ŭ ���� ���� ������ ���� ���̴�.
            ///         * ������ ������ ���� óġ���� �ʰ� ������ �׾��� ��� ���� �ð� ������ ���� �� �ִٴ� �������� ����
            ///         * ������ ��ǥ óġ ������� �ð� ������ ���� ��ǥ�� ������ ���� �������� �ð� ������ ���� ���ֵ��� ������
            ///         * (3600: 1�ð��� �ʷ� ��ȯ�� ����)
            timeScore = (3600 - (int)playTime) > 0 ? 3600 - (int)playTime : 0;
            timeScore *= (killCountInfo.normal / goalNumberOfKill);

            totalScore = killScore + timeScore;
        }
    }
}