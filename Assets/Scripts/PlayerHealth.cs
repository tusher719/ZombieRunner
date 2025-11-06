using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 100f;
    [SerializeField] TextMeshProUGUI hitPointsText;

    void Start()
    {
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        hitPoints -= damage;

        UpdateHealthUI();

        if (hitPoints <= 0)
        {
            GetComponent<DeathHandler>().HandleDeath();
        }
    }

    void UpdateHealthUI()
    {
        if (hitPointsText != null)
        {
            hitPointsText.text = Mathf.Max(hitPoints, 0).ToString("0");
        }
    }

    // Methods for HealthDisplay
    public float GetHealth()
    {
        return Mathf.Max(hitPoints, 0);  // Never return negative
    }

    public float GetMaxHealth()
    {
        return 100f;  // Will be updated to match initial hitPoints
    }
}