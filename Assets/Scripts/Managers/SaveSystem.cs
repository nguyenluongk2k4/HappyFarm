using UnityEngine;
using System.IO;
using System;

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    // Lưu game
    public static void Save(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("✅ Game saved to: " + SavePath);
    }

    // Load game
    public static GameData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found.");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<GameData>(json);
    }

    public static bool HasSaveFile()
    {
        return File.Exists(SavePath);
    }
}
