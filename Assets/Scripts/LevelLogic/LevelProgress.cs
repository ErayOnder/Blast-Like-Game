using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Threading.Tasks;

public class LevelProgress : Singleton<LevelProgress>
{
    public int MovesLeft { get; private set; }
    public Dictionary<ItemType, int> Goals { get; private set; }
    
    public UnityEvent OnLevelWon;
    public UnityEvent OnLevelFailed;

    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private GoalObject goalPrefab;
    [SerializeField] private Transform goalsParent;
    private readonly List<GoalObject> goalObjects = new();

    public void Initialize(LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("LevelProgress: Null levelData provided.");
            return;
        }

        MovesLeft = levelData.Moves;
        UpdateMovesText();

        Goals = new Dictionary<ItemType, int>();

        foreach (var goal in levelData.Goals)
        {
            if (!Goals.ContainsKey(goal.ItemType))
            {
                Goals.Add(goal.ItemType, goal.Count);
            }
            else
            {
                Goals[goal.ItemType] += goal.Count;
            }
        }

        if (goalPrefab != null && goalsParent != null)
        {
            foreach (var levelGoal in levelData.Goals)
            {
                GoalObject goalObj = Instantiate(goalPrefab, goalsParent);
                goalObj.Prepare(levelGoal);
                goalObjects.Add(goalObj);
            }
        }
        else
        {
            Debug.LogWarning("Goal prefab or goals parent is not assigned in LevelProgress.");
        }
    }

    public void ProcessMove()
    {
        MovesLeft--;
        UpdateMovesText();
        CheckFailure();
    }

    public async Task ProcessMoveAsync()
    {
        MovesLeft--;
        UpdateMovesText();

        if (MovesLeft <= 0 && Goals.Count > 0)
        {
            MovesLeft = 0;
            UpdateMovesText();

            await Task.Delay(System.TimeSpan.FromSeconds(1));
            OnLevelFailed?.Invoke();
        }
    }

    private void UpdateMovesText()
    {
        if (movesText != null)
        {
            movesText.text = MovesLeft.ToString();
        }
    }

    public void ProcessObstacleDestroyed(ItemType itemType)
    {
        if (Goals.ContainsKey(itemType))
        {
            Goals[itemType]--;
            if (Goals[itemType] <= 0)
            {
                Goals.Remove(itemType);
            }

            GoalObject goalObj = goalObjects.Find(g => g.LevelGoal.ItemType.Equals(itemType));
            if (goalObj != null)
            {
                goalObj.DecreaseCount();
            }

            CheckWin();
        }
    }

    private void CheckWin()
    {
        if (Goals.Count == 0)
        {
            OnLevelWon?.Invoke();
        }
    }

    private void CheckFailure()
    {
        if (MovesLeft <= 0 && Goals.Count > 0)
        {
            OnLevelFailed?.Invoke();
        }
    }
}
