using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalObject : MonoBehaviour
{
    [SerializeField] private Image goalImage;
    [SerializeField] private Image completedMarkImage;
    [SerializeField] private TextMeshProUGUI goalCountText;
    [SerializeField] private ItemSpriteConfig spriteConfig;
    
    private int goalCount;
    private LevelGoal levelGoal;
    
    public LevelGoal LevelGoal => levelGoal;
    
    public void Prepare(LevelGoal goal)
    {
        levelGoal = goal;
    
        if (spriteConfig != null)
        {
            var goalSprite = spriteConfig.GetSpriteForItemType(levelGoal.ItemType);
            if (goalSprite != null)
            {
                goalImage.sprite = goalSprite;
            }
            else
            {
                Debug.LogWarning("GoalObject: Sprite not found for " + levelGoal.ItemType);
            }
        }
        else
        {
            Debug.LogWarning("GoalObject: Sprite config not assigned.");
        }
    
        goalCount = levelGoal.Count;
        UpdateGoalCountText();
    
        if (completedMarkImage != null)
        {
            completedMarkImage.gameObject.SetActive(false);
        }
    }
    
    public void DecreaseCount()
    {
        if (goalCount <= 0)
            return;
    
        goalCount--;
    
        if (goalCount <= 0)
        {
            MarkGoalAsCompleted();
        }
        else
        {
            UpdateGoalCountText();
        }
    }
    
    private void UpdateGoalCountText()
    {
        if (goalCountText != null)
        {
            goalCountText.text = goalCount.ToString();
        }
    }

    private void MarkGoalAsCompleted()
    {
        goalCount = 0;
        if (goalCountText != null)
        {
            goalCountText.gameObject.SetActive(false);
        }
        if (completedMarkImage != null)
        {
            completedMarkImage.gameObject.SetActive(true);
        }
    }
    
}