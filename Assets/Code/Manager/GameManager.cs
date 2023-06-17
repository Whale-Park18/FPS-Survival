using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

using WhalePark18.Character.Player;
using WhalePark18.FSM;
using WhalePark18.FSM.State;
using WhalePark18.FSM.State.GameManagerState;
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
        public GameOverEvent GameOverEvent;

        [Header("Window")]
        [SerializeField]
        private GameObject      _windowMain;
        
        [SerializeField]
        private GameObject      _windowPlayerHUD;

        public GameObject       WindowMain => _windowMain;

        public GameObject       WindowPlayerHUD => _windowPlayerHUD;

        [Header("Game")]
        [Tooltip("��ǥ óġ ��(�Ϲ� ���)")]
        public int GoalNumberOfKill = 1000;

        public bool             IsPause => _isPause;

        public Timer            Timer => _timer;

        public PlayerController Player => _player;

        [Header("Mouse Setting")]
        [Tooltip("���콺 ���� ���� ���")]
        public string DetailPath;

        [Tooltip("Json ���� �̸�")]
        public string JsonFileName;

        public Mouse MouseSetting { get => _mouseSetting; }
        
        // Game
        private bool                _isPause;            // [�÷���] ������ �Ͻ����� ����� Ȯ��
        private int                 _pauseStack = 0;     // �Ͻ� ���� ��û�� ���� �� ������Ű�� ����
        private Timer               _timer;
        private ScoreSystem         _scoreSystem;
        private PlayerController    _player;

        // Mouse Setting
        private string _path;
        private Mouse _mouseSetting;

        // State
        private StateBase<GameManager>[]    _states;
        private StateMachine<GameManager>   _stateMachine;

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

            _path = Application.dataPath + $"/{DetailPath}/{JsonFileName}.json";
            if(File.Exists(_path))
            {
                var json = File.ReadAllText(_path);
                _mouseSetting = JsonUtility.FromJson<Mouse>(json);
            }
            else
            {
                var directoryPath = Application.dataPath + $"/{DetailPath}";
                if(Directory.Exists(directoryPath) == false)
                    Directory.CreateDirectory(directoryPath);

                _mouseSetting = new Mouse(1f, 1f);

                var jsonFile = File.CreateText(_path);
                jsonFile.Write(JsonUtility.ToJson(_mouseSetting));
                jsonFile.Close();
            }

        }

        /// <summary>
        /// ù ������ ����, ȣ��Ǵ� �ʱ�ȭ �޼ҵ�
        /// </summary>
        private void Start()
        {
            // #DEBUG
            if(DebugMode)
            {
                _stateMachine.Setup(Instance, _states[(int)GameManagerStates.Debug]);
                return;
            }

            /// StateMachine�� owner�� �̱��� ��ü��, �ʱ� ���·� Main���� �����Ѵ�.
            _stateMachine.Setup(Instance, _states[(int)GameManagerStates.Main]);
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();

            var json = JsonUtility.ToJson(_mouseSetting);
            File.WriteAllText(_path, json);
        }

        public void OnVariableInitialized()
        {
            _timer = GetComponent<Timer>();
            _scoreSystem = new ScoreSystem();
            _player = GameObject.Find("Player").GetComponent<PlayerController>();
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
            _states = new StateBase<GameManager>[3];
            _states[(int)GameManagerStates.Main] = stateObject.AddComponent<Main>();
            _states[(int)GameManagerStates.Game] = stateObject.AddComponent<Game>();
            
            // #DEBUG
            if(DebugMode)
                _states[(int)GameManagerStates.Debug] = stateObject.AddComponent<StateDebug>();

            /// 3.2. ���� �ӽ� �ʱ�ȭ
            _stateMachine = new StateMachine<GameManager>();
        }

        /// <summary>
        /// ���¸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="newState">������ ���ο� ����</param>
        private void ChangeState(GameManagerStates newState)
        {
            _stateMachine.ChangeState(_states[(int)newState]);
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
            _timer.Stop();
            _player.GetComponent<PlayerController>().enabled = false;
            
            _scoreSystem.CalculateScore(GoalNumberOfKill, EnemyManager.Instance.KillCountInfo, _timer.Time);
            GameOverEvent.Invoke(_scoreSystem.TotalScore, _timer.ToString(), _scoreSystem.KillScore);
        }

        /// <summary>
        /// �������� �����ϴ� �޼ҵ�
        /// </summary>
        public void ReturnMain()
        {
            /// ���� ���
            var json = JsonUtility.ToJson(_mouseSetting);
            File.WriteAllText(_path, json);

            SoundManager.Instance.BackupSetting();

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
            _pauseStack++;
            _isPause = _pauseStack > 0;

            if (_isPause)
                Time.timeScale = 0f;
        }

        /// <summary>
        /// �̾��ϱ� �޼ҵ�
        /// </summary>
        public void Resume()
        {
            /// �Ͻ� ���� ������ 1 ���ҽ�Ų��(��, ������ �������� �ʴ´�).
            _pauseStack = _pauseStack - 1 <= 0 ? 0 : _pauseStack - 1;
            _isPause = _pauseStack > 0;

            if (_isPause == false)
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
            return _timer.ToString();
        }
    }
}