using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelNumberText;
    public GameObject lockBG;                    // Lock background image (locked এ visible)
    public GameObject lockIcon;                  // Lock icon (locked এ visible)
    public GameObject unlockIndicator;           // Unlock indicator image (unlocked + not completed এ visible) - NEW!
    public GameObject completedIcon;             // Completed checkmark image
    public TextMeshProUGUI levelScoreText;
    public Button button;
    
    [Header("Special Indicators")]
    public GameObject currentLevelText;          // "CURRENT LEVEL" text + icon
    public GameObject baseLabel;                 // "BASE" label (Level 1 only)
    public GameObject labLabel;                  // "LAB" label (Level 5 only)

    private int levelNumber;
    private bool isUnlocked;
    private MapManager mapManager;

    void Start()
    {
        mapManager = FindObjectOfType<MapManager>();
        
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        // Add click listener
        button.onClick.AddListener(OnButtonClick);
    }

    public void Setup(int level, bool unlocked, bool completed, int score, int currentUnlockedLevel)
    {
        levelNumber = level;
        isUnlocked = unlocked;

        // 1. Update level number text
        if (levelNumberText != null)
        {
            levelNumberText.text = level.ToString();
        }

        // 2. CurrentLevelText - শুধু unlocked কিন্তু NOT completed levels এ show
        if (currentLevelText != null)
        {
            bool isCurrentLevel = (unlocked && !completed);
            currentLevelText.SetActive(isCurrentLevel);
        }

        // 3. LockBG & LockIcon - locked হলে show, unlocked হলে hide (একসাথে কাজ করবে)
        bool isLocked = !unlocked;
        
        if (lockBG != null)
        {
            lockBG.SetActive(isLocked);
        }
        
        if (lockIcon != null)
        {
            lockIcon.SetActive(isLocked);
        }

        // 4. Unlock Indicator - NEW! unlocked + not completed এ show
        if (unlockIndicator != null)
        {
            bool showUnlockIndicator = (unlocked && !completed);
            unlockIndicator.SetActive(showUnlockIndicator);
        }

        // 5. Base label - শুধু Level 1 এ always visible
        if (baseLabel != null)
        {
            baseLabel.SetActive(level == 1);
        }

        // 6. Lab label - শুধু Level 5 এ always visible
        if (labLabel != null)
        {
            labLabel.SetActive(level == 5);
        }

        // 7. Completed icon - completed হলে show
        if (completedIcon != null)
        {
            completedIcon.SetActive(completed);
        }

        // 8. Score text - শুধু COMPLETED levels এ actual score show করবে
        if (levelScoreText != null)
        {
            if (completed && score > 0)
            {
                levelScoreText.text = $"Score: {score}";
                levelScoreText.gameObject.SetActive(true);
            }
            else
            {
                // Unlocked but not played, or locked - hide score
                levelScoreText.gameObject.SetActive(false);
            }
        }

        // 9. Button interactable - all unlocked levels clickable করবে (replay করতে পারবে)
        if (button != null)
        {
            button.interactable = unlocked;
        }
    }

    void OnButtonClick()
    {
        if (isUnlocked && mapManager != null)
        {
            mapManager.LoadLevel(levelNumber);
        }
    }
}