using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("List of UI Panels")]
    [SerializeField] private List<GameObject> panels = new List<GameObject>();

    [Header("Hotkey Panels")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject hotbarPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        HandleKeyInput();
    }

    private void HandleKeyInput()
    {
        // Nhấn I mở/tắt Inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Toggling Inventory Panel");
            if (inventoryPanel != null)
                OpenPanel(inventoryPanel);
            else
                OpenPanel("InventoryPanel");
        }

        // Nhấn H mở/tắt Hotbar
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (hotbarPanel != null)
                OpenPanel(hotbarPanel);
            else
                OpenPanel("HotbarPanel");
        }
    }

    public void OpenPanel(string panelName)
    {
        foreach (var panel in panels)
        {
            if (panel != null && panel.name == panelName)
            {
                bool newState = !panel.activeSelf;
                panel.SetActive(newState);

                foreach (var otherPanel in panels)
                {
                    if (otherPanel != null && otherPanel != panel)
                        otherPanel.SetActive(false);
                }
                return;
            }
        }
    }

    public void OpenPanel(GameObject panel)
    {
        if (panel != null)
        {
            bool newState = !panel.activeSelf;
            panel.SetActive(newState);

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

    public void ClosePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void CloseAllPanels()
    {
        foreach (var panel in panels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }

    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
            panel.SetActive(!panel.activeSelf);
    }
}
