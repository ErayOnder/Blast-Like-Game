using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Threading.Tasks;

// LevelProgress: Manages remaining moves and goal tracking; triggers win/fail events.
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

    private bool levelEnded = false;

    // Init: sets up moves and goal counts from level data.
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
        levelEnded = false;

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
        if (levelEnded)
            return;

        MovesLeft--;
        UpdateMovesText();

        CheckWin();

        if (!levelEnded)
        {
            StartCoroutine(DelayedCheckFailure());
        }
    }

    // Waits a frame then verifies failure condition.
    private IEnumerator DelayedCheckFailure()
    {
        yield return null;
        if (!levelEnded)
        {
            CheckFailure();
        }
    }

    // Async process: decrements move count and checks for failure after delay.
    public async Task ProcessMoveAsync()
    {
        if (levelEnded)
            return;

        MovesLeft--;
        UpdateMovesText();

        CheckWin();

        if (!levelEnded && MovesLeft <= 0 && Goals.Count > 0)
        {
            await Task.Delay(System.TimeSpan.FromSeconds(1));
            if (!levelEnded && Goals.Count > 0)
            {
                levelEnded = true;
                OnLevelFailed?.Invoke();
            }
        }
    }

    private void CheckWin()
    {
        if (!levelEnded && Goals.Count == 0)
        {
            levelEnded = true;
            OnLevelWon?.Invoke();
        }
    }

    private void CheckFailure()
    {
        if (!levelEnded && MovesLeft <= 0 && Goals.Count > 0)
        {
            levelEnded = true;
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

    // Updates goal count for an obstacle and checks win condition.
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
}
