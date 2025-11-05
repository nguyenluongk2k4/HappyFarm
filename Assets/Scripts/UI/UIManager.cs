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
    /// Nếu panel đang active thì đóng, nếu inactive thì mở.
    /// </summary>
    public void OpenPanel(string panelName)
    {
        foreach (var panel in panels)
        {
            if (panel != null && panel.name == panelName)
            {
                // Toggle trạng thái của panel này
                bool newState = !panel.activeSelf;
                panel.SetActive(newState);
                
                // Đóng các panel khác
                foreach (var otherPanel in panels)
                {
                    if (otherPanel != null && otherPanel != panel)
                        otherPanel.SetActive(false);
                }
                return;
            }
        }
    }

    /// <summary>
    /// Mở panel bằng reference.
    /// Nếu panel đang active thì đóng, nếu inactive thì mở.
    /// </summary>
    public void OpenPanel(GameObject panel)
    {
        if (panel != null)
        {
            // Toggle trạng thái của panel này
            bool newState = !panel.activeSelf;
            panel.SetActive(newState);
            
            // Đóng các panel khác khi mở panel mới
            if (newState)
            {
                foreach (var otherPanel in panels)
                {
                    if (otherPanel != null && otherPanel != panel)
                        otherPanel.SetActive(false);
                }
            }
        }
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
