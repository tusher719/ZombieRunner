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

    // Instance materials (not shared) - এটা prefab issue solve করবে
    private Material instanceLockedMat;
    private Material instanceUnlockedMat;

    void Awake()
    {
        Debug.Log($"[{gameObject.name}] 🚪 Awake called");
        
        // CRITICAL: Force locked FIRST, before creating materials
        isLocked = true;
        Debug.Log($"[{gameObject.name}] 🔒 Awake: isLocked forced to TRUE");
        
        // Create material instances to avoid prefab sharing issues
        CreateMaterialInstances();
        
        // Apply locked state immediately after materials created
        if (Application.isPlaying)
        {
            SetLockedState(true);
            Debug.Log($"[{gameObject.name}] 🔒 Awake: Applied locked state");
        }
    }

    void Start()
    {
        Debug.Log($"[{gameObject.name}] 🚪 Start called - Level {levelNumber}");
        
        // CRITICAL: FORCE locked state - IGNORE Inspector value!
        isLocked = true;
        
        Debug.Log($"[{gameObject.name}] 🔒 FORCING isLocked = TRUE (ignoring Inspector)");
        
        SetLockedState(true);
        
        Debug.Log($"[{gameObject.name}] 🔒 Door initialized as LOCKED for Level {levelNumber}");
        
        // Debug material assignments
        Debug.Log($"[{gameObject.name}] Door Renderer: {(doorRenderer != null ? "✅ Assigned" : "❌ NULL")}");
        Debug.Log($"[{gameObject.name}] Locked Material: {(lockedMaterial != null ? "✅ Assigned" : "❌ NULL")}");
        Debug.Log($"[{gameObject.name}] Unlocked Material: {(unlockedMaterial != null ? "✅ Assigned" : "❌ NULL")}");
        Debug.Log($"[{gameObject.name}] Door Light: {(doorLight != null ? "✅ Assigned" : "❌ NULL")}");
        Debug.Log($"[{gameObject.name}] Lock Icon: {(lockIcon != null ? "✅ Assigned" : "❌ NULL")}");
    }

    void OnEnable()
    {
        // CRITICAL: Also force locked when enabled - IGNORE Inspector!
        if (Application.isPlaying)
        {
            isLocked = true;
            SetLockedState(true);
            Debug.Log($"[{gameObject.name}] 🔒 OnEnable: Door FORCED to locked state");
        }
    }

    // 🎨 Create material instances to prevent prefab sharing issues
    void CreateMaterialInstances()
    {
        if (doorRenderer != null)
        {
            // Create instance copies of materials
            if (lockedMaterial != null)
            {
                instanceLockedMat = new Material(lockedMaterial);
                Debug.Log($"[{gameObject.name}] ✅ Created instance of locked material");
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] ❌ Locked material is NULL!");
            }
            
            if (unlockedMaterial != null)
            {
                instanceUnlockedMat = new Material(unlockedMaterial);
                Debug.Log($"[{gameObject.name}] ✅ Created instance of unlocked material");
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] ❌ Unlocked material is NULL!");
            }
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] ❌ Door Renderer is NULL!");
        }
    }

    public void UnlockDoor()
    {
        Debug.Log($"[{gameObject.name}] 🔓 UnlockDoor() called! Current state: {(isLocked ? "LOCKED" : "UNLOCKED")}");
        
        if (isLocked)
        {
            Debug.Log($"[{gameObject.name}] 🎉 Unlocking door for Level {levelNumber}!");
            
            isLocked = false;
            SetLockedState(false);

            // Play unlock effects
            if (unlockEffect != null)
            {
                unlockEffect.Play();
                Debug.Log($"[{gameObject.name}] ✨ Particle effect played");
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] ⚠️ No particle effect assigned");
            }

            if (audioSource != null && unlockSound != null)
            {
                audioSource.PlayOneShot(unlockSound);
                Debug.Log($"[{gameObject.name}] 🔊 Unlock sound played");
            }

            // Show UI message
            if (DoorUnlockUI.Instance != null)
            {
                DoorUnlockUI.Instance.ShowUnlockMessage(transform.position);
                Debug.Log($"[{gameObject.name}] 💬 UI message sent");
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] ⚠️ DoorUnlockUI.Instance is NULL");
            }

            // Pulse animation
            StartCoroutine(PulseDoor());
            
            Debug.Log($"[{gameObject.name}] ✅ Door UNLOCKED for Level {levelNumber}");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] ⚠️ UnlockDoor called but door already unlocked!");
        }
    }

    void SetLockedState(bool locked)
    {
        Debug.Log($"[{gameObject.name}] 🎨 SetLockedState({locked}) called");
        
        isLocked = locked;

        // Change material using INSTANCE materials (not shared)
        if (doorRenderer != null)
        {
            if (locked && instanceLockedMat != null)
            {
                doorRenderer.material = instanceLockedMat;
                Debug.Log($"[{gameObject.name}] 🔴 Material changed to LOCKED (RED)");
            }
            else if (!locked && instanceUnlockedMat != null)
            {
                doorRenderer.material = instanceUnlockedMat;
                Debug.Log($"[{gameObject.name}] 🟢 Material changed to UNLOCKED (GREEN)");
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] ❌ Material change failed! locked={locked}, instanceMat={(locked ? instanceLockedMat : instanceUnlockedMat) != null}");
            }
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] ❌ Door Renderer is NULL! Cannot change material");
        }

        // Change light color
        if (doorLight != null)
        {
            doorLight.enabled = true;
            doorLight.color = locked ? Color.red : Color.green;
            doorLight.intensity = locked ? 3 : 5;
            Debug.Log($"[{gameObject.name}] 💡 Light changed to {(locked ? "RED" : "GREEN")}");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] ⚠️ Door Light is NULL!");
        }

        // Show/hide lock icon
        if (lockIcon != null)
        {
            lockIcon.SetActive(locked);
            Debug.Log($"[{gameObject.name}] 🔒 Lock icon {(locked ? "SHOWN" : "HIDDEN")}");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] ⚠️ Lock Icon is NULL!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[{gameObject.name}] 👤 Player touched door (Level {levelNumber}). Locked: {isLocked}");
            
            if (!isLocked)
            {
                Debug.Log($"[{gameObject.name}] ✅ Door unlocked! Loading next level...");
                
                // Door is unlocked, load next level
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.LoadNextLevel();
                }
            }
            else
            {
                Debug.Log($"[{gameObject.name}] 🔒 Door locked! Showing message...");
                
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
        Debug.Log($"[{gameObject.name}] 💫 Starting pulse animation");
        
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
        Debug.Log($"[{gameObject.name}] ✅ Pulse animation complete");
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