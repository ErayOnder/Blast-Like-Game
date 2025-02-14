using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Button levelButton;
    [SerializeField] private TextMeshProUGUI levelText;

    // This flag tracks for level 1 whether we've already switched from "Start" to "Level 1"
    private bool startPressed = false;

    private void Awake()
    {
        levelButton.onClick.RemoveAllListeners();
        levelButton.onClick.AddListener(OnButtonPressed);
    }

    private void Start()
    {
        int level = PlayerPrefs.GetInt("Level", 1);
        
        if (level > 10)
        {
            levelText.text = "Finished";
            startPressed = true; // So that clicking resets the game.
        }
        else if (level == 1)
        {
            // For level 1, show "Start" initially.
            levelText.text = "Start";
            startPressed = false;
        }
        else // For any level greater than 1
        {
            levelText.text = "Level " + level;
            startPressed = true;
        }
    }

    private void OnButtonPressed()
    {
        int level = PlayerPrefs.GetInt("Level", 1);
        
        if (level > 10)
        {
            GameManager.Instance.ResetGame();
        }
        else if (level == 1 && !startPressed)
        {
            // When level is 1 and the button still shows "Start", update the text to "Level 1"
            // but do not load the level yet.
            levelText.text = "Level 1";
            startPressed = true;
        }
        else
        {
            // For level 1 (after the first click) and for all other levels,
            // clicking the button loads the level.
            GameManager.Instance.LoadLevel();
        }
    }
}
