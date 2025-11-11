using UnityEngine;

[DisallowMultipleComponent]
public class AnimalMarker : MonoBehaviour
{
    [Tooltip("Tên prefab gốc dùng để spawn lại khi load game")]
    public string prefabName;

    private void Reset()
    {
        prefabName = GetDefaultPrefabName();
    }

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(prefabName))
            prefabName = GetDefaultPrefabName();
    }

    private string GetDefaultPrefabName()
    {
        string raw = gameObject.name;
        int cloneIndex = raw.IndexOf("(Clone)");
        if (cloneIndex >= 0)
            raw = raw.Substring(0, cloneIndex);
        return raw.Trim();
    }
}
