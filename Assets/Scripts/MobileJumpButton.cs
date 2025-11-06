using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MobileJumpButton : MonoBehaviour
{
    private RigidbodyFirstPersonController fpsController;
    private Rigidbody playerRigidbody;
    private float jumpForce = 50f;  // Default from your FPS controller
    
    [Header("Ground Check")]
    public LayerMask groundLayer;  // Ground layer set করতে হবে Inspector থেকে
    public float groundCheckDistance = 0.2f;

    void Start()
    {
        // Find FPS Controller
        fpsController = FindObjectOfType<RigidbodyFirstPersonController>();
        
        if (fpsController != null)
        {
            playerRigidbody = fpsController.GetComponent<Rigidbody>();
            jumpForce = fpsController.movementSettings.JumpForce;
            Debug.Log($"✅ Jump button connected! Jump Force: {jumpForce}");
        }
        else
        {
            Debug.LogError("❌ RigidbodyFirstPersonController not found!");
        }
        
        // Check ground layer setup
        if (groundLayer == 0)
        {
            Debug.LogWarning("⚠️ Ground Layer not set! Set it in Inspector");
        }
    }

    // Called from UI Button
    public void OnJumpPressed()
    {
        Debug.Log("🔴 JUMP BUTTON PRESSED!");

        if (playerRigidbody == null)
        {
            Debug.LogError("❌ Rigidbody missing!");
            return;
        }

        // Simple jump - works every time button pressed
        Vector3 velocity = playerRigidbody.velocity;
        
        // Only jump if not moving up fast
        if (velocity.y < 1f)
        {
            velocity.y = 0f;
            playerRigidbody.velocity = velocity;
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log($"🔼 JUMP! Force: {jumpForce}");
    }
    }

    // Ground check using raycast
    private bool CheckGrounded()
    {
        if (fpsController == null) return false;

        Vector3 position = fpsController.transform.position;
        
        // Raycast down from player position
        bool hit = Physics.Raycast(
            position, 
            Vector3.down, 
            groundCheckDistance, 
            groundLayer
        );

        return hit;
    }

    // Debug visualization
    void OnDrawGizmos()
    {
        if (fpsController != null)
        {
            Gizmos.color = Color.red;
            Vector3 position = fpsController.transform.position;
            Gizmos.DrawLine(position, position + Vector3.down * groundCheckDistance);
        }
    }
}
