using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MobileJumpButton : MonoBehaviour
{
    private RigidbodyFirstPersonController fpsController;
    private Rigidbody playerRigidbody;
    
    [Header("Settings")]
    public float jumpForce = 30f;  // Default FPS controller jump force

    void Start()
    {
        // Find FPS Controller
        fpsController = FindObjectOfType<RigidbodyFirstPersonController>();
        
        if (fpsController != null)
        {
            playerRigidbody = fpsController.GetComponent<Rigidbody>();
            jumpForce = fpsController.movementSettings.JumpForce;  // Use controller's jump force
            Debug.Log($"✅ Jump button connected! Jump Force: {jumpForce}");
        }
        else
        {
            Debug.LogError("❌ RigidbodyFirstPersonController not found!");
        }
    }

    // Called from UI button
    public void OnJumpPressed()
    {
        // Check if grounded using FPS controller
        if (fpsController != null && fpsController.Grounded && playerRigidbody != null)
        {
            // Reset vertical velocity
            playerRigidbody.velocity = new Vector3(
                playerRigidbody.velocity.x, 
                0f, 
                playerRigidbody.velocity.z
            );
            
            // Apply jump force
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            Debug.Log("🔼 Jump! Force: " + jumpForce);
        }
        else
        {
            Debug.Log("⚠️ Cannot jump - not grounded or controller missing");
        }
    }
}