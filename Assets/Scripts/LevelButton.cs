using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI levelNumberText;
    public GameObject lockIcon;
    public GameObject completedIcon;
    public TextMeshProUGUI levelScoreText;
    public TextMeshProUGUI lockedText;
    public Button button;

    [Header("Settings")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = Color.gray;

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

    public void Setup(int level, bool unlocked, bool completed, int score)
    {
        levelNumber = level;
        isUnlocked = unlocked;

        // Update level number
        if (levelNumberText != null)
        {
            levelNumberText.text = level.ToString();
        }

        // Show/hide lock icon
        if (lockIcon != null)
        {
            lockIcon.SetActive(!unlocked);
        }

        // Show/hide completed checkmark
        if (completedIcon != null)
        {
            completedIcon.SetActive(completed);
        }

        // Update score text
        if (levelScoreText != null)
        {
            if (unlocked)
            {
                levelScoreText.text = $"Score: {score}";
                levelScoreText.gameObject.SetActive(true);
            }
            else
            {
                levelScoreText.gameObject.SetActive(false);
            }
        }

        // Show/hide locked text
        if (lockedText != null)
        {
            lockedText.gameObject.SetActive(!unlocked);  // ← ERROR HERE (Line 76)
        }

        // Change button color
        Image buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = unlocked ? unlockedColor : lockedColor;
        }

        // Enable/disable button interaction
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