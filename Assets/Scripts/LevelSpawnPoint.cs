using UnityEngine;

public class LevelSpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings - SET MANUALLY")]
    [Tooltip("Which level uses this spawn point (1, 2, 3, etc.)")]
    public int levelNumber = 1;  // Inspector থেকে manually set করবে

    [Header("Visual Helper (Editor Only)")]
    public Color gizmoColor = Color.green;

    void OnDrawGizmos()
    {
        // Show spawn point in editor
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw arrow showing spawn direction
        Gizmos.DrawRay(transform.position, transform.forward * 2);
        
        // Draw level number
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 2, 
            $"Level {levelNumber} Spawn\n▼ Player spawns here"
        );
        #endif
    }
}