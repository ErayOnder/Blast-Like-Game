using UnityEngine;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public LevelData LoadLevelData()
    {
        int currentLevel = PlayerPrefs.GetInt("Level", 1);
        TextAsset levelTextAsset = Resources.Load<TextAsset>("Levels/level_" + currentLevel.ToString("00"));
        if (levelTextAsset == null)
        {
            Debug.LogError("Level data not found for level " + currentLevel);
        }
        
        LevelData levelData = JsonUtility.FromJson<LevelData>(levelTextAsset.text);
        Debug.Log("Loaded level " + currentLevel);
        return levelData;
    }
    
}
