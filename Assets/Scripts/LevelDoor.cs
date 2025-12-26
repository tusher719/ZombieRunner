using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelDoor : MonoBehaviour
{
    [Header("Level Settings - SET MANUALLY")]
    [Tooltip("Which level this door belongs to (1, 2, 3, etc.)")]
    public int levelNumber = 1;

    [Header("Door States")]
    public bool isLocked = true;

    [Header("Visual Elements")]
    public Renderer doorRenderer;
    public Material lockedMaterial;
    public Material unlockedMaterial;
    public ParticleSystem unlockEffect;
    public Light doorLight;

    [Header("Lock Icon")]
    public GameObject lockIcon;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip unlockSound;

    // Instance materials (not shared)
    private Material instanceLockedMat;
    private Material instanceUnlockedMat;

    void Awake()
    {        
        // CRITICAL: Force locked FIRST, before creating materials
        isLocked = true;
        
        // Create material instances to avoid prefab sharing issues
        CreateMaterialInstances();
        
        // Apply locked state immediately after materials created
        if (Application.isPlaying)
        {
            SetLockedState(true);
        }
    }

    void Start()
    {        
        // CRITICAL: FORCE locked state - IGNORE Inspector value!
        isLocked = true;
        
        SetLockedState(true);
    }

    void OnEnable()
    {
        // CRITICAL: Also force locked when enabled - IGNORE Inspector!
        if (Application.isPlaying)
        {
            isLocked = true;
            SetLockedState(true);
        }
    }

    // Create material instances to prevent prefab sharing issues
    void CreateMaterialInstances()
    {
        if (doorRenderer != null)
        {
            // Create instance copies of materials
            if (lockedMaterial != null)
            {
                instanceLockedMat = new Material(lockedMaterial);
            }
            
            if (unlockedMaterial != null)
            {
                instanceUnlockedMat = new Material(unlockedMaterial);
            }
        }
    }

    public void UnlockDoor()
    {        
        if (isLocked)
        {
            
            isLocked = false;
            SetLockedState(false);

            // Play unlock effects
            if (unlockEffect != null)
            {
                unlockEffect.Play();
            }

            if (audioSource != null && unlockSound != null)
            {
                audioSource.PlayOneShot(unlockSound);
            }

            // Show UI message
            if (DoorUnlockUI.Instance != null)
            {
                DoorUnlockUI.Instance.ShowUnlockMessage(transform.position);
            }

            // Pulse animation
            StartCoroutine(PulseDoor());
        }
    }

    void SetLockedState(bool locked)
    {
        
        isLocked = locked;

        // Change material using INSTANCE materials (not shared)
        if (doorRenderer != null)
        {
            if (locked && instanceLockedMat != null)
            {
                doorRenderer.material = instanceLockedMat;
            }
            else if (!locked && instanceUnlockedMat != null)
            {
                doorRenderer.material = instanceUnlockedMat;
            }
        }

        // Change light color
        if (doorLight != null)
        {
            doorLight.enabled = true;
            doorLight.color = locked ? Color.red : Color.green;
            doorLight.intensity = locked ? 3 : 5;
        }

        // Show/hide lock icon
        if (lockIcon != null)
        {
            lockIcon.SetActive(locked);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            if (!isLocked)
            {                
                // Door is unlocked, load next level
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.LoadNextLevel();
                }
            }
            else
            {                
                // Door is still locked - show message
                if (DoorUnlockUI.Instance != null)
                {
                    DoorUnlockUI.Instance.ShowLockedMessage();
                }
            }
        }
    }

    IEnumerator PulseDoor()
    {
        
        Vector3 originalScale = transform.localScale;
        float pulseAmount = 0.1f;
        float pulseDuration = 0.5f;

        for (int i = 0; i < 3; i++)
        {
            // Pulse up
            float elapsed = 0;
            while (elapsed < pulseDuration)
            {
                elapsed += Time.deltaTime;
                float scale = 1 + (pulseAmount * (elapsed / pulseDuration));
                transform.localScale = originalScale * scale;
                yield return null;
            }

            // Pulse down
            elapsed = 0;
            while (elapsed < pulseDuration)
            {
                elapsed += Time.deltaTime;
                float scale = 1 + (pulseAmount * (1 - elapsed / pulseDuration));
                transform.localScale = originalScale * scale;
                yield return null;
            }
        }

        transform.localScale = originalScale;
    }

    // Editor helper - show level number in inspector
    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = isLocked ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 2f);
        
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 3, 
            $"Exit Door - Level {levelNumber}\n{(isLocked ? "LOCKED" : "UNLOCKED")}"
        );
    }
    #endif
}