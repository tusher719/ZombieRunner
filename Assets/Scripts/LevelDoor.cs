using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelDoor : MonoBehaviour
{
    [Header("Door States")]
    public bool isLocked = true;

    [Header("Visual Elements")]
    public Renderer doorRenderer;
    public Material lockedMaterial;
    public Material unlockedMaterial;
    public ParticleSystem unlockEffect;
    public Light doorLight;

    [Header("Lock Icon")]
    public GameObject lockIcon;  // 3D lock model or sprite

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip unlockSound;

    void Start()
    {
        // Set initial locked state
        SetLockedState(true);
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

        // Change material
        if (doorRenderer != null)
        {
            doorRenderer.material = locked ? lockedMaterial : unlockedMaterial;
        }

        // Change light color
        if (doorLight != null)
        {
            doorLight.color = locked ? Color.red : Color.green;
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
}