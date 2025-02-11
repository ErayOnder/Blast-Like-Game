using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public GameGrid gameGrid;

    private void Start()
    {
        LevelData levelData = LevelLoader.LoadLevelData();

        if (levelData != null)
        {
            gameGrid.BuildGrid(levelData);
        }
        else
        {
            Debug.LogError("Failed to load level data.");
        }
    }
}
