#define DEBUG

using TMPro;
using UnityEngine.Assertions.Must;
using WhalePark18.Manager;

namespace WhalePark18.FSM.State.GameManagerState
{
    public class Main : StateBase<GameManager>
    {
        public override void Enter(GameManager owner)
        {
            LogManager.ConsoleDebugLog("Main<GameManager>", "Enter");

            owner.Resume();
            owner.SetCursorActive(true);
            owner.WindowMain.SetActive(true);
            owner.Player.SetActive(false);
        }

        public override void Execute(GameManager owner)
        {
            LogManager.ConsoleDebugLog("Main<GameManager>", "Execute");
        }

        public override void Exit(GameManager owner)
        {
            LogManager.ConsoleDebugLog("Main<GameManager>", "Exit");

            owner.WindowMain.SetActive(false);
        }
    }

    public class Game : StateBase<GameManager>
    {
        public override void Enter(GameManager owner)
        {
            LogManager.ConsoleDebugLog("Game<GameManager>", "Enter");
      
            owner.SetCursorActive(false);
            owner.Player.SetActive(true);
            owner.WindowPlayerHUD.SetActive(true);

            Execute(owner);
        }

        public override void Execute(GameManager owner)
        {
            LogManager.ConsoleDebugLog("Game<GameManager>", "Execute");

            owner.Timer.Run();
            EnemyManager.Instance.Setup(owner.Player.transform);
            EnemyManager.Instance.SpawnEnemyBackground(EnemyClass.Normal);
        }

        public override void Exit(GameManager owner)
        {
            LogManager.ConsoleDebugLog("Game<GameManager>", "Exit");
      
            owner.WindowPlayerHUD.SetActive(false);
        }
    }

    /// <summary>
    /// Debug¿ë »óÅÂ
    /// </summary>
    public class StateDebug : StateBase<GameManager>
    {
        public override void Enter(GameManager owner)
        {
            LogManager.ConsoleDebugLog("StateDebug<GameManager>", "Enter");

            if(owner.StartState.Equals(GameManagerStates.Main))
            {
                owner.Resume();
                owner.SetCursorActive(true);
                owner.WindowMain.SetActive(true);
                owner.Player.SetActive(false);
            }
            else if(owner.StartState.Equals(GameManagerStates.Game))
            {
                owner.SetCursorActive(false);
                owner.Player.SetActive(true);
                owner.WindowPlayerHUD.SetActive(true);
            }
            else if(owner.StartState.Equals(GameManagerStates.Debug))
            {
                if (owner.DeactivePlayer)
                {
                    owner.SetCursorActive(true);
                    owner.Player.SetActive(false);
                }
                else
                {
                    owner.SetCursorActive(false);
                    owner.Player.SetActive(true);
                }
                owner.WindowDebug.SetActive(true);
            }

            Execute(owner);
        }

        public override void Execute(GameManager owner)
        {
            if (owner.StartState.Equals(GameManagerStates.Main))
            {

            }
            else if (owner.StartState.Equals(GameManagerStates.Game))
            {
                owner.Timer.Run();
                EnemyManager.Instance.Setup(owner.Player.transform);
                if(owner.StopSpawnEnemy == false)
                    EnemyManager.Instance.SpawnEnemyBackground(EnemyClass.Normal);
            }
            else if (owner.StartState.Equals(GameManagerStates.Debug))
            {
                
            }
        }

        public override void Exit(GameManager owner)
        {
            if (owner.StartState.Equals(GameManagerStates.Main))
            {
                owner.WindowMain.SetActive(false);
            }
            else if (owner.StartState.Equals(GameManagerStates.Game))
            {
                owner.WindowPlayerHUD.SetActive(false);
            }
            else if (owner.StartState.Equals(GameManagerStates.Debug))
            {

            }
        }
    }
}