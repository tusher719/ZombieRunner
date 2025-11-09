// EnemyHealthbar.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // ✅ Added this

public class EnemyHealthbar : MonoBehaviour
{
    [SerializeField] private Image _healthBarSprite;
    [SerializeField] private float _reduceSpeed = 2;

    private float _target = 1;
    private Camera _cam;
    private Canvas _canvas;

    void Start()
    {
        _cam = Camera.main;
        
        // Get canvas component for visibility control
        _canvas = GetComponent<Canvas>();
        
        // Auto-find healthbar image if not assigned
        if (_healthBarSprite == null)
        {
            Transform foreground = transform.Find("Foreground");
            if (foreground != null)
            {
                _healthBarSprite = foreground.GetComponent<Image>();
            }
        }
    }

    // Update healthbar fill amount based on current health
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        // Ensure health value stays within valid range
        currentHealth = Mathf.Max(currentHealth, 0);
        _target = Mathf.Clamp01(currentHealth / maxHealth);
    }

    // Hide the healthbar canvas when enemy dies
    public void HideHealthBar()
    {
        if (_canvas != null)
        {
            _canvas.enabled = false;
        }
        else
        {
            // Fallback to disabling entire GameObject
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Billboard effect - healthbar always faces camera
        transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position);
        
        // Smoothly animate healthbar fill amount
        if (_healthBarSprite != null)
        {
            _healthBarSprite.fillAmount = Mathf.MoveTowards(_healthBarSprite.fillAmount, _target, _reduceSpeed * Time.deltaTime);
        }
    }
}