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

        private bool isPause;            // [플래그] 게임이 일시정지 됬는지 확인
        private int pauseStack = 0;     // 일시 정지 요청이 왔을 때 증가시키는 스택
        private Timer timer;
        [SerializeField]
        private int goalNumberOfKill = 1000;
        private ScoreSystem scoreSystem;

        private PlayerController player;

        [Header("Setting Asset")]
        public Mouse MouseSetting;

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

        /****************************************
         * 프로퍼티
         ****************************************/
        public GameObject       WindowMain => windowMain;
        public GameObject       WindowPlayerHUD => windowPlayerHUD;
        public bool             IsPause => isPause;
        public Timer            Timer => timer;
        public int              GoalNumberOfKill => goalNumberOfKill;
        public PlayerController Player => player;
        public ScoreSystem      ScoreSystem => scoreSystem;

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
        }

        /// <summary>
        /// 첫 프레임 직전, 호출되는 초기화 메소드
        /// </summary>
        private void Start()
        {
            // #DEBUG
            if(DebugMode)
            {
                stateMachine.Setup(Instance, states[(int)GameManagerStates.Debug]);
                return;
            }

            /// StateMachine의 owner를 싱글톤 객체로, 초기 상태로 Main으로 설정한다.
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
            /// 3. 상태 관련 초기화
            /// 3.1. 상태 초기화
            /// 3.1.1. 상태 컴포넌트를 붙일 오브젝트 생성 및 초기화
            GameObject stateObject = new GameObject("States");
            stateObject.transform.parent = this.transform;
            stateObject.transform.localScale = Vector3.zero;

            /// 3.1.2. 상태 컴포넌트 부착 및 초기화
            states = new StateBase<GameManager>[3];
            states[(int)GameManagerStates.Main] = stateObject.AddComponent<Main>();
            states[(int)GameManagerStates.Game] = stateObject.AddComponent<Game>();
            
            // #DEBUG
            if(DebugMode)
                states[(int)GameManagerStates.Debug] = stateObject.AddComponent<StateDebug>();

            /// 3.2. 상태 머신 초기화
            stateMachine = new StateMachine<GameManager>();
        }

        /// <summary>
        /// 상태를 변경하는 메소드
        /// </summary>
        /// <param name="newState">변경할 새로운 상태</param>
        private void ChangeState(GameManagerStates newState)
        {
            stateMachine.ChangeState(states[(int)newState]);
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
            timer.Stop();
            player.GetComponent<PlayerController>().enabled = false;
            
            scoreSystem.CalculateScore(goalNumberOfKill, EnemyManager.Instance.KillCountInfo, timer.Time);
            gameOverEvent.Invoke(scoreSystem.TotalScore, timer.ToString(), scoreSystem.KillScore);
        }

        /// <summary>
        /// 메인으로 복귀하는 메소드
        /// </summary>
        public void ReturnMain()
        {
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
            pauseStack++;
            isPause = pauseStack > 0;

            if (isPause)
                Time.timeScale = 0f;
        }

        /// <summary>
        /// 이어하기 메소드
        /// </summary>
        public void Resume()
        {
            /// 일시 정지 스택을 1 감소시킨다(단, 음수로 내려가진 않는다).
            pauseStack = pauseStack - 1 <= 0 ? 0 : pauseStack - 1;
            isPause = pauseStack > 0;

            if (isPause == false)
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
            return timer.ToString();
        }
    }
}