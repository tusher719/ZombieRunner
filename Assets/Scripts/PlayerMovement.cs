using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mobile Joystick Reference")]
    public Joystick joystick;  
    
    [Header("Movement Settings")]
    public float walkSpeed = 8f;
    public float runSpeed = 12f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float acceleration = 10f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentVelocity;
    private bool isGrounded;
    private bool isRunning = false;
    private bool isMobile = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0, 0, 0);
        }

        // Check if mobile platform
        #if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
        #endif

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
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get input
        float horizontal = 0f;
        float vertical = 0f;

        // Mobile: Use joystick if mobile platform AND joystick exists
        if (isMobile && joystick != null)
        {
            horizontal = joystick.Horizontal;
            vertical = joystick.Vertical;
        }
        else
        {
            // PC: Always use keyboard
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }

        // Movement calculation
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 targetVelocity = moveDirection * currentSpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        
        controller.Move(currentVelocity * Time.deltaTime);

        // Jump (PC: Space key)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Mobile jump button method
    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    public void SetRunning(bool running)
    {
        isRunning = running;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}