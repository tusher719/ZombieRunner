using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public Image healthBarFill;
    public TextMeshProUGUI healthText;
    
    [Header("Colors")]
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    [Range(0, 100)]
    public float lowHealthThreshold = 30f;
    
    [Header("Animation")]
    public bool useSmoothAnimation = true;
    public float smoothSpeed = 5f;
    
    private float targetFillAmount;
    private float maxHealth;

    void Start()
    {
        // Find PlayerHealth if not assigned
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            Debug.LogError("❌ PlayerHealth not found!");
            return;
        }

        // Get initial max health from PlayerHealth current value
        maxHealth = playerHealth.GetHealth();
        targetFillAmount = 1f;
        
        Debug.Log($"✅ Health Display initialized - Max Health: {maxHealth}");
        
        UpdateHealthDisplay();
    }

    void Update()
    {
        UpdateHealthDisplay();
    }

    void UpdateHealthDisplay()
    {
        if (playerHealth == null) return;

        // Get current health
        float currentHealth = playerHealth.GetHealth();
        
        // Ensure max health is set
        if (maxHealth <= 0)
        {
            maxHealth = currentHealth;
        }

        // Calculate fill amount (0 to 1)
        targetFillAmount = Mathf.Clamp01(currentHealth / maxHealth);

        // Update health bar fill
        if (healthBarFill != null)
        {
            if (useSmoothAnimation)
            {
                healthBarFill.fillAmount = Mathf.Lerp(
                    healthBarFill.fillAmount,
                    targetFillAmount,
                    smoothSpeed * Time.deltaTime
                );
            }
            else
            {
                healthBarFill.fillAmount = targetFillAmount;
            }

            // Change color based on health percentage
            float healthPercent = (currentHealth / maxHealth) * 100f;
            
            if (healthPercent <= lowHealthThreshold)
            {
                healthBarFill.color = lowHealthColor;
            }
            else
            {
                healthBarFill.color = fullHealthColor;
            }
        }

        // Update text
        if (healthText != null)
        {
            int displayHealth = Mathf.RoundToInt(currentHealth);
            healthText.text = $"Health: {displayHealth}";
        }
    }
}