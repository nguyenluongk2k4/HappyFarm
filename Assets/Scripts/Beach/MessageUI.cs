using UnityEngine;
using TMPro;
using System.Collections;

public class MessageUI : MonoBehaviour
{
    public static MessageUI Instance;

    [Header("Refs")]
    [SerializeField] private CanvasGroup canvasGroup; // TopMessage 的 CanvasGroup
    [SerializeField] private RectTransform backgroundRect; // TopMessage hoặc Background rect
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Tuning")]
    [SerializeField] private float showDuration = 2.0f;
    [SerializeField] private float fadeTime = 0.25f;
    [SerializeField] private float popScale = 1.05f;
    [SerializeField] private float popTime = 0.18f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // ensure initial hidden
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    public void ShowMessage(string text)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowRoutine(text));
    }

    private IEnumerator ShowRoutine(string text)
    {
        // set text
        messageText.text = text;

        // pop scale effect
        if (backgroundRect != null)
        {
            backgroundRect.localScale = Vector3.one * 0.92f;
            float t = 0f;
            while (t < popTime)
            {
                t += Time.unscaledDeltaTime;
                float p = Mathf.SmoothStep(0f, 1f, t / popTime);
                backgroundRect.localScale = Vector3.Lerp(Vector3.one * 0.92f, Vector3.one * popScale, p);
                yield return null;
            }
            backgroundRect.localScale = Vector3.one;
        }

        // fade in
        if (canvasGroup != null)
        {
            float t = 0f;
            while (t < fadeTime)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        // stay
        float timer = 0f;
        while (timer < showDuration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // fade out
        if (canvasGroup != null)
        {
            float t = 0f;
            while (t < fadeTime)
            {
                t += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }

        // reset scale
        if (backgroundRect != null) backgroundRect.localScale = Vector3.one;

        currentRoutine = null;
    }
}
