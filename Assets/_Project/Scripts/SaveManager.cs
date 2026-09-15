using UnityEngine;

public static class SaveManager
{
    public static void SaveSFX(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    public static float LoadSFX(string key, float defaultValue)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public static void SaveLocale(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static int LoadLocale(string key, int defaultValue)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SaveLevel(string key, LevelDTO levelDTO)
    {
        string saveValue = JsonUtility.ToJson(levelDTO);
        PlayerPrefs.SetString(key, saveValue);
    }

    public static LevelDTO LoadLevel(string key)
    {
        string defaultValue = null;
        string loadValue = PlayerPrefs.GetString(key, defaultValue);

        if (string.IsNullOrEmpty(loadValue))
            return null;
        
        return JsonUtility.FromJson<LevelDTO>(loadValue);
    }
}
