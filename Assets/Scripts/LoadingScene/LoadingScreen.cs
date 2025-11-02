using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar; // Thanh loading (tùy chọn)

    void Start()
    {
        string nextScene = PlayerPrefs.GetString("NextScene", "PlayerPref");
        StartCoroutine(LoadSceneAsync(nextScene));
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
