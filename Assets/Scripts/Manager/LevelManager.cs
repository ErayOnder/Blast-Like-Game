using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public GameGrid gameGrid;
    
    // Property to hold the current level info so other managers can access it.
    public LevelInfo CurrentLevelInfo { get; private set; }
    public LevelData CurrentLevelData { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
        LevelInfo levelInfo = LevelLoader.LoadLevel();

        if (levelInfo != null)
        {
            CurrentLevelInfo = levelInfo;
            CurrentLevelData = new LevelData(CurrentLevelInfo);
            
            gameGrid.BuildGrid(CurrentLevelInfo);
                        
            LevelProgress.Instance.Initialize(CurrentLevelData);
            ItemManager.Instance.InitializeItems(CurrentLevelData);
            CascadeManager.Instance.UpdateBonusGroups();
        }
        else
        {
            Debug.LogError("Failed to load level data.");
        }
    }
}
