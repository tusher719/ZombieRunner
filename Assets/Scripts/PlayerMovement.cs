using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mobile Joystick Reference")]
    public Joystick joystick;  
    
    [Header("Movement Settings")]
    public float walkSpeed = 8f;              // ✅ Speed বাড়ালাম
    public float runSpeed = 12f;              
    public float jumpForce = 8f;       
    public float gravity = -20f;       
    public float rotationSpeed = 10f;         // ✅ Rotation smooth করার জন্য
    public float acceleration = 10f;          // ✅ Acceleration speed
    
    [Header("Ground Check")]
    public Transform groundCheck;      
    public float groundDistance = 0.4f;
    public LayerMask groundMask;       
    
    // Private variables
    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentVelocity;          // ✅ Current movement velocity
    private bool isGrounded;
    private bool isRunning = false;
    
    void Start()
    {
        // CharacterController setup
        controller = GetComponent<CharacterController>();
        
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0, 0, 0);
        }

        // Joystick check
        if (joystick != null)
        {
            Debug.Log("✅ Joystick connected!");
        }
        else
        {
            Debug.LogWarning("⚠️ Joystick not assigned!");
        }

        // Ground check setup
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.parent = transform;
            groundCheckObj.transform.localPosition = new Vector3(0, 0, 0);
            groundCheck = groundCheckObj.transform;
        }
    }

    void Update()
    {
        // ========== GROUND CHECK ==========
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // ========== GET INPUT ==========
        float horizontal = 0f;
        float vertical = 0f;

        // Mobile: Joystick input
        if (joystick != null)
        {
            horizontal = joystick.Horizontal;
            vertical = joystick.Vertical;
        }
        else
        {
            // PC: Keyboard input
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }

        // ========== MOVEMENT CALCULATION ==========
        // Camera-relative direction
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        
        // Normalize to prevent faster diagonal movement
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Current speed
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // ✅ Smooth acceleration using Lerp
        Vector3 targetVelocity = moveDirection * currentSpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        
        // Move character
        controller.Move(currentVelocity * Time.deltaTime);

        // ========== JUMP ==========
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // ========== GRAVITY ==========
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ========== PUBLIC METHODS ==========
    
    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            Debug.Log("🔼 Jump!");
        }
    }

    public void SetRunning(bool running)
    {
        isRunning = running;
    }

    // ========== DEBUG ==========
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}