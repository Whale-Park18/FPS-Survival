using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.SceneManagement;

using WhalePark18.Character.Player;
using WhalePark18.FSM;
using WhalePark18.FSM.State;
using WhalePark18.FSM.State.GameManagerState;
using WhalePark18.Objects;
using WhalePark18.UserSetting;

namespace WhalePark18.Manager
{
    /// TODO: Main -> Game ��ȯ ���� ��� ���濡 ���� ���� ����
    public struct SceneName
    {
        public static string Start => "Start";
        public static string Game => "Game";
    }

    public enum Tag { Target, Metal, ExplosiveBarrel, ImpactNormal, ImpactObstacle, ImpactEnemy, InteractionObejct, Item }

    public enum GameManagerStates { Main = 0, Game, Debug}

    [System.Serializable]
    public class GameOverEvent : UnityEngine.Events.UnityEvent<int, string, int> { }

    public class GameManager : MonoSingleton<GameManager>
    {
        [HideInInspector]
        public GameOverEvent gameOverEvent;

        // State
        private StateBase<GameManager>[] states;
        private StateMachine<GameManager> stateMachine;

        [Header("Main")]
        [SerializeField]
        private GameObject windowMain;

        [Header("Game")]
        [SerializeField]
        private GameObject windowPlayerHUD;

        private bool isPause;            // [�÷���] ������ �Ͻ����� ����� Ȯ��
        private int pauseStack = 0;     // �Ͻ� ���� ��û�� ���� �� ������Ű�� ����
        private Timer timer;
        [SerializeField]
        private int goalNumberOfKill = 1000;
        private ScoreSystem scoreSystem;

        private PlayerController player;

        [Header("Setting Asset")]
        public Mouse MouseSetting;

        [Header("Debug")]
        [Tooltip("����� ���")]
        public bool DebugMode;

        [Tooltip("���� ���� ���")]
        public GameManagerStates StartState;

        [Space(5)]
        [Tooltip("�÷��̾� ��Ȱ��ȭ")]
        public bool DeactivePlayer;

        [Tooltip("�α� ��� ��Ȱ��ȭ")]
        public bool DeactiveLog;

        [Space(5)]
        [Tooltip("�� ��ȯ ����")]
        public bool StopSpawnEnemy;

        [Tooltip("Enemy ���� ��")]
        public bool DevelopingEnemy;

        [Space(5)]
        [Tooltip("����׿� ������")]
        public GameObject WindowDebug;

        /****************************************
         * ������Ƽ
         ****************************************/
        public GameObject       WindowMain => windowMain;
        public GameObject       WindowPlayerHUD => windowPlayerHUD;
        public bool             IsPause => isPause;
        public Timer            Timer => timer;
        public int              GoalNumberOfKill => goalNumberOfKill;
        public PlayerController Player => player;
        public ScoreSystem      ScoreSystem => scoreSystem;

        /// <summary>
        /// GameManager�� �ν��Ͻ�ȭ �� ����, ȣ��Ǵ� �ʱ�ȭ �޼ҵ�
        /// </summary>
        protected override void Awake()
        {
            /// 1. �̱��� �۾�
            base.Awake();

            /// 2. ������Ʈ �� ���� �ʱ�ȭ
            OnVariableInitialized();

            /// 3. ���� ���� �ʱ�ȭ
            OnStateInitialized();
        }

        /// <summary>
        /// ù ������ ����, ȣ��Ǵ� �ʱ�ȭ �޼ҵ�
        /// </summary>
        private void Start()
        {
            // #DEBUG
            if(DebugMode)
            {
                stateMachine.Setup(Instance, states[(int)GameManagerStates.Debug]);
                return;
            }

            /// StateMachine�� owner�� �̱��� ��ü��, �ʱ� ���·� Main���� �����Ѵ�.
            stateMachine.Setup(Instance, states[(int)GameManagerStates.Main]);
        }

        public void OnVariableInitialized()
        {
            timer = GetComponent<Timer>();
            scoreSystem = new ScoreSystem();
            player = GameObject.Find("Player").GetComponent<PlayerController>();
        }

        private void OnStateInitialized()
        {
            /// 3. ���� ���� �ʱ�ȭ
            /// 3.1. ���� �ʱ�ȭ
            /// 3.1.1. ���� ������Ʈ�� ���� ������Ʈ ���� �� �ʱ�ȭ
            GameObject stateObject = new GameObject("States");
            stateObject.transform.parent = this.transform;
            stateObject.transform.localScale = Vector3.zero;

            /// 3.1.2. ���� ������Ʈ ���� �� �ʱ�ȭ
            states = new StateBase<GameManager>[3];
            states[(int)GameManagerStates.Main] = stateObject.AddComponent<Main>();
            states[(int)GameManagerStates.Game] = stateObject.AddComponent<Game>();
            
            // #DEBUG
            if(DebugMode)
                states[(int)GameManagerStates.Debug] = stateObject.AddComponent<StateDebug>();

            /// 3.2. ���� �ӽ� �ʱ�ȭ
            stateMachine = new StateMachine<GameManager>();
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
        public void GameStart()
        {
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
            
            scoreSystem.CalculateScore(goalNumberOfKill, EnemyManager.Instance.KillCountInfo, timer.Time);
            gameOverEvent.Invoke(scoreSystem.TotalScore, timer.ToString(), scoreSystem.KillScore);
        }

        /// <summary>
        /// �������� �����ϴ� �޼ҵ�
        /// </summary>
        public void ReturnMain()
        {
            /// ���� �ٽ� �ε��� �������� �����Ѵ�.
            SceneManager.LoadScene(SceneName.Game.ToString());
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