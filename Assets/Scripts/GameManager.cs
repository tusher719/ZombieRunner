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
    private int currentLevelScore = 0;

    [Header("Zombie Tracking")]
    private int zombiesKilled = 0;

    [Header("UI References")]
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI scoreText;

    [Header("Door Reference")]
    public LevelDoor levelDoor;

    [Header("Pause System")]
    public GameObject pausePanel;      // The panel that shows when paused
    public GameObject pauseButton;     // The button to trigger pause
    private bool isPaused = false;

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
        // Load which level we're playing
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        
        UpdateUI();

        // Door handles its own initial state in LevelDoor.cs Start()
        // No need to set active here

        // Initialize pause UI
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }
    }

    void Update()
    {
        // Check for pause input (ESC key for PC)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ZombieKilled()
    {
        zombiesKilled++;
        currentLevelScore += pointsPerZombie;

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
            levelDoor.UnlockDoor();
        }

        // Save progress
        MapManager.LevelCompleted(currentLevel, currentLevelScore);
    }

    public void LoadNextLevel()
    {
        // Return to map scene
        Time.timeScale = 1f;
        SceneManager.LoadScene("Map");
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
            scoreText.text = $"Score: {currentLevelScore}";
        }
    }

    public int GetCurrentLevelScore()
    {
        return currentLevelScore;
    }

    // ========== PAUSE SYSTEM ==========

    // Called by Pause Button (Mobile/PC UI)
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // Show pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        // Hide pause button
        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable weapon switching during pause
        WeaponeSwitcher weaponSwitcher = FindObjectOfType<WeaponeSwitcher>();
        if (weaponSwitcher != null)
        {
            weaponSwitcher.enabled = false;
        }
    }

    // Called by Continue Button
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        // Hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Show pause button
        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }

        // Hide cursor (for PC gameplay)
        #if UNITY_STANDALONE
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        #endif

        // Re-enable weapon switching
        WeaponeSwitcher weaponSwitcher = FindObjectOfType<WeaponeSwitcher>();
        if (weaponSwitcher != null)
        {
            weaponSwitcher.enabled = true;
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}