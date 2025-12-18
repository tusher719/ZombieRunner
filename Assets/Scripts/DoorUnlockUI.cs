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
    public float doorReachDistance = 5f; // Distance to hide arrow

    private Camera mainCamera;
    private Transform doorTransform;
    private bool showingArrow = false;
    private Transform playerTransform;

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
        
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

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
            // Check distance to door
            if (playerTransform != null && doorTransform != null)
            {
                float distance = Vector3.Distance(playerTransform.position, doorTransform.position);
                
                if (distance <= doorReachDistance)
                {
                    // Player reached door, hide arrow
                    HideArrow();
                    return;
                }
            }

            // Update arrow position and rotation
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
        // Find door transform
        GameObject door = GameObject.Find("ExitDoor");
        if (door != null)
        {
            doorTransform = door.transform;
        }

        StartCoroutine(DisplayUnlockMessage());
    }

    public void ShowLockedMessage()
    {
        if (unlockMessagePanel != null && unlockMessageText != null)
        {
            unlockMessageText.text = "DOOR LOCKED!";
            unlockMessageText.color = Color.white;
            unlockMessagePanel.SetActive(true);
            StartCoroutine(HideMessageAfterDelay(2f));
        }
    }

    IEnumerator DisplayUnlockMessage()
    {
        // Show unlock message
        if (unlockMessagePanel != null && unlockMessageText != null)
        {
            unlockMessageText.text = "DOOR UNLOCKED!";
            unlockMessageText.color = Color.white;
            unlockMessagePanel.SetActive(true);
        }

        // Wait
        yield return new WaitForSeconds(messageDuration);

        // Hide message
        if (unlockMessagePanel != null)
        {
            unlockMessagePanel.SetActive(false);
        }

        // Show arrow pointing to door (permanent until player reaches)
        if (arrowIndicator != null)
        {
            showingArrow = true;
            arrowIndicator.SetActive(true);
            // No timer! Arrow stays until player reaches door
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

    void HideArrow()
    {
        if (arrowIndicator != null)
        {
            arrowIndicator.SetActive(false);
            showingArrow = false;
        }
    }

    void UpdateArrowPosition()
    {
        if (mainCamera == null || arrowTransform == null || doorTransform == null)
            return;

        // Get door screen position
        Vector3 doorScreenPos = mainCamera.WorldToScreenPoint(doorTransform.position);

        // If door is behind camera, flip the position
        bool isBehind = doorScreenPos.z < 0;
        
        if (isBehind)
        {
            doorScreenPos.x = Screen.width - doorScreenPos.x;
            doorScreenPos.y = Screen.height - doorScreenPos.y;
            doorScreenPos.z = -doorScreenPos.z;
        }

        // Clamp to screen edges with margin
        float margin = 80f;
        bool isOffscreen = false;

        if (doorScreenPos.x < margin || doorScreenPos.x > Screen.width - margin ||
            doorScreenPos.y < margin || doorScreenPos.y > Screen.height - margin ||
            isBehind)
        {
            isOffscreen = true;
        }

        if (isOffscreen)
        {
            // Position arrow at screen edge
            doorScreenPos.x = Mathf.Clamp(doorScreenPos.x, margin, Screen.width - margin);
            doorScreenPos.y = Mathf.Clamp(doorScreenPos.y, margin, Screen.height - margin);
        }

        // Update arrow position
        arrowTransform.position = doorScreenPos;

        // Calculate rotation to point towards door
        Vector3 playerPos = mainCamera.transform.position;
        Vector3 doorPos = doorTransform.position;
        
        // Get direction in screen space
        Vector3 screenDir = doorScreenPos - new Vector3(Screen.width / 2, Screen.height / 2, 0);
        
        // Calculate angle
        float angle = Mathf.Atan2(screenDir.y, screenDir.x) * Mathf.Rad2Deg;
        
        // Apply rotation (arrow sprite should point right by default)
        arrowTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
