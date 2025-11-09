using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [Header("Crosshair References")]
    [SerializeField] private Image crosshairImage;
    
    [Header("Size Settings")]
    [SerializeField] private float normalSize = 50f;
    [SerializeField] private float shootSize = 70f;
    [SerializeField] private float enemyDetectedSize = 65f;
    [SerializeField] private float sizeAnimationSpeed = 15f;
    
    [Header("Color Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color enemyColor = Color.red;
    [SerializeField] private float colorChangeSpeed = 10f;
    
    [Header("Enemy Detection")]
    [SerializeField] private Camera playerCamera;
    
    private RectTransform rectTransform;
    private float targetSize;
    private Color targetColor;
    private float currentWeaponRange = 100f;
    private bool isShooting = false;
    private float shootTimer = 0f;
    private float shootResetTime = 0.15f;

    void Start()
    {
        // Get components
        rectTransform = GetComponent<RectTransform>();
        
        // Auto-find if not assigned
        if (crosshairImage == null)
        {
            crosshairImage = GetComponent<Image>();
        }
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        // Set initial values
        targetSize = normalSize;
        targetColor = normalColor;
        
        // Set initial size immediately
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(normalSize, normalSize);
        }
    }

    void Update()
    {
        // Handle shooting timer
        if (isShooting)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                isShooting = false;
            }
        }
        
        // Check for enemy detection first (priority)
        bool enemyDetected = CheckEnemyDetection();
        
        // Set target size based on state
        if (isShooting)
        {
            targetSize = shootSize;
        }
        else if (enemyDetected)
        {
            targetSize = enemyDetectedSize;
        }
        else
        {
            targetSize = normalSize;
        }
        
        // Animate size and color
        AnimateSize();
        AnimateColor();
    }

    // Smooth size animation
    void AnimateSize()
    {
        if (rectTransform == null) return;
        
        float currentSize = rectTransform.sizeDelta.x;
        float newSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * sizeAnimationSpeed);
        rectTransform.sizeDelta = new Vector2(newSize, newSize);
    }

    // Smooth color animation
    void AnimateColor()
    {
        if (crosshairImage == null) return;
        
        crosshairImage.color = Color.Lerp(crosshairImage.color, targetColor, Time.deltaTime * colorChangeSpeed);
    }

    // Check if aiming at enemy (returns true if enemy detected)
    bool CheckEnemyDetection()
    {
        if (playerCamera == null) return false;
        
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        // Use weapon range for detection
        if (Physics.Raycast(ray, out hit, currentWeaponRange))
        {
            // Check if hit object has EnemyHealth component
            if (hit.collider.GetComponent<EnemyHealth>() != null)
            {
                targetColor = enemyColor;
                return true;
            }
        }
        
        // No enemy detected
        targetColor = normalColor;
        return false;
    }

    // Called from weapon script when shooting (instant response)
    public void OnShoot()
    {
        isShooting = true;
        shootTimer = shootResetTime;
        targetSize = shootSize;
    }

    // Called from weapon script to update range dynamically
    public void SetWeaponRange(float range)
    {
        currentWeaponRange = range;
    }

    // Show/hide crosshair
    public void SetActive(bool active)
    {
        if (crosshairImage != null)
        {
            crosshairImage.enabled = active;
        }
    }
}