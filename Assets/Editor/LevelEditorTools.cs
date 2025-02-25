using UnityEngine;
using UnityEditor;

public class LevelEditorTools : EditorWindow
{
    private int levelNumber = 1;
    
    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorTools>("Set Level");
    }

    void OnGUI()
    {
        GUILayout.Label("Level Editor", EditorStyles.boldLabel);
        levelNumber = EditorGUILayout.IntField("Level Number", levelNumber);

        if (GUILayout.Button("Set Level"))
        {
            PlayerPrefs.SetInt("Level", levelNumber);
            PlayerPrefs.Save();
            Debug.Log("Level set to " + levelNumber);
        }

        if (GUILayout.Button("Get Current Level"))
        {
            levelNumber = PlayerPrefs.GetInt("Level", 1);
            Debug.Log("Current level is " + levelNumber);
        }
    }
    
}