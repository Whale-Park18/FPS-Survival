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
            /// 1. CanvasMain 활성화
        }

        public override void Execute(GameManager owner)
        {
            
        }

        public override void Exit(GameManager owner)
        {
            /// TODO:
            /// 1. CanvasMain 비활성화
        }
    }

    public class Game : StateBase<GameManager>
    {
        public override void Enter(GameManager owner)
        {
            /// TODO:
            /// 1. 초기화 작업
            ///     * 플레이어 초기화
            ///         * 플레이어 위치
            ///         * 스탯(+ 특성)
            ///         * 무기
            ///     * 적 초기화
            ///         * 현재 맵에 적이 있다면 오브젝트풀에 반환
            ///     * 기둥 초기화
            ///         * 파괴된 기둥이 있다면 복구
            ///     * 게임 스코어 초기화
            ///         * 게임 스코어 0으로 초기화
            /// 
            /// 2. CanvasGame 활성화
        }

        public override void Execute(GameManager owner)
        {
            /// TODO: 게임 플레이에 필요한 활성화
            /// * EnemyManager
            ///     * target 초기화
            ///     * ObjectPool 활성화
            /// * Timer 활성화
            EnemyManager.Instance.Setup(owner.Player.transform);
            EnemyManager.Instance.SpawnEnemyBackground(EnemyClass.Normal);

            owner.Timer.Run();
        }

        public override void Exit(GameManager owner)
        {
            /// TODO:
            /// 1. CanvasGame 비활성화
        }
    }
}