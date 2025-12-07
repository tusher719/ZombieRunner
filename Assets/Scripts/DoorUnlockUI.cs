using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DoorUnlockUI : MonoBehaviour
{
    public static DoorUnlockUI Instance;

    [Header("UI Elements")]
    public GameObject unlockMessagePanel;
    public TextMeshProUGUI unlockMessageText;
    public GameObject arrowIndicator;
    public RectTransform arrowTransform;

    [Header("Settings")]
    public float messageDuration = 3f;
    public float arrowDuration = 5f;

    private Camera mainCamera;
    private Vector3 doorWorldPosition;
    private bool showingArrow = false;

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
        mainCamera = Camera.main;

        if (unlockMessagePanel != null)
        {
            unlockMessagePanel.SetActive(false);
        }

        if (arrowIndicator != null)
        {
            arrowIndicator.SetActive(false);
        }
    }

    void Update()
    {
        if (showingArrow && arrowIndicator != null && arrowIndicator.activeSelf)
        {
            UpdateArrowPosition();
            
            // Blink effect
            Image arrowImage = arrowIndicator.GetComponent<Image>();
            if (arrowImage != null)
            {
                float alpha = 0.5f + 0.5f * Mathf.Sin(Time.time * 3f);
                Color c = arrowImage.color;
                c.a = alpha;
                arrowImage.color = c;
            }
        }
    }

    public void ShowUnlockMessage(Vector3 doorPosition)
    {
        doorWorldPosition = doorPosition;
        StartCoroutine(DisplayUnlockMessage());
    }

    public void ShowLockedMessage()
    {
        if (unlockMessagePanel != null && unlockMessageText != null)
        {
            unlockMessageText.text = "DOOR LOCKED!\nKill all zombies to unlock!";
            unlockMessageText.color = Color.red;
            unlockMessagePanel.SetActive(true);
            StartCoroutine(HideMessageAfterDelay(2f));
        }
    }

    IEnumerator DisplayUnlockMessage()
    {
        // Show unlock message
        if (unlockMessagePanel != null && unlockMessageText != null)
        {
            unlockMessageText.text = "DOOR UNLOCKED!\nHead to the exit!";
            unlockMessageText.color = Color.green;
            unlockMessagePanel.SetActive(true);
        }

        // Wait
        yield return new WaitForSeconds(messageDuration);

        // Hide message
        if (unlockMessagePanel != null)
        {
            unlockMessagePanel.SetActive(false);
        }

        // Show arrow pointing to door
        if (arrowIndicator != null)
        {
            showingArrow = true;
            arrowIndicator.SetActive(true);
            StartCoroutine(HideArrowAfterDelay());
        }
    }

    IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (unlockMessagePanel != null)
        {
            unlockMessagePanel.SetActive(false);
        }
    }

    IEnumerator HideArrowAfterDelay()
    {
        yield return new WaitForSeconds(arrowDuration);
        if (arrowIndicator != null)
        {
            arrowIndicator.SetActive(false);
            showingArrow = false;
        }
    }

    void UpdateArrowPosition()
    {
        if (mainCamera == null || arrowTransform == null)
            return;

        // Convert world position to screen position
        Vector3 screenPos = mainCamera.WorldToScreenPoint(doorWorldPosition);

        // Check if door is behind camera
        if (screenPos.z < 0)
        {
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
            screenPos.z = 0;
        }

        // Clamp to screen edges with margin
        float margin = 100f;
        screenPos.x = Mathf.Clamp(screenPos.x, margin, Screen.width - margin);
        screenPos.y = Mathf.Clamp(screenPos.y, margin, Screen.height - margin);

        // Update arrow position
        arrowTransform.position = screenPos;

        // Rotate arrow to point towards door
        Vector3 direction = doorWorldPosition - mainCamera.transform.position;
        direction.y = 0; // Keep arrow horizontal
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            arrowTransform.rotation = Quaternion.Euler(0, 0, -angle);
        }
    }
}