using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    [Header("Pause System")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;
    private bool isPaused = false;

    [Header("Door Reference")]
    public GameObject levelDoor;

    void Awake()
    {
        // Singleton pattern
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
        // Initialize UI
        UpdateUI();

        // Ensure game starts unpaused
        ResumeGame();
        
        // Hide pause panel and show pause button
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }

        // Keep door hidden until objective complete
        if (levelDoor != null)
        {
            levelDoor.SetActive(false);
        }
    }

    void Update()
    {
        // ESC key to toggle pause (for PC testing)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // Called when zombie dies
    public void ZombieKilled()
    {
        zombiesKilled++;
        totalScore += pointsPerZombie;

        UpdateUI();

        Debug.Log($"Zombies Killed: {zombiesKilled}/{zombiesToKill} | Score: {totalScore}");

        // Check if level objective completed
        if (zombiesKilled >= zombiesToKill)
        {
            LevelComplete();
        }
    }

    // Level completion logic
    void LevelComplete()
    {
        Debug.Log("Level Complete! Door unlocked.");
        
        if (levelDoor != null)
        {
            levelDoor.SetActive(true);
        }
    }

    // Update UI elements
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

    // Get score for external scripts
    public int GetTotalScore()
    {
        return totalScore;
    }

    public int GetZombiesKilled()
    {
        return zombiesKilled;
    }

    // Pause/Resume Methods
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Freeze all time-based operations
        
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

        // Show cursor for UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resume normal time
        
        // Hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // Show pause button again
        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }

        // Hide cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void TogglePause()
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

    public bool IsPaused()
    {
        return isPaused;
    }

    // UI Button Methods for Pause Menu
    public void OnContinueButtonClicked()
    {
        ResumeGame();
    }

    public void OnMainMenuButtonClicked()
    {
        // Resume time before loading scene
        Time.timeScale = 1f;
        
        // Load main menu scene
        LoadMainMenu();
    }

    // Called from Pause Button in gameplay UI
    public void OnPauseButtonClicked()
    {
        PauseGame();
    }

    // Scene Loading Methods
    public void LoadNextLevel()
    {
        // Resume time before loading
        Time.timeScale = 1f;
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if next scene exists
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next level found! Loading Main Menu.");
            LoadMainMenu();
        }
    }

    public void LoadMainMenu()
    {
        // Resume time before loading
        Time.timeScale = 1f;
        
        // Load main menu scene by name
        SceneManager.LoadScene("Main");
    }

    public void RestartLevel()
    {
        // Resume time before reloading
        Time.timeScale = 1f;
        
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}