using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using WhalePark18.Character.Player;
using WhalePark18.FSM;
using WhalePark18.FSM.State;
using WhalePark18.FSM.State.GameManagerState;
using WhalePark18.Objects;

namespace WhalePark18.Manager
{
    /// TODO: Main -> Game ��ȯ ���� ��� ���濡 ���� ���� ����
    public struct SceneName
    {
        public static string Start => "StartScene";
        public static string Game => "GameScene";
    }

    public enum Tag { Target, Metal, ExplosiveBarrel, ImpactNormal, ImpactObstacle, ImpactEnemy, InteractionObejct, Item }

    public enum GameManagerStates { Main = 0, Game, }

    [System.Serializable]
    public class GameOverEvent : UnityEngine.Events.UnityEvent<int, string, int> { }

    public class GameManager : MonoSingleton<GameManager>
    {
        [HideInInspector]
        public GameOverEvent gameOverEvent;

        /****************************************
         * GameManager Base
         ****************************************/
        private StateBase<GameManager>[]    states;
        private StateMachine<GameManager>   stateMachine;

        /****************************************
         * Main
         ****************************************/
        [SerializeField]
        private GameObject                  canvasMain;

        /****************************************
         * Game
         ****************************************/
        [SerializeField]
        private GameObject                  canvasGame;

        private bool                        isPause;            // [�÷���] ������ �Ͻ����� ����� Ȯ��
        private int                         pauseStack = 0;     // �Ͻ� ���� ��û�� ���� �� ������Ű�� ����
        private Timer                       timer;
        [SerializeField]
        private int                         goalNumberOfKill = 1000;
        private ScoreSystem                 scoreSystem;

        private GameObject                  player;
        private List<DestructiblePillar>    pillarList;
        [SerializeField]
        private LayerMask                   pillarLayerMask;

        /****************************************
         * ������Ƽ
         ****************************************/
        public bool IsPause => isPause;
        public int GoalNumberOfKill => goalNumberOfKill;
        public Timer Timer => timer;
        public GameObject Player => player;

        /// <summary>
        /// GameManager�� �ν��Ͻ�ȭ �� ����, ȣ��Ǵ� �ʱ�ȭ �޼ҵ�
        /// </summary>
        private void Awake()
        {
            /// 1. �̱��� �۾�
            if(Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }

            /// 2. ������Ʈ �� ���� �ʱ�ȭ
            timer = GetComponent<Timer>();
            scoreSystem = new ScoreSystem();

            /// 3. ���� �ʱ�ȭ
            /// 3.1. ���� ������Ʈ�� ���� ������Ʈ ���� �� �ʱ�ȭ
            GameObject stateObject = new GameObject("States");
            stateObject.transform.parent = this.transform;
            stateObject.transform.localScale = Vector3.zero;

            /// 3.2. ���� ������Ʈ ���� �� �ʱ�ȭ
            states = new StateBase<GameManager>[4];
            states[(int)GameManagerStates.Main] = stateObject.AddComponent<Main>();
            states[(int)GameManagerStates.Game] = stateObject.AddComponent<Game>();
        }

        /// <summary>
        /// ù ������ ����, ȣ��Ǵ� �ʱ�ȭ �޼ҵ�
        /// </summary>
        private void Start()
        {
            /// StateMachine�� owner�� �̱��� ��ü��, �ʱ� ���·� Main���� �����Ѵ�.
            stateMachine = new StateMachine<GameManager>();
            stateMachine.Setup(Instance, states[(int)GameManagerStates.Main]);

            /// 1. ���� ������Ʈ �ʱ�ȭ
            /// 1.1. �÷��̾� �ʱ�ȭ
            player = GameObject.Find("Player");
            
            /// 1.2. ��� �ʱ�ȭ
            pillarList = new List<DestructiblePillar>();
            Collider[] colliders = Physics.OverlapSphere(Vector3.zero, 150f, pillarLayerMask);
            foreach(Collider collider in colliders)
            {
                DestructiblePillar pillar = collider.GetComponent<DestructiblePillar>();
                if(pillar != null ) pillarList.Add(pillar);
            }
        }

        /// <summary>
        /// ���¸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="newState">������ ���ο� ����</param>
        private void ChangeState(GameManagerStates newState)
        {
            stateMachine.ChangeState(states[(int)newState]);
        }

        /// <summary>
        /// ���� ���� �޼ҵ�
        /// </summary>
        private void GameStart()
        {
            /// TODO: Game(StateBase).Execute()���� ����� ����
            EnemyManager.Instance.Setup(player.transform);
            //EnemyManager.Instance.SpawnBackgroundEnemy();
            timer.Run();

            ChangeState(GameManagerStates.Game);
        }

        /// <summary>
        /// �÷��̾� ��� �� ��ǥ �޼��� ȣ��Ǵ� �޼ҵ�
        /// </summary>
        public void GameOver()
        {
            /// ���� ����� �ʿ��� ��ġ ����
            ///     * Ŀ�� Ȱ��ȭ
            ///     * �Ͻ� ����(�˸�â �ڷ� ĳ���͵��� �������� �ʰ�)
            ///     * Ÿ�̸� ����
            ///     * PlayerController�� ��Ȱ��ȭ�� �÷��̾� ���ӿ�����Ʈ�� ������ �� ���� �Ѵ�.
            SetCursorActive(true);
            Pause();
            timer.Stop();
            player.GetComponent<PlayerController>().enabled = false;

            scoreSystem.CalculateScore(EnemyManager.Instance.KillCountInfo, timer.Time);
            gameOverEvent.Invoke(scoreSystem.TotalScore, timer.ToString(), scoreSystem.KillScore);
        }

        /// <summary>
        /// ���� �ʱ�ȭ �޼ҵ�
        /// </summary>
        public void GameReset()
        {
            /// 1. �÷��̾� �ʱ�ȭ
            //player.GetComponent<PlayerController>().enabled = true;
            var playerController = player.GetComponent<PlayerController>();
            playerController.SetActive(false);
            playerController.Reset();

            /// 2. �� �ʱ�ȭ
            EnemyManager.Instance.Reset();
            
            /// 3. ��� �ʱ�ȭ
            foreach(var pillar in pillarList)
            {
                pillar.Reset();
            }
        }

        /// <summary>
        /// ���� ���� �޼ҵ�
        /// </summary>
        public void GameExit()
        {
            Application.Quit();
        }

        /// <summary>
        /// �Ͻ� ���� �޼ҵ�
        /// </summary>
        public void Pause()
        {
            /// �Ͻ� ���� ������ 1 ������Ų��.
            pauseStack++;
            isPause = pauseStack > 0;

            if (isPause)
                Time.timeScale = 0f;
        }

        /// <summary>
        /// �̾��ϱ� �޼ҵ�
        /// </summary>
        public void Resume()
        {
            /// �Ͻ� ���� ������ 1 ���ҽ�Ų��(��, ������ �������� �ʴ´�).
            pauseStack = pauseStack - 1 <= 0 ? 0 : pauseStack - 1;
            isPause = pauseStack > 0;

            if (isPause == false)
                Time.timeScale = 1f;
        }

        /// <summary>
        /// ���콺 Ŀ�� Ȱ��ȭ ���� �޼ҵ�
        /// </summary>
        /// <param name="active">Ȱ��ȭ ����</param>
        public void SetCursorActive(bool active)
        {
            if (active)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        /// <summary>
        /// �÷��� �ð��� ���ڿ��� ��ȯ�ϴ� �޼ҵ�
        /// </summary>
        /// <returns></returns>
        public string TimeToString()
        {
            return timer.ToString();
        }
    }
}