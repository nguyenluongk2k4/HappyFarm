using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    // Singleton pattern để dễ truy cập
    public static UIManager Instance { get; private set; }

    [Header("List of UI Panels")]
    [SerializeField] private List<GameObject> panels = new List<GameObject>();

    private void Awake()
    {
        // Đảm bảo chỉ có 1 instance duy nhất
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Mở panel theo tên GameObject.
    /// </summary>
    public void OpenPanel(string panelName)
    {
        foreach (var panel in panels)
        {
            if (panel != null)
                panel.SetActive(panel.name == panelName);
        }
    }

    /// <summary>
    /// Mở panel bằng reference.
    /// </summary>
    public void OpenPanel(GameObject panel)
    {
        CloseAllPanels();
        if (panel != null)
            panel.SetActive(true);
    }

    /// <summary>
    /// Đóng 1 panel cụ thể.
    /// </summary>
    public void ClosePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
    }

    /// <summary>
    /// Đóng toàn bộ panel.
    /// </summary>
    public void CloseAllPanels()
    {
        foreach (var panel in panels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }

    /// <summary>
    /// Toggle panel (bật/tắt).
    /// </summary>
    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(!panel.activeSelf);
    }
}
