using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Role: Manages game state, including level loading and resetting.
public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private GameObject loadScreen;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void ResetGame()
    {
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainScene");
    }

    public void LevelCompleted()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainScene");
    }

    public void LoadLevel()
    {
        StartCoroutine(LoadLevelCoroutine("LevelScene"));
    }

    private IEnumerator LoadLevelCoroutine(string levelScene)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(levelScene);

        if (loadScreen != null)
        {
            loadScreen.SetActive(true);
        }

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        loadScreen.SetActive(false);
    }
}
