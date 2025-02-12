using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public static LevelInfo LoadLevel()
    {
        int currentLevel = PlayerPrefs.GetInt("Level", 1);
        TextAsset levelTextAsset = Resources.Load<TextAsset>("Levels/level_" + currentLevel.ToString("00"));
        if (levelTextAsset == null)
        {
            Debug.LogError("Level data not found for level " + currentLevel);
        }
        
        LevelInfo levelInfo = JsonUtility.FromJson<LevelInfo>(levelTextAsset.text);
        Debug.Log("Loaded level " + currentLevel);
        return levelInfo;
    }
    
}
