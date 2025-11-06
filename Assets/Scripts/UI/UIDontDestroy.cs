using UnityEngine;

public class UIDontDestroy : MonoBehaviour
{
    private static UIDontDestroy instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Ngăn duplicate UI
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
