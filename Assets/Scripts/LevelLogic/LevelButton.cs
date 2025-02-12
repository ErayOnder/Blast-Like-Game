using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private Button levelButton;
    [SerializeField] private TextMeshProUGUI levelText;

    private void Awake()
    {
        levelButton.onClick.RemoveAllListeners();
        levelButton.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        int level = PlayerPrefs.GetInt("Level", 1);

        if (level > 10)
        {
            levelText.text = "Finished";
            GameManager.Instance.ResetGame();
        }
        else
        {
            levelText.text = "Level " + level;
            GameManager.Instance.LoadLevel();
        }
    }
}
