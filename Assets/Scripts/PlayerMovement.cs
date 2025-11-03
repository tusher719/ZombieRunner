using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mobile Joystick Reference")]
    public Joystick joystick;  // ✅ Direct Joystick reference (not GameObject)
    
    [Header("Movement Settings")]
    public float walkSpeed = 6f;       
    public float runSpeed = 10f;       
    public float jumpForce = 8f;       
    public float gravity = -20f;       
    public float smoothTime = 0.1f;    
    
    [Header("Ground Check")]
    public Transform groundCheck;      
    public float groundDistance = 0.4f;
    public LayerMask groundMask;       
    
    // Private variables
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning = false;
    private Vector3 smoothMoveVelocity;
    
    void Start()
    {
        // CharacterController component নিন
        controller = GetComponent<CharacterController>();
        
        // যদি CharacterController না থাকে তাহলে add করুন
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0, 1, 0);
        }

        // Joystick check করুন
        if (joystick != null)
        {
            Debug.Log("✅ Joystick connected!");
        }
        else
        {
            Debug.LogWarning("⚠️ Joystick is not assigned! Drag Fixed Joystick to PlayerMovement script.");
        }

        // Ground check object না থাকলে তৈরি করুন
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

        // ✅ Mobile: Direct joystick access
        if (joystick != null)
        {
            horizontal = joystick.Horizontal;
            vertical = joystick.Vertical;
            
            // Debug log
            if (horizontal != 0 || vertical != 0)
            {
                Debug.Log($"✅ Joystick Input - H: {horizontal:F2}, V: {vertical:F2}");
            }
        }
        else
        {
            // PC: Keyboard input
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }

        // ========== MOVEMENT CALCULATION ==========
        // Camera-relative movement direction
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        
        // Normalize করুন
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Current speed
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Smooth movement
        Vector3 targetVelocity = moveDirection * currentSpeed;
        Vector3 smoothMove = Vector3.SmoothDamp(controller.velocity, targetVelocity, ref smoothMoveVelocity, smoothTime);
        
        // Move character
        controller.Move(smoothMove * Time.deltaTime);

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

    // ========== DEBUG VISUALIZATION ==========
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}