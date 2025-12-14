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
        LoadProgress();
        UpdateUI();
    }

    void LoadProgress()
    {
        currentUnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        
        Debug.Log($"[MapManager] ========== LOADING PROGRESS ==========");
        Debug.Log($"[MapManager] Current Unlocked Level: {currentUnlockedLevel}");

        for (int i = 0; i < 5; i++)
        {
            levelScores[i] = PlayerPrefs.GetInt($"Level{i + 1}Score", 0);
            levelCompleted[i] = PlayerPrefs.GetInt($"Level{i + 1}Completed", 0) == 1;
            
            Debug.Log($"[MapManager] Level {i + 1} - Score: {levelScores[i]}, Completed: {levelCompleted[i]}");
        }
        
        Debug.Log($"[MapManager] ========================================");
    }

    void UpdateUI()
    {
        // Update total score
        int totalScore = 0;
        for (int i = 0; i < 5; i++)
        {
            totalScore += levelScores[i];
        }
        
        if (totalScoreText != null)
        {
            totalScoreText.text = $"Total Score: {totalScore}";
        }

        Debug.Log($"[MapManager] ========== UPDATING UI ==========");
        Debug.Log($"[MapManager] Total Score: {totalScore}");

        // Update each level button
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1;
            bool isUnlocked = levelNumber <= currentUnlockedLevel;
            bool isCompleted = levelCompleted[i];
            int score = levelScores[i];

            Debug.Log($"[MapManager] Updating Level {levelNumber} Button - Unlocked: {isUnlocked}, Completed: {isCompleted}, Score: {score}");

            levelButtons[i].Setup(levelNumber, isUnlocked, isCompleted, score, currentUnlockedLevel);
        }
        
        Debug.Log($"[MapManager] =====================================");
    }

    public void LoadLevel(int levelNumber)
    {
        // Check if level is unlocked
        if (levelNumber <= currentUnlockedLevel)
        {
            // Store which level to load
            PlayerPrefs.SetInt("CurrentLevel", levelNumber);
            PlayerPrefs.Save();

            // UPDATED: All levels 1-5 are in Sandbox scene
            SceneManager.LoadScene("Sandbox");
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
        Debug.Log($"[MapManager] ========== LEVEL COMPLETED ==========");
        Debug.Log($"[MapManager] Level {levelNumber} completed with score: {score}");
        
        // Save completion
        PlayerPrefs.SetInt($"Level{levelNumber}Completed", 1);
        Debug.Log($"[MapManager] Saved Level{levelNumber}Completed = 1");
        
        // Save score (only if better)
        int previousScore = PlayerPrefs.GetInt($"Level{levelNumber}Score", 0);
        if (score > previousScore)
        {
            PlayerPrefs.SetInt($"Level{levelNumber}Score", score);
            Debug.Log($"[MapManager] Saved Level{levelNumber}Score = {score} (previous: {previousScore})");
        }
        else
        {
            Debug.Log($"[MapManager] Score {score} not better than previous {previousScore}, not updating");
        }

        // Unlock next level
        int currentUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        Debug.Log($"[MapManager] Current UnlockedLevel: {currentUnlocked}");
        
        if (levelNumber >= currentUnlocked && levelNumber < 5)
        {
            int newUnlocked = levelNumber + 1;
            PlayerPrefs.SetInt("UnlockedLevel", newUnlocked);
            Debug.Log($"[MapManager] ✅ Unlocked Level {newUnlocked}! (UnlockedLevel = {newUnlocked})");
        }
        else
        {
            Debug.Log($"[MapManager] Level {levelNumber} already unlocked or max level reached. No unlock needed.");
        }

        PlayerPrefs.Save();
        Debug.Log($"[MapManager] PlayerPrefs saved!");
        Debug.Log($"[MapManager] =======================================");
    }
}