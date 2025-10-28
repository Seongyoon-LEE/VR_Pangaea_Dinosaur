using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMoveManager : MonoBehaviour
{
    [Header("Fade 설정")]
    public CanvasGroup fadePanel;   // 페이드용 패널
    public float fadeDuration = 1f; // 페이드 속도 (초 단위)

    private void Start()
    {
        // 시작 시 자동으로 페이드 인
        StartCoroutine(FadeIn());
    }

    public void LoadMainScene()
    {
        StartCoroutine(FadeAndLoadScene("MainScene"));
    }

    public void ExitGame()
    {
        StartCoroutine(FadeAndQuit());
    }

    private IEnumerator FadeIn()
    {
        fadePanel.alpha = 1;
        while (fadePanel.alpha > 0)
        {
            fadePanel.alpha -= Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // 페이드 아웃
        while (fadePanel.alpha < 1)
        {
            fadePanel.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeAndQuit()
    {
        // 페이드 아웃
        while (fadePanel.alpha < 1)
        {
            fadePanel.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
        Debug.Log("게임 종료");
        Application.Quit();
    }


}
