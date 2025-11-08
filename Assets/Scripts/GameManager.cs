using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int zombiesToKill = 10;

    [Header("Score Settings")]
    public int pointsPerZombie = 10;
    private int totalScore = 0;

    [Header("Zombie Tracking")]
    private int zombiesKilled = 0;

    [Header("UI References")]
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI scoreText;

    [Header("Door Reference")]
    public GameObject levelDoor;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();

        if (levelDoor != null)
        {
            levelDoor.SetActive(false);
        }
    }

    public void ZombieKilled()
    {
        zombiesKilled++;
        totalScore += pointsPerZombie;

        UpdateUI();

        if (zombiesKilled >= zombiesToKill)
        {
            LevelComplete();
        }
    }

    void LevelComplete()
    {
        if (levelDoor != null)
        {
            levelDoor.SetActive(true);
        }
    }

    public void LoadNextLevel()
    {
        // For now, just reload same scene for testing
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main");
    }

    void UpdateUI()
    {
        if (killCountText != null)
        {
            killCountText.text = $"{zombiesKilled}/{zombiesToKill}";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Score: {totalScore}";
        }
    }

    public int GetTotalScore()
    {
        return totalScore;
    }
}