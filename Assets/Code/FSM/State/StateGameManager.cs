using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhalePark18.Manager;

namespace WhalePark18.FSM.State.GameManagerState
{
    public class Main : StateBase<GameManager>
    {
        public override void Enter(GameManager owner)
        {
            /// TODO:
            /// 1. CanvasMain Ȱ��ȭ
        }

        public override void Execute(GameManager owner)
        {
            
        }

        public override void Exit(GameManager owner)
        {
            /// TODO:
            /// 1. CanvasMain ��Ȱ��ȭ
        }
    }

    public class Game : StateBase<GameManager>
    {
        public override void Enter(GameManager owner)
        {
            /// TODO:
            /// 1. �ʱ�ȭ �۾�
            ///     * �÷��̾� �ʱ�ȭ
            ///         * �÷��̾� ��ġ
            ///         * ����(+ Ư��)
            ///         * ����
            ///     * �� �ʱ�ȭ
            ///         * ���� �ʿ� ���� �ִٸ� ������ƮǮ�� ��ȯ
            ///     * ��� �ʱ�ȭ
            ///         * �ı��� ����� �ִٸ� ����
            ///     * ���� ���ھ� �ʱ�ȭ
            ///         * ���� ���ھ� 0���� �ʱ�ȭ
            /// 
            /// 2. CanvasGame Ȱ��ȭ
        }

        public override void Execute(GameManager owner)
        {
            /// TODO: ���� �÷��̿� �ʿ��� Ȱ��ȭ
            /// * EnemyManager
            ///     * target �ʱ�ȭ
            ///     * ObjectPool Ȱ��ȭ
            /// * Timer Ȱ��ȭ
            EnemyManager.Instance.Setup(owner.Player.transform);
            EnemyManager.Instance.SpawnEnemyBackground(EnemyClass.Normal);

            owner.Timer.Run();
        }

        public override void Exit(GameManager owner)
        {
            /// TODO:
            /// 1. CanvasGame ��Ȱ��ȭ
        }
    }
}