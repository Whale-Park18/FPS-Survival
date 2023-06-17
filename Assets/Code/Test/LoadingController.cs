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

        /// �ڵ����� ȣ���� ������ ������ ������ ����.
        /// True: �ڵ����� ����
        /// False: 90% �ε� ��, ���
        ///     1. �������� �� �ε��� ���� ���, ��¥ �ε��� ����
        ///     2. ������ ū ������ ��� ���� ��� ���� ������ ��, �����ϴ� ���� �ƴ϶�
        ///        "�ּ� ����"�̶�� ������ ������ �����ϰ� �� �ּ� ����� ���� ���ҽ���
        ///        ����, �� ���ҽ��� ���� �ε��Ǹ� ��� ������ ���ҽ��� �ε��Ǳ� ����
        ///        ���� �ε��� ������ ���� ���ҽ��� �Ҵ���� �ʾ� ������Ʈ�� ���� ����.
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
