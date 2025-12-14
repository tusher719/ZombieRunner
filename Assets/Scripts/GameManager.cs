using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

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
        // Load which level we're playing
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        
        Debug.Log($"[GameManager] Starting Level {currentLevel}");
        
        // Activate correct level area and spawn player
        SetupLevel(currentLevel);
        
        UpdateUI();

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

    void SetupLevel(int levelNum)
    {
        Debug.Log($"[GameManager] SetupLevel called for Level {levelNum}");
        
        // Find all level areas (1-5) - All in Sandbox scene
        GameObject level1 = GameObject.Find("Level-1");
        GameObject level2 = GameObject.Find("Level-2");
        GameObject level3 = GameObject.Find("Level-3");
        GameObject level4 = GameObject.Find("Level-4");
        GameObject level5 = GameObject.Find("Level-5");

        // Deactivate all first
        if (level1 != null) level1.SetActive(false);
        if (level2 != null) level2.SetActive(false);
        if (level3 != null) level3.SetActive(false);
        if (level4 != null) level4.SetActive(false);
        if (level5 != null) level5.SetActive(false);

        // Activate current level and store reference
        GameObject currentLevelArea = null;
        
        switch (levelNum)
        {
            case 1:
                if (level1 != null)
                {
                    level1.SetActive(true);
                    currentLevelArea = level1;
                    Debug.Log("[GameManager] Level-1 Area activated");
                }
                else
                {
                    Debug.LogError("[GameManager] Level-1 not found!");
                }
                break;
            case 2:
                if (level2 != null)
                {
                    level2.SetActive(true);
                    currentLevelArea = level2;
                    Debug.Log("[GameManager] Level-2 Area activated");
                }
                else
                {
                    Debug.LogError("[GameManager] Level-2 not found!");
                }
                break;
            case 3:
                if (level3 != null)
                {
                    level3.SetActive(true);
                    currentLevelArea = level3;
                    Debug.Log("[GameManager] Level-3 Area activated");
                }
                else
                {
                    Debug.LogError("[GameManager] Level-3 not found!");
                }
                break;
            case 4:
                if (level4 != null)
                {
                    level4.SetActive(true);
                    currentLevelArea = level4;
                    Debug.Log("[GameManager] Level-4 Area activated");
                }
                else
                {
                    Debug.LogError("[GameManager] Level-4 not found!");
                }
                break;
            case 5:
                if (level5 != null)
                {
                    level5.SetActive(true);
                    currentLevelArea = level5;
                    Debug.Log("[GameManager] Level-5 Area activated");
                }
                else
                {
                    Debug.LogError("[GameManager] Level-5 not found!");
                }
                break;
        }

        // CRITICAL: Wait one frame for Unity to process hierarchy changes
        // Then spawn player in the now-active area
        if (currentLevelArea != null)
        {
            StartCoroutine(SpawnPlayerNextFrame(levelNum, currentLevelArea));
        }
        else
        {
            Debug.LogError($"[GameManager] Could not find Level-{levelNum}!");
        }
    }

    void SpawnPlayerAtLevelStart(int levelNum)
    {
        // Find ALL spawn points (even in inactive objects)
        LevelSpawnPoint[] allSpawns = Resources.FindObjectsOfTypeAll<LevelSpawnPoint>();
        
        Debug.Log($"[GameManager] Found {allSpawns.Length} total spawn points");
        
        foreach (LevelSpawnPoint spawn in allSpawns)
        {
            Debug.Log($"[GameManager] Spawn point: Level {spawn.levelNumber}, Active: {spawn.gameObject.activeInHierarchy}");
            
            if (spawn.levelNumber == levelNum)
            {
                // Found correct spawn point
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    // Make sure spawn point's parent is active
                    spawn.gameObject.SetActive(true);
                    
                    player.transform.position = spawn.transform.position;
                    player.transform.rotation = spawn.transform.rotation;
                    
                    Debug.Log($"[GameManager] Player spawned at Level {levelNum} spawn: {spawn.transform.position}");
                    return;
                }
            }
        }
        
        Debug.LogWarning($"[GameManager] No spawn point found for Level {levelNum}!");
    }

    // নতুন Coroutine - Area activate হওয়ার পর spawn করে
    System.Collections.IEnumerator SpawnPlayerNextFrame(int levelNum, GameObject levelArea)
    {
        // Unity কে hierarchy process করার সময় দিচ্ছি (1 frame wait)
        yield return null;
        
        Debug.Log($"[GameManager] Spawning player for Level {levelNum} after area activation");
        
        // এখন active area তে spawn point খুঁজব - MULTIPLE methods
        LevelSpawnPoint spawnPoint = null;
        
        // Method 1: Search in children
        spawnPoint = levelArea.GetComponentInChildren<LevelSpawnPoint>(true);
        
        // Method 2: If not found, search all active spawn points
        if (spawnPoint == null)
        {
            Debug.LogWarning($"[GameManager] Spawn point not found as child, searching all active spawn points...");
            
            LevelSpawnPoint[] allSpawns = FindObjectsOfType<LevelSpawnPoint>();
            foreach (LevelSpawnPoint spawn in allSpawns)
            {
                if (spawn.levelNumber == levelNum)
                {
                    spawnPoint = spawn;
                    Debug.Log($"[GameManager] Found spawn point by level number: {spawn.gameObject.name}");
                    break;
                }
            }
        }
        
        // Method 3: Fallback - spawn at level area position
        if (spawnPoint == null)
        {
            Debug.LogWarning($"[GameManager] ⚠️ No LevelSpawnPoint found for Level {levelNum}! Using level area position as fallback.");
            
            // Spawn player at level area's position as fallback
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                
                // Use level area position + offset
                Vector3 fallbackPosition = levelArea.transform.position + Vector3.up * 2f;
                player.transform.position = fallbackPosition;
                player.transform.rotation = levelArea.transform.rotation;
                
                if (cc != null) cc.enabled = true;
                
                Debug.Log($"[GameManager] ⚠️ Player spawned at fallback position: {fallbackPosition}");
            }
        }
        else
        {
            // Normal spawn with spawn point found
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // CharacterController থাকলে disable করে teleport করব
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null)
                {
                    cc.enabled = false;
                }
                
                // Player কে spawn point এ teleport করছি
                player.transform.position = spawnPoint.transform.position;
                player.transform.rotation = spawnPoint.transform.rotation;
                
                // CharacterController আবার enable করছি
                if (cc != null)
                {
                    cc.enabled = true;
                }
                
                Debug.Log($"[GameManager] ✅ Player spawned at Level {levelNum}: {spawnPoint.transform.position}");
            }
            else
            {
                Debug.LogError("[GameManager] Player not found with tag 'Player'!");
            }
        }
        
        // এখন door খুঁজব
        FindCurrentLevelDoor();
    }

    void FindCurrentLevelDoor()
    {
        // Find active door in current level
        LevelDoor[] doors = FindObjectsOfType<LevelDoor>();
        
        foreach (LevelDoor door in doors)
        {
            if (door.gameObject.activeInHierarchy)
            {
                levelDoor = door;
                Debug.Log($"[GameManager] Door found: {door.gameObject.name}");
                break;
            }
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

    // ========== ZOMBIE & SCORE SYSTEM (UPDATED) ==========

    public void ZombieKilled()
    {
        zombiesKilled++;
        currentLevelScore += pointsPerZombie;

        UpdateUI();

        Debug.Log($"[GameManager] 🎯 Zombies killed: {zombiesKilled}/{zombiesToKill}");
        Debug.Log($"[GameManager] Score: {currentLevelScore}");

        // Check if door should be unlocked (at threshold)
        // IMPORTANT: Only unlock once, don't save yet!
        if (zombiesKilled >= zombiesToKill && levelDoor != null && levelDoor.isLocked)
        {
            Debug.Log($"[GameManager] ✅ Target reached! Unlocking door...");
            UnlockExitDoor();
        }
        else if (zombiesKilled < zombiesToKill)
        {
            Debug.Log($"[GameManager] Need {zombiesToKill - zombiesKilled} more zombies");
        }
        else if (zombiesKilled > zombiesToKill)
        {
            Debug.Log($"[GameManager] 💀 Extra kill! Total: {zombiesKilled} kills, Score: {currentLevelScore}");
        }
    }

    void UnlockExitDoor()
    {
        Debug.Log($"[GameManager] 🚪 Unlocking door for Level {currentLevel}");
        
        if (levelDoor != null)
        {
            Debug.Log($"[GameManager] ✅ Door found: {levelDoor.gameObject.name}");
            levelDoor.UnlockDoor();
        }
        else
        {
            Debug.LogError("[GameManager] ❌ Door reference is NULL! Trying to find door...");
            FindCurrentLevelDoor();
            
            if (levelDoor != null)
            {
                Debug.Log("[GameManager] ✅ Door found on retry! Unlocking...");
                levelDoor.UnlockDoor();
            }
            else
            {
                Debug.LogError("[GameManager] ❌ Still can't find door!");
            }
        }
    }

    public void LoadNextLevel()
    {
        // IMPORTANT: Save progress with FINAL score (including all extra kills!)
        // This is called when player enters the unlocked door
        Debug.Log($"[GameManager] 💾 Saving Level {currentLevel} progress...");
        Debug.Log($"[GameManager] Final Stats - Kills: {zombiesKilled}, Score: {currentLevelScore}");
        
        MapManager.LevelCompleted(currentLevel, currentLevelScore);
        
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