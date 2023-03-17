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
        private bool isPause;       // [�÷���] ������ �Ͻ����� ����� Ȯ��
        private int pauseStack = 0; // �Ͻ� ���� ��û�� ���� �� ������Ű�� ����

        /// <summary>
        /// Game Play
        /// </summary>
        private GameObject player;

        private Timer timer;
        [SerializeField]
        private int killGoals = 1000;

        /// <summary>
        /// ������Ƽ
        /// </summary>
        public bool IsPause => isPause;
        public int KillGoals => killGoals;

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }

            /// SceneManager�� ���� �� �ε� �� �۵��� �Լ� �߰�
            SceneManager.sceneLoaded += OnSceneLoaded;

            /// ������Ʈ �ʱ�ȭ
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
        /// ���� ���� �޼ҵ�
        /// </summary>
        public void GameExit()
        {
            Application.Quit();
        }

        /// <summary>
        /// ���� �ʱ�ȭ �޼ҵ�
        /// </summary>
        public void GameReset()
        {
            player.GetComponent<PlayerController>().enabled = true;
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

            /// ���� ��� ��, WindowScore�� �ݿ��Ѵ�.
            /// ���� ���� = �ð� ���� + �� óġ ����
            /// �ð� ���� = (3600 - ���� �ð�) * �� ��ġ �� / ��ǥ �� óġ ��
            ///     * 3600: 1�ð��� �ʷ� ��ȯ�� ����
            ///         * 3600�� ���� �ð��� ���鼭 �� ���� �ð��� ��ǥ�� �޼����� �� ���� ������ ��´�.
            ///         * ��, ���� ���� ���� �ð��� 3600�� ���� �� ����.
            ///     * �� óġ �� / ��ǥ �� óġ ��: �� óġ �����
            ///         * �� óġ ������� ���� ���� óġ���� �ʰ� ���� ����� ���� �ð� ������ ��� ���� �����Ѵ�.
            int killScore = EnemyManager.Instance.KillCount;
            int timeScore = (3600 - (int)timer.Time) > 0 ? 3600 - (int)timer.Time : 0;
            timeScore *= (killScore / killGoals);
            int totalScore = timeScore + killScore;
            gameOverEvent.Invoke(totalScore, timer.ToString(), killScore);
        }

        /// <summary>
        /// �Ͻ� ���� �޼ҵ�
        /// </summary>
        public void Pause()
        {
            /// �Ͻ� ���� ������ 1 ������Ų��.
            pauseStack++;
            isPause = pauseStack > 0;
            //print("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color> pauseStack: " + pauseStack);

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
            //print("<color=green>" + MethodBase.GetCurrentMethod().Name + "</color> pauseStack: " + pauseStack);

            if (isPause == false)
                Time.timeScale = 1f;
        }

        /// <summary>
        /// ���콺 Ŀ�� Ȱ��ȭ ���� �޼ҵ�
        /// </summary>
        /// <param name="active">Ȱ��ȭ ����</param>
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