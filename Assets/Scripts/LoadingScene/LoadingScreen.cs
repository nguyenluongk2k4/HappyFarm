using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;

    void Start()
    {
        // Kiểm tra: đang chơi chưa?
        if (PlayerPrefs.GetInt("IsPlaying", 0) == 1)
        {
            // Nếu đang chơi → vào Farm luôn (không đọc NextScene)
            StartCoroutine(LoadSceneAsync("Farm"));
        }
        else
        {
            // Nếu chưa chơi → đọc NextScene (từ MainMenu)
            string nextScene = PlayerPrefs.GetString("NextScene", "MainMenu");
            StartCoroutine(LoadSceneAsync(nextScene));
        }
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;

            yield return null;
        }
    }
}