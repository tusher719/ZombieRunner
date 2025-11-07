using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MobileCameraLook : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;             
    public RigidbodyFirstPersonController fpsController;
    
    [Header("Sensitivity")]
    public float pcSensitivity = 2f;
    public float touchSensitivity = 0.8f;  // Reduced for smoother control
    
    [Header("Smoothing")]
    public bool enableSmoothing = true;
    public float smoothSpeed = 10f;  // Higher = more responsive, lower = smoother
    
    [Header("Rotation Limits")]
    public float minVerticalAngle = -80f;    
    public float maxVerticalAngle = 80f;
    
    [Header("Touch Area")]
    [Range(0f, 1f)]
    public float joystickAreaWidth = 0.35f;

    private float xRotation = 0f;
    private Vector2 lastTouchPosition;
    private int activeTouchId = -1;
    private bool isMobile = false;
    
    // Smoothing variables
    private Vector2 currentRotationVelocity;
    private Vector2 targetRotation;

    void Start()
    {
        if (playerBody == null)
        {
            Debug.LogError("Player Body not assigned!");
        }

        #if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
            Input.multiTouchEnabled = true;
        #endif

        if (!isMobile && fpsController != null)
        {
            fpsController.mouseLook.XSensitivity = pcSensitivity;
            fpsController.mouseLook.YSensitivity = pcSensitivity;
            this.enabled = false;
        }
    }

    void Update()
    {
        if (isMobile)
        {
            HandleMobileCameraRotation();
        }
    }

    void HandleMobileCameraRotation()
    {
        float deltaX = 0f;
        float deltaY = 0f;
        bool isTouching = false;

        if (Input.touchCount > 0)
        {
            // Process all touches to find camera touch
            foreach (Touch touch in Input.touches)
            {
                // Calculate if touch is on right side
                float touchXNormalized = touch.position.x / Screen.width;
                bool isRightSide = touchXNormalized > joystickAreaWidth;
                
                // Only process right side touches
                if (isRightSide)
                {
                    // New touch began
                    if (touch.phase == TouchPhase.Began && activeTouchId == -1)
                    {
                        activeTouchId = touch.fingerId;
                        lastTouchPosition = touch.position;
                        currentRotationVelocity = Vector2.zero;  // Reset velocity
                        isTouching = true;
                    }
                    // Active touch is moving
                    else if (touch.fingerId == activeTouchId && touch.phase == TouchPhase.Moved)
                    {
                        Vector2 delta = touch.position - lastTouchPosition;
                        
                        // Calculate rotation delta with reduced sensitivity
                        deltaX = delta.x * touchSensitivity;
                        deltaY = delta.y * touchSensitivity;
                        
                        lastTouchPosition = touch.position;
                        isTouching = true;
                    }
                    // Touch ended
                    else if (touch.fingerId == activeTouchId && 
                            (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
                    {
                        activeTouchId = -1;
                        currentRotationVelocity = Vector2.zero;  // Reset on release
                    }
                }
            }
        }

        // Apply rotation with smoothing
        if (playerBody != null && isTouching)
        {
            if (enableSmoothing)
            {
                // Smooth rotation using lerp
                targetRotation = new Vector2(deltaX, deltaY);
                currentRotationVelocity = Vector2.Lerp(
                    currentRotationVelocity, 
                    targetRotation, 
                    smoothSpeed * Time.deltaTime
                );
                
                float smoothMouseX = currentRotationVelocity.x;
                float smoothMouseY = currentRotationVelocity.y;
                
                // Apply smoothed rotation
                xRotation -= smoothMouseY;
                xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                playerBody.Rotate(Vector3.up * smoothMouseX);
            }
            else
            {
                // Direct rotation without smoothing
                xRotation -= deltaY;
                xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
                transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                playerBody.Rotate(Vector3.up * deltaX);
            }
        }
        else if (!isTouching)
        {
            // Gradually stop rotation when not touching
            currentRotationVelocity = Vector2.Lerp(currentRotationVelocity, Vector2.zero, smoothSpeed * Time.deltaTime);
        }
    }
}