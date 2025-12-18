using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MapManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI totalScoreText;
    public LevelButton[] levelButtons;

    [Header("Level Data")]
    private int currentUnlockedLevel = 1;
    private int[] levelScores = new int[5];
    private bool[] levelCompleted = new bool[5];

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Map")
        {
            return;
        }
        
        if (MusicManager.Instance != null && !MusicManager.Instance.IsMuted())
        {
            MusicManager.Instance.AllowMusicResume();
        }

        LoadProgress();
        UpdateUI();
    }

    void LoadProgress()
    {
        currentUnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < 5; i++)
        {
            levelScores[i] = PlayerPrefs.GetInt($"Level{i + 1}Score", 0);
            levelCompleted[i] = PlayerPrefs.GetInt($"Level{i + 1}Completed", 0) == 1;
        }
    }

    void UpdateUI()
    {
        int totalScore = 0;
        for (int i = 0; i < 5; i++)
        {
            totalScore += levelScores[i];
        }
        
        if (totalScoreText != null)
        {
            totalScoreText.text = $"Total Score: {totalScore}";
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1;
            bool isUnlocked = levelNumber <= currentUnlockedLevel;
            bool isCompleted = levelCompleted[i];
            int score = levelScores[i];

            levelButtons[i].Setup(levelNumber, isUnlocked, isCompleted, score, currentUnlockedLevel);
        }
    }

    public void LoadLevel(int levelNumber)
    {
        if (levelNumber <= currentUnlockedLevel)
        {
            PlayerPrefs.SetInt("CurrentLevel", levelNumber);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Sandbox");
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main");
    }

    public static void LevelCompleted(int levelNumber, int score)
    {
        PlayerPrefs.SetInt($"Level{levelNumber}Completed", 1);
        
        int previousScore = PlayerPrefs.GetInt($"Level{levelNumber}Score", 0);
        if (score > previousScore)
        {
            PlayerPrefs.SetInt($"Level{levelNumber}Score", score);
        }

        int currentUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        
        if (levelNumber >= currentUnlocked && levelNumber < 5)
        {
            int newUnlocked = levelNumber + 1;
            PlayerPrefs.SetInt("UnlockedLevel", newUnlocked);
        }

        PlayerPrefs.Save();
    }
}