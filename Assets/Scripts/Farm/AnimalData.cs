using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class AnimalData
{
    public string prefabName;
    public Vector3 position;

    // Lưu tạm trong RAM khi load (chờ scene đổi xong mới spawn)
    public static List<AnimalData> memory = new List<AnimalData>();

    public AnimalData(string name, Vector3 pos)
    {
        prefabName = name;
        position = pos;
    }

    static string SavePath => Application.persistentDataPath + "/animals.json";

    // ================= SAVE =================
    public static void Save()
    {
        List<AnimalData> list = new();

        var markers = Object.FindObjectsByType<AnimalMarker>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var marker in markers)
        {
            list.Add(new AnimalData(marker.prefabName, marker.transform.position));
        }

        File.WriteAllText(SavePath, JsonUtility.ToJson(new Wrapper(list), true));
        Debug.Log($"🐔 Đã lưu {list.Count} animal!");
    }

    // ================= LOAD (chỉ đọc vào memory, không spawn ngay) =================
    public static void LoadToMemory()
    {
        memory.Clear();

        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("⚠ Không có file animal save");
            return;
        }

        var data = JsonUtility.FromJson<Wrapper>(File.ReadAllText(SavePath));
        memory = data.animals;
        Debug.Log($"📦 Đã load {memory.Count} animal vào bộ nhớ (chưa spawn)");
    }

    // ================= SPAWN (chỉ spawn khi scene game load xong) =================
    public static void SpawnFromMemory()
    {
        // ❗ Nếu là Boot scene → không spawn
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Debug.Log("⛔ Đây là Boot Scene → bỏ qua spawn gà");
            return;
        }

        // Xóa gà cũ trong scene
        foreach (var old in Object.FindObjectsByType<AnimalMarker>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            Object.Destroy(old.gameObject);

        // Spawn lại từ memory
        foreach (var a in memory)
        {
            if (GameManager.Instance.animalPrefabDict.TryGetValue(a.prefabName, out GameObject prefab))
            {
                var obj = Object.Instantiate(prefab, a.position, Quaternion.identity);
                obj.name = prefab.name;
                obj.AddComponent<AnimalMarker>().prefabName = prefab.name;
            }
        }

        Debug.Log($"🐣 Đã spawn {memory.Count} animal vào scene!");
    }

    [System.Serializable]
    class Wrapper
    {
        public List<AnimalData> animals;
        public Wrapper(List<AnimalData> a) => animals = a;
    }
}
