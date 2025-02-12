using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public GameGrid gameGrid;
    
    // Property to hold the current level info so other managers can access it.
    public LevelInfo CurrentLevelInfo { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        LevelInfo levelInfo = LevelLoader.LoadLevel();

        if (levelInfo != null)
        {
            CurrentLevelInfo = levelInfo;
            
            gameGrid.BuildGrid(CurrentLevelInfo);
            
            ItemManager.Instance.InitializeItems(CurrentLevelInfo);
        }
        else
        {
            Debug.LogError("Failed to load level data.");
        }
    }
}
