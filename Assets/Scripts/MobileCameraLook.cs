using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MobileCameraLook : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;             
    public RigidbodyFirstPersonController fpsController;
    
    [Header("Sensitivity")]
    public float pcSensitivity = 2f;
    public float touchSensitivity = 1.2f;  // Slightly increased for smoothness
    
    [Header("Rotation Limits")]
    public float minVerticalAngle = -80f;    
    public float maxVerticalAngle = 80f;
    
    [Header("Touch Area")]
    [Range(0f, 1f)]
    public float joystickAreaWidth = 0.35f;  // 35% left = joystick area

    private float xRotation = 0f;
    private Vector2 lastTouchPosition;
    private int activeTouchId = -1;
    private bool isMobile = false;

    void Start()
    {
        if (playerBody == null)
        {
            Debug.LogError("❌ Player Body not assigned!");
        }

        #if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
            Input.multiTouchEnabled = true;
            Debug.Log("📱 Mobile multi-touch enabled");
        #endif

        if (!isMobile && fpsController != null)
        {
            fpsController.mouseLook.XSensitivity = pcSensitivity;
            fpsController.mouseLook.YSensitivity = pcSensitivity;
            this.enabled = false;
            Debug.Log("🖱️ Using original MouseLook for PC");
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
        float mouseX = 0f;
        float mouseY = 0f;
        bool foundValidTouch = false;

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
                        foundValidTouch = true;
                        Debug.Log($"📷 Camera touch started: {activeTouchId}");
                    }
                    // Our active touch is moving
                    else if (touch.fingerId == activeTouchId && touch.phase == TouchPhase.Moved)
                    {
                        Vector2 delta = touch.position - lastTouchPosition;
                        
                        // Calculate rotation with smoothing
                        mouseX = delta.x * touchSensitivity * 0.1f;
                        mouseY = delta.y * touchSensitivity * 0.1f;
                        
                        lastTouchPosition = touch.position;
                        foundValidTouch = true;
                    }
                    // Touch ended
                    else if (touch.fingerId == activeTouchId && 
                            (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
                    {
                        activeTouchId = -1;
                        Debug.Log("📷 Camera touch ended");
                    }
                }
            }
        }

        // Apply rotation smoothly
        if (playerBody != null && foundValidTouch && (Mathf.Abs(mouseX) > 0.001f || Mathf.Abs(mouseY) > 0.001f))
        {
            // Vertical rotation (camera up/down)
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            
            // Horizontal rotation (player turn)
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}