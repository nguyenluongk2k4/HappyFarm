using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("GameManager initialized");
    }

    void Update()
    {
        // Game logic here
    }

    /// <summary>
    /// Load scene theo index
    /// </summary>
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"Scene index {sceneIndex} is out of range!");
            return;
        }

        Debug.Log($"Loading scene at index: {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);
    }
}