using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    [Header("Visual Feedback")]
    public Light doorLight;
    public ParticleSystem doorEffect;

    void Start()
    {
        if (doorLight != null)
        {
            doorLight.enabled = true;
        }

        if (doorEffect != null)
        {
            doorEffect.Play();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadNextLevel();
            }
        }
    }
}