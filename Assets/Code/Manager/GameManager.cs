using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using WhalePark18.Character.Player;

namespace WhalePark18.Manager
{
    public struct SceneName
    {
        public static string Start => "StartScene";
        public static string Game => "GameScene";
    }

    [System.Serializable]
    public class GameOverEvent : UnityEngine.Events.UnityEvent<int, string, int> { }

    public class GameManager : MonoSingleton<GameManager>
    {
        [HideInInspector]
        public GameOverEvent gameOverEvent;

        /// <summary>
        /// Pause
        /// </summary>
        private bool isPause;       // [플래그] 게임이 일시정지 됬는지 확인
        private int pauseStack = 0; // 일시 정지 요청이 왔을 때 증가시키는 스택

        /// <summary>
        /// Game Play
        /// </summary>
        private GameObject player;

        private Timer timer;
        [SerializeField]
        private int killGoals = 1000;

        /// <summary>
        /// 프로퍼티
        /// </summary>
        public bool IsPause => isPause;
        public int KillGoals => killGoals;

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }

            /// SceneManager를 통해 씬 로드 시 작동할 함수 추가
            SceneManager.sceneLoaded += OnSceneLoaded;

            /// 컴포넌트 초기화
            timer = GetComponent<Timer>();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(scene.name == SceneName.Game)
                GameStart();
        }

        private void GameStart()
        {
            WhalePark18.Debug.Log(DebugCategory.Debug, MethodBase.GetCurrentMethod().Name);

            player = GameObject.Find("Player");

            EnemyManager.Instance.Target = player.transform;
            EnemyManager.Instance.Run();

            timer.Run();
        }

        /// <summary>
        /// 게임 종료 메소드
        /// </summary>
        public void GameExit()
        {
            Application.Quit();
        }

        /// <summary>
        /// 게임 초기화 메소드
        /// </summary>
        public void GameReset()
        {
            player.GetComponent<PlayerController>().enabled = true;
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

            /// 점수 계산 후, WindowScore에 반영한다.
            /// 최종 점수 = 시간 점수 + 적 처치 점수
            /// 시간 점수 = (3600 - 생존 시간) * 적 저치 수 / 목표 적 처치 수
            ///     * 3600: 1시간을 초로 변환한 숫자
            ///         * 3600에 생존 시간을 빼면서 더 적은 시간에 목표를 달성했을 시 높은 점수를 얻는다.
            ///         * 단, 점수 계산시 생존 시간은 3600을 넘을 수 없다.
            ///     * 적 처치 수 / 목표 적 처치 수: 적 처치 백분율
            ///         * 적 처치 백분율을 곱해 적을 처치하지 않고 빨리 사망해 높은 시간 점수를 얻는 것을 방지한다.
            int killScore = EnemyManager.Instance.KillCount;
            int timeScore = (3600 - (int)timer.Time) > 0 ? 3600 - (int)timer.Time : 0;
            timeScore *= (killScore / killGoals);
            int totalScore = timeScore + killScore;
            gameOverEvent.Invoke(totalScore, timer.ToString(), killScore);
        }

        /// <summary>
        /// 일시 정지 메소드
        /// </summary>
        public void Pause()
        {
            /// 일시 정지 스택을 1 증가시킨다.
            pauseStack++;
            isPause = pauseStack > 0;
            //print("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color> pauseStack: " + pauseStack);

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
            //print("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color> pauseStack: " + pauseStack);

            if (isPause == false)
                Time.timeScale = 1f;
        }

        /// <summary>
        /// 마우스 커서 활성화 설정 메소드
        /// </summary>
        /// <param name="active">활성화 여부</param>
        public void SetCursorActive(bool active)
        {
            //print("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color> active: " + active);

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

        public string TimeToString()
        {
            return timer.ToString();
        }
    }
}