using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsManager : MonoBehaviour
{
    [Header("Background Music Toggle")]
    public GameObject bgMusicToggle;        // The clickable box image
    public GameObject bgMusicCheckmark;     // The checkmark image (child)
    
    [Header("UI Music Toggle")]
    public GameObject uiMusicToggle;        // The clickable box image
    public GameObject uiMusicCheckmark;     // The checkmark image (child)

    [Header("Button References")]
    public Button saveButton;
    public Button backButton;

    // Internal state
    private bool bgMusicOn = true;
    private bool uiMusicOn = true;

    void Start()
    {
        // Load saved settings
        LoadSettings();
        
        // Update visual state
        UpdateTogglesVisual();
        
        // Add click listeners to toggles
        AddClickListener(bgMusicToggle, OnBgMusicToggleClicked);
        AddClickListener(uiMusicToggle, OnUIMusicToggleClicked);
        
        // Add listener to save button
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveSettings);
        }
    }

    void LoadSettings()
    {
        // Load background music setting
        // 0 = muted (OFF), 1 = playing (ON)
        bgMusicOn = PlayerPrefs.GetInt("BackgroundMusicMuted", 0) == 0;
        
        // Load UI music setting
        uiMusicOn = PlayerPrefs.GetInt("UIMusicMuted", 0) == 0;
        
    }

    void UpdateTogglesVisual()
    {
        // Update background music checkmark
        if (bgMusicCheckmark != null)
        {
            bgMusicCheckmark.SetActive(bgMusicOn);
        }
        
        // Update UI music checkmark
        if (uiMusicCheckmark != null)
        {
            uiMusicCheckmark.SetActive(uiMusicOn);
        }
    }

    void AddClickListener(GameObject toggleObject, UnityEngine.Events.UnityAction action)
    {
        if (toggleObject == null) return;
        
        // Try to get Button component first
        Button button = toggleObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(action);
            return;
        }
        
        // If no Button, add EventTrigger for click
        EventTrigger trigger = toggleObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = toggleObject.AddComponent<EventTrigger>();
        }
        
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { action.Invoke(); });
        trigger.triggers.Add(entry);
    }

    void OnBgMusicToggleClicked()
    {
        // Toggle state
        bgMusicOn = !bgMusicOn;
        
        // Update visual
        if (bgMusicCheckmark != null)
        {
            bgMusicCheckmark.SetActive(bgMusicOn);
        }
        
        // Apply to music manager immediately
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetMuted(!bgMusicOn);
        }
        
    }

    void OnUIMusicToggleClicked()
    {
        // Toggle state
        uiMusicOn = !uiMusicOn;
        
        // Update visual
        if (uiMusicCheckmark != null)
        {
            uiMusicCheckmark.SetActive(uiMusicOn);
        }
        
        // UI music control (for future use)
        
    }

    public void SaveSettings()
    {
        // Save background music setting
        // ON = 0 (not muted), OFF = 1 (muted)
        int bgMusicMuted = bgMusicOn ? 0 : 1;
        PlayerPrefs.SetInt("BackgroundMusicMuted", bgMusicMuted);
        
        // Save UI music setting
        int uiMusicMuted = uiMusicOn ? 0 : 1;
        PlayerPrefs.SetInt("UIMusicMuted", uiMusicMuted);
        
        // Save to disk
        PlayerPrefs.Save();
        
        
        // Optional: Show feedback
        // StartCoroutine(ShowSaveText());
    }

    void OnDestroy()
    {
        // Remove save button listener
        if (saveButton != null)
        {
            saveButton.onClick.RemoveListener(SaveSettings);
        }
    }
}