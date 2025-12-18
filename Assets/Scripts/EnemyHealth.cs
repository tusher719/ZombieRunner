using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 100f;
    [SerializeField] private EnemyHealthbar _healthbar;
    [SerializeField] private GameObject deathParticle;

    private float maxHealth;
    bool isDead = false;

    void Start()
    {
        maxHealth = hitPoints;
        _healthbar.UpdateHealthBar(maxHealth, hitPoints);
        
        if (deathParticle != null)
        {
            deathParticle.SetActive(false);
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        BroadcastMessage("OnDamageTaken");
        GetComponent<EnemyAI>().OnDamageTaken();
        
        hitPoints -= damage;
        hitPoints = Mathf.Max(hitPoints, 0);

        if (hitPoints <= 0)
        {
            Die();
        } 
        else
        {
            _healthbar.UpdateHealthBar(maxHealth, hitPoints);
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        
        _healthbar.UpdateHealthBar(maxHealth, 0);
        _healthbar.HideHealthBar();

        if (deathParticle != null)
        {
            deathParticle.SetActive(true);
            ParticleSystem ps = deathParticle.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }

        GetComponent<Animator>().SetTrigger("die");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ZombieKilled();
        }

        Destroy(gameObject, 1f);
    }
}