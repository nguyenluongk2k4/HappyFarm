using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public CanvasGroup menuMainGroup;   // Chỉ còn Play, Exit

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    void Start()
    {
        ShowPanel(menuMainGroup, true);
    }

    // ====================== BUTTON CALLS ======================

    public void PlayGame()
    {
        // Bắt đầu chơi mới → vào Farm luôn
        
        PlayerPrefs.SetString("NextScene", "Farm");
        PlayerPrefs.SetInt("IsPlaying", 1); // Đánh dấu đang chơi
        StartCoroutine(LoadWithFade());
    }

    public void QuitGame()
    {
        Debug.Log("Thoát game...");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void LoadGame()
    {

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadGameAndStart();
            StartCoroutine(LoadWithFade());
        }
        else
        {
            Debug.LogError("GameManager instance NULL!");
        }
    }

IEnumerator LoadSaved()
    {
        yield return new WaitForSeconds(0.3f);
        GameManager.Instance.LoadGameAndStart();
    }


    // ====================== HÀM HỖ TRỢ ======================

    IEnumerator LoadWithFade()
    {
        yield return FadePanel(menuMainGroup, 0f);
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator FadePanel(CanvasGroup panel, float targetAlpha)
    {
        panel.interactable = (targetAlpha > 0.5f);
        panel.blocksRaycasts = (targetAlpha > 0.5f);

        float startAlpha = panel.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            panel.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        panel.alpha = targetAlpha;
    }

    void ShowPanel(CanvasGroup panel, bool show)
    {
        panel.alpha = show ? 1f : 0f;
        panel.interactable = show;
        panel.blocksRaycasts = show;
    }
}