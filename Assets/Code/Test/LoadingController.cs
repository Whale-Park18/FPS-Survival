using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    public string nextScene;

    public Image progressBar;

    public void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);

        /// 자동으로 호출한 씬으로 변경할 것인지 설정.
        /// True: 자동으로 변경
        /// False: 90% 로딩 후, 대기
        ///     1. 생각보다 씬 로딩이 빠른 경우, 가짜 로딩을 넣음
        ///     2. 볼륨이 큰 게임의 경우 씬에 모든 것을 설정한 후, 빌드하는 것이 아니라
        ///        "애셋 번들"이라는 것으로 나눠서 빌드하고 이 애셋 번들로 부터 리소스를
        ///        읽음, 이 리소스가 먼저 로딩되면 상관 없지만 리소스가 로딩되기 전에
        ///        씬의 로딩이 끝나면 씬에 리소스가 할당되지 않아 오브젝트가 깨져 보임.
        op.allowSceneActivation = false;

        float timer = 0f;
        while (op.isDone == false)
        {
            yield return null;

            if (op.progress < 0.9f)
                progressBar.fillAmount = op.progress;
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);

                if (progressBar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
