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
    /// TODO: Main -> Game 전환 변경 방식 변경에 따른 삭제 예정
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
        [Tooltip("목표 처치 수(일반 등급)")]
        public int GoalNumberOfKill = 1000;

        public bool             IsPause => _isPause;

        public Timer            Timer => _timer;

        public PlayerController Player => _player;

        [Header("Mouse Setting")]
        [Tooltip("마우스 설정 파일 경로")]
        public string DetailPath;

        [Tooltip("Json 파일 이름")]
        public string JsonFileName;

        public Mouse MouseSetting { get => _mouseSetting; }
        
        // Game
        private bool                _isPause;            // [플래그] 게임이 일시정지 됬는지 확인
        private int                 _pauseStack = 0;     // 일시 정지 요청이 왔을 때 증가시키는 스택
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
        [Tooltip("디버그 모드")]
        public bool DebugMode;

        [Tooltip("게임 시작 모드")]
        public GameManagerStates StartState;

        [Space(5)]
        [Tooltip("플레이어 비활성화")]
        public bool DeactivePlayer;

        [Tooltip("로그 출력 비활성화")]
        public bool DeactiveLog;

        [Space(5)]
        [Tooltip("적 소환 정지")]
        public bool StopSpawnEnemy;

        [Tooltip("Enemy 개발 중")]
        public bool DevelopingEnemy;

        [Space(5)]
        [Tooltip("디버그용 윈도우")]
        public GameObject WindowDebug;

        /// <summary>
        /// GameManager가 인스턴스화 된 직후, 호출되는 초기화 메소드
        /// </summary>
        protected override void Awake()
        {
            /// 1. 싱글톤 작업
            base.Awake();

            /// 2. 컴포넌트 및 변수 초기화
            OnVariableInitialized();

            /// 3. 상태 관련 초기화
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
        /// 첫 프레임 직전, 호출되는 초기화 메소드
        /// </summary>
        private void Start()
        {
            // #DEBUG
            if(DebugMode)
            {
                _stateMachine.Setup(Instance, _states[(int)GameManagerStates.Debug]);
                return;
            }

            /// StateMachine의 owner를 싱글톤 객체로, 초기 상태로 Main으로 설정한다.
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
            /// 3. 상태 관련 초기화
            /// 3.1. 상태 초기화
            /// 3.1.1. 상태 컴포넌트를 붙일 오브젝트 생성 및 초기화
            GameObject stateObject = new GameObject("States");
            stateObject.transform.parent = this.transform;
            stateObject.transform.localScale = Vector3.zero;

            /// 3.1.2. 상태 컴포넌트 부착 및 초기화
            _states = new StateBase<GameManager>[3];
            _states[(int)GameManagerStates.Main] = stateObject.AddComponent<Main>();
            _states[(int)GameManagerStates.Game] = stateObject.AddComponent<Game>();
            
            // #DEBUG
            if(DebugMode)
                _states[(int)GameManagerStates.Debug] = stateObject.AddComponent<StateDebug>();

            /// 3.2. 상태 머신 초기화
            _stateMachine = new StateMachine<GameManager>();
        }

        /// <summary>
        /// 상태를 변경하는 메소드
        /// </summary>
        /// <param name="newState">변경할 새로운 상태</param>
        private void ChangeState(GameManagerStates newState)
        {
            _stateMachine.ChangeState(_states[(int)newState]);
        }

        /// <summary>
        /// 게임 시작 메소드
        /// </summary>
        public void GameStart()
        {
            ChangeState(GameManagerStates.Game);
        }

        /// <summary>
        /// 플레이어 사망 및 목표 달성시 호출되는 메소드
        /// </summary>
        public void GameOver()
        {
            /// 게임 종료시 필요한 조치 실행
            ///     * 커서 활성화
            ///     * 일시 정지(알림창 뒤로 캐릭터들이 움직이지 않게)
            ///     * 타이머 정지
            ///     * PlayerController를 비활성화해 플레이어 게임오브젝트를 조작할 수 없게 한다.
            SetCursorActive(true);
            Pause();
            _timer.Stop();
            _player.GetComponent<PlayerController>().enabled = false;
            
            _scoreSystem.CalculateScore(GoalNumberOfKill, EnemyManager.Instance.KillCountInfo, _timer.Time);
            GameOverEvent.Invoke(_scoreSystem.TotalScore, _timer.ToString(), _scoreSystem.KillScore);
        }

        /// <summary>
        /// 메인으로 복귀하는 메소드
        /// </summary>
        public void ReturnMain()
        {
            /// 설정 백업
            var json = JsonUtility.ToJson(_mouseSetting);
            File.WriteAllText(_path, json);

            SoundManager.Instance.BackupSetting();

            /// 씬을 다시 로딩해 메인으로 복귀한다.
            SceneManager.LoadScene(SceneName.Game.ToString());
        }

        /// <summary>
        /// 게임 종료 메소드
        /// </summary>
        public void GameExit()
        {
            Application.Quit();
        }

        /// <summary>
        /// 일시 정지 메소드
        /// </summary>
        public void Pause()
        {
            /// 일시 정지 스택을 1 증가시킨다.
            _pauseStack++;
            _isPause = _pauseStack > 0;

            if (_isPause)
                Time.timeScale = 0f;
        }

        /// <summary>
        /// 이어하기 메소드
        /// </summary>
        public void Resume()
        {
            /// 일시 정지 스택을 1 감소시킨다(단, 음수로 내려가진 않는다).
            _pauseStack = _pauseStack - 1 <= 0 ? 0 : _pauseStack - 1;
            _isPause = _pauseStack > 0;

            if (_isPause == false)
                Time.timeScale = 1f;
        }

        /// <summary>
        /// 마우스 커서 활성화 설정 메소드
        /// </summary>
        /// <param name="active">활성화 여부</param>
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
        /// 플레이 시간을 문자열로 반환하는 메소드
        /// </summary>
        /// <returns></returns>
        public string TimeToString()
        {
            return _timer.ToString();
        }
    }
}