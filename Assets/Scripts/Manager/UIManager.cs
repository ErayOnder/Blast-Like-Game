using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Manages UI elements for level completion and failure.
public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI levelNumberTextCompleted;
    [SerializeField] private TextMeshProUGUI levelNumberTextFailed;

    [SerializeField] private GameObject levelCompletedPanel;
    [SerializeField] private Button nextButton;

    [SerializeField] private GameObject levelFailedPanel;
    [SerializeField] private Button retryButton;

    private void OnEnable()
    {
        if (LevelProgress.Instance != null)
        {
            LevelProgress.Instance.OnLevelWon.AddListener(SetLevelCompletedPanel);
            LevelProgress.Instance.OnLevelFailed.AddListener(SetLostPanel);
        }
    }

    private void OnDisable()
    {
        if (LevelProgress.Instance != null)
        {
            LevelProgress.Instance.OnLevelWon.RemoveListener(SetLevelCompletedPanel);
            LevelProgress.Instance.OnLevelFailed.RemoveListener(SetLostPanel);
        }
    }

    public void SetLevelCompletedPanel()
    {
        levelNumberTextCompleted.text = "Level " + PlayerPrefs.GetInt("Level", 1);
        levelCompletedPanel.SetActive(true);
        nextButton.onClick.RemoveAllListeners();

        if (GameManager.Instance == null)
            throw new System.Exception("ILLEGAL STATE: Please load the game from MainScene.");

        nextButton.onClick.AddListener(() => GameManager.Instance.LevelCompleted());
    }

    public void SetLostPanel()
    {
        levelNumberTextFailed.text = "Level " + PlayerPrefs.GetInt("Level", 1);
        levelFailedPanel.SetActive(true);

        if (GameManager.Instance == null)
            throw new System.Exception("ILLEGAL STATE: Please load the game from MainScene.");

        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(() => GameManager.Instance.LoadLevel());
    }
}
