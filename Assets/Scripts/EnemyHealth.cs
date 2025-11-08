using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitPoints = 100f;
    bool isDead = false;

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

        if (hitPoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        GetComponent<Animator>().SetTrigger("die");

        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ZombieKilled();
        }

        // Destroy after animation (adjust time based on your animation length)
        Destroy(gameObject, 3f);
    }
}