using UnityEngine;

// Role: Manages level data and initialization.
public class LevelManager : Singleton<LevelManager>
{
    public GameGrid gameGrid;
    
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
