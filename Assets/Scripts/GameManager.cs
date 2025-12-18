using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int[] zombiesToKillPerLevel = new int[5] { 3, 5, 7, 10, 15 };
    private int zombiesToKill;

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
    public GameObject pausePanel;
    public GameObject pauseButton;
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
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        
        if (currentLevel >= 1 && currentLevel <= zombiesToKillPerLevel.Length)
        {
            zombiesToKill = zombiesToKillPerLevel[currentLevel - 1];
        }
        
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.StopMusic();
        }
        
        SetupLevel(currentLevel);
        UpdateUI();

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }
    }

    void SetupLevel(int levelNum)
    {
        GameObject level1 = GameObject.Find("Level-1");
        GameObject level2 = GameObject.Find("Level-2");
        GameObject level3 = GameObject.Find("Level-3");
        GameObject level4 = GameObject.Find("Level-4");
        GameObject level5 = GameObject.Find("Level-5");

        if (level1 != null) level1.SetActive(false);
        if (level2 != null) level2.SetActive(false);
        if (level3 != null) level3.SetActive(false);
        if (level4 != null) level4.SetActive(false);
        if (level5 != null) level5.SetActive(false);

        GameObject currentLevelArea = null;
        
        switch (levelNum)
        {
            case 1:
                if (level1 != null)
                {
                    level1.SetActive(true);
                    currentLevelArea = level1;
                }
                break;
            case 2:
                if (level2 != null)
                {
                    level2.SetActive(true);
                    currentLevelArea = level2;
                }
                break;
            case 3:
                if (level3 != null)
                {
                    level3.SetActive(true);
                    currentLevelArea = level3;
                }
                break;
            case 4:
                if (level4 != null)
                {
                    level4.SetActive(true);
                    currentLevelArea = level4;
                }
                break;
            case 5:
                if (level5 != null)
                {
                    level5.SetActive(true);
                    currentLevelArea = level5;
                }
                break;
        }

        if (currentLevelArea != null)
        {
            StartCoroutine(SpawnPlayerNextFrame(levelNum, currentLevelArea));
        }
    }

    void SpawnPlayerAtLevelStart(int levelNum)
    {
        LevelSpawnPoint[] allSpawns = Resources.FindObjectsOfTypeAll<LevelSpawnPoint>();
        
        foreach (LevelSpawnPoint spawn in allSpawns)
        {
            if (spawn.levelNumber == levelNum)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    spawn.gameObject.SetActive(true);
                    player.transform.position = spawn.transform.position;
                    player.transform.rotation = spawn.transform.rotation;
                    return;
                }
            }
        }
    }

    IEnumerator SpawnPlayerNextFrame(int levelNum, GameObject levelArea)
    {
        yield return null;
        
        LevelSpawnPoint spawnPoint = levelArea.GetComponentInChildren<LevelSpawnPoint>(true);
        
        if (spawnPoint == null)
        {
            LevelSpawnPoint[] allSpawns = FindObjectsOfType<LevelSpawnPoint>();
            foreach (LevelSpawnPoint spawn in allSpawns)
            {
                if (spawn.levelNumber == levelNum)
                {
                    spawnPoint = spawn;
                    break;
                }
            }
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;
        
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        
        if (spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;
        }
        else
        {
            Vector3 fallbackPosition = levelArea.transform.position + Vector3.up * 2f;
            player.transform.position = fallbackPosition;
            player.transform.rotation = levelArea.transform.rotation;
        }
        
        if (cc != null) cc.enabled = true;
        
        FindCurrentLevelDoor();
    }

    void FindCurrentLevelDoor()
    {
        LevelDoor[] doors = FindObjectsOfType<LevelDoor>();
        
        foreach (LevelDoor door in doors)
        {
            if (door.gameObject.activeInHierarchy)
            {
                levelDoor = door;
                break;
            }
        }
    }

    void Update()
    {
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

        if (zombiesKilled >= zombiesToKill && levelDoor != null && levelDoor.isLocked)
        {
            UnlockExitDoor();
        }
    }

    void UnlockExitDoor()
    {
        if (levelDoor != null)
        {
            levelDoor.UnlockDoor();
        }
        else
        {
            FindCurrentLevelDoor();
            if (levelDoor != null)
            {
                levelDoor.UnlockDoor();
            }
        }
    }

    public void LoadNextLevel()
    {
        MapManager.LevelCompleted(currentLevel, currentLevelScore);
        Time.timeScale = 1f;
        SceneManager.LoadScene("Map");
    }

    public void LoadMainMenu()
    {
        if (MusicManager.Instance != null && !MusicManager.Instance.IsMuted())
        {
            MusicManager.Instance.AllowMusicResume();
        }
        
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

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        WeaponeSwitcher weaponSwitcher = FindObjectOfType<WeaponeSwitcher>();
        if (weaponSwitcher != null)
        {
            weaponSwitcher.enabled = false;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }

        #if UNITY_STANDALONE
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        #endif

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