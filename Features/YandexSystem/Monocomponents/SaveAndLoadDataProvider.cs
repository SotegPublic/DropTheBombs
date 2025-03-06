using System.Runtime.InteropServices;
using UnityEngine;

public class SaveAndLoadDataProvider : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SaveExtern(string playerDate);

    [DllImport("__Internal")]
    private static extern void SaveToLocalStorage(string key, string playerDate);

    [DllImport("__Internal")]
    private static extern int HasKey(string key);

    [DllImport("__Internal")]
    public static extern string LoadFromLocalStorage(string key);

    [DllImport("__Internal")]
    private static extern void LoadExtern();

    private const string LOCAL_KEY = "localSave";

    public void SaveGameExtern(string saveData)
    {
#if !UNITY_EDITOR
        SaveExtern(saveData);
#endif
    }

    public void SaveGameLocal(string saveData)
    {
#if !UNITY_EDITOR
        SaveToLocalStorage(LOCAL_KEY, saveData);
#endif
    }

    public bool TryLoadLocal(out string jsonStr)
    {
#if !UNITY_EDITOR
        if (HasKey(LOCAL_KEY) == 1)
        {
            jsonStr = LoadFromLocalStorage(LOCAL_KEY);
            return true;
        }
        else
        {
            jsonStr = null;
            return false;
        }
#else
        jsonStr = null;
        return false;
#endif
    }

    public void LoadFromCloud()
    {
#if !UNITY_EDITOR
        LoadExtern();
#endif
    }
}
