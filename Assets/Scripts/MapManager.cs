using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MapManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI totalScoreText;
    public LevelButton[] levelButtons; // Array of 5 level buttons

    [Header("Level Data")]
    private int currentUnlockedLevel = 1;
    private int[] levelScores = new int[5]; // Scores for levels 1-5
    private bool[] levelCompleted = new bool[5];

    void Start()
    {
        LoadProgress();
        UpdateUI();
    }

    void LoadProgress()
    {
        // Load from PlayerPrefs
        currentUnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < 5; i++)
        {
            levelScores[i] = PlayerPrefs.GetInt($"Level{i + 1}Score", 0);
            levelCompleted[i] = PlayerPrefs.GetInt($"Level{i + 1}Completed", 0) == 1;
        }
    }

    void UpdateUI()
    {
        // Update total score
        int totalScore = 0;
        for (int i = 0; i < 5; i++)
        {
            totalScore += levelScores[i];
        }
        totalScoreText.text = $"Total Score: {totalScore}";

        // Update each level button
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1;
            bool isUnlocked = levelNumber <= currentUnlockedLevel;
            bool isCompleted = levelCompleted[i];
            int score = levelScores[i];

            levelButtons[i].Setup(levelNumber, isUnlocked, isCompleted, score);
        }
    }

    public void LoadLevel(int levelNumber)
    {
        // Check if level is unlocked
        if (levelNumber <= currentUnlockedLevel)
        {
            // Store which level to load
            PlayerPrefs.SetInt("CurrentLevel", levelNumber);

            // Load appropriate scene
            if (levelNumber <= 3)
            {
                // Levels 1-3 are in Sandbox
                SceneManager.LoadScene("Sandbox");
            }
            else if (levelNumber == 4)
            {
                SceneManager.LoadScene("Level4");
            }
            else if (levelNumber == 5)
            {
                SceneManager.LoadScene("Level5");
            }
        }
        else
        {
            Debug.Log("Level is locked!");
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main");
    }

    // Called when a level is completed
    public static void LevelCompleted(int levelNumber, int score)
    {
        // Save completion
        PlayerPrefs.SetInt($"Level{levelNumber}Completed", 1);
        
        // Save score (only if better)
        int previousScore = PlayerPrefs.GetInt($"Level{levelNumber}Score", 0);
        if (score > previousScore)
        {
            PlayerPrefs.SetInt($"Level{levelNumber}Score", score);
        }

        // Unlock next level
        int currentUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (levelNumber >= currentUnlocked && levelNumber < 5)
        {
            PlayerPrefs.SetInt("UnlockedLevel", levelNumber + 1);
        }

        PlayerPrefs.Save();
    }
}