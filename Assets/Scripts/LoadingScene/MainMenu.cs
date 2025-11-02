using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public CanvasGroup menuMainGroup;   // Menu chính (Play, Exit)
    public CanvasGroup menuPlayGroup;   // Menu chọn map (Beach, Farm...)

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    void Start()
    {
        // Bắt đầu: Chỉ hiện menu chính, ẩn menu play
        ShowPanel(menuMainGroup, true);
        ShowPanel(menuPlayGroup, false);
    }

    // ====================== BUTTON CALLS ======================

    public void PlayGame()
    {
        // Từ Main → Play Menu
        StartCoroutine(SwitchMenu(menuMainGroup, menuPlayGroup));
    }

    public void OnExitClicked()
    {
        // Từ Play Menu → Main Menu
        StartCoroutine(SwitchMenu(menuPlayGroup, menuMainGroup));
    }

    public void BeachMap()
    {
        LoadMap("Beach");
    }

    public void FarmMap()
    {
        LoadMap("Farm");
    }

    public void MarketMap()
    {
        LoadMap("Market");
    }

    public void QuitGame()
    {
        Debug.Log("Thoát game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ====================== HÀM HỖ TRỢ ======================

    void LoadMap(string mapName)
    {
        PlayerPrefs.SetString("NextScene", mapName);
        SceneManager.LoadScene("LoadingScene");
    }

    IEnumerator SwitchMenu(CanvasGroup fromPanel, CanvasGroup toPanel)
    {
        // 1. Fade out panel cũ
        yield return FadePanel(fromPanel, 0f);

        // 2. Fade in panel mới
        yield return FadePanel(toPanel, 1f);
    }

    IEnumerator FadePanel(CanvasGroup panel, float targetAlpha)
    {
        // Bật/tắt interactable
        panel.interactable = (targetAlpha > 0.5f);
        panel.blocksRaycasts = (targetAlpha > 0.5f);

        float startAlpha = panel.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Dùng unscaled để mượt khi pause
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
        // Không cần SetActive → vẫn giữ object
    }
}