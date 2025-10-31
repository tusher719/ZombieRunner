using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mobile Joystick")]
    public GameObject joystickObject;  // Fixed Joystick drag করবেন
    
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    private MonoBehaviour joystickScript;
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning = false;

    void Start()
    {
        // CharacterController get করুন
        controller = GetComponent<CharacterController>();
        
        // যদি না থাকে তাহলে add করুন
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0, 1, 0);
        }

        // Joystick script get করুন
        if (joystickObject != null)
        {
            joystickScript = joystickObject.GetComponent<MonoBehaviour>();
        }

        // Ground check না থাকলে তৈরি করুন
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

        // Mobile: Joystick input
        if (joystickScript != null)
        {
            var horizontalProp = joystickScript.GetType().GetProperty("Horizontal");
            var verticalProp = joystickScript.GetType().GetProperty("Vertical");

            if (horizontalProp != null && verticalProp != null)
            {
                horizontal = (float)horizontalProp.GetValue(joystickScript);
                vertical = (float)verticalProp.GetValue(joystickScript);
            }
        }
        else
        {
            // PC: Keyboard input
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }

        // Movement direction (camera-relative)
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // Current speed
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Move character
        controller.Move(move * currentSpeed * Time.deltaTime);

        // PC Jump (Space key)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Mobile Jump button এর জন্য
    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    // Run toggle (optional - run button এর জন্য)
    public void SetRunning(bool running)
    {
        isRunning = running;
    }
}