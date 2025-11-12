using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class AnimalData
{
    public string prefabName;
    public Vector3 position;

    // ✅ Dữ liệu trạng thái riêng của từng loại (hiện mới dùng cho Chicken)
    public Chicken.ChickenSaveData chickenData;

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
            // Lưu thông tin cơ bản
            var data = new AnimalData(marker.prefabName, marker.transform.position);

            // Nếu là gà → lưu thêm trạng thái
            var chicken = marker.GetComponent<Chicken>();
            if (chicken != null)
            {
                data.chickenData = chicken.SaveState();
            }

            list.Add(data);
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

                // Gắn marker lại
                var marker = obj.AddComponent<AnimalMarker>();
                marker.prefabName = prefab.name;

                // Nếu là gà → khôi phục trạng thái
                var chicken = obj.GetComponent<Chicken>();
                if (chicken != null && a.chickenData != null)
                {
                    chicken.LoadState(a.chickenData);
                }
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
