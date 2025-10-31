using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MobileJumpButton : MonoBehaviour
{
    private RigidbodyFirstPersonController playerController;

    void Start()
    {
        playerController = FindObjectOfType<RigidbodyFirstPersonController>();
    }

    // UI Button থেকে এই method call করবেন
    public void OnJumpPressed()
    {
        if (playerController != null && playerController.Grounded)
        {
            // Jump করো
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(new Vector3(0f, playerController.movementSettings.JumpForce, 0f), ForceMode.Impulse);
        }
    }
}