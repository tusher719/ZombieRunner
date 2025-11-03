using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MobileCameraLook : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;             
    public RigidbodyFirstPersonController fpsController;  // ✅ Original controller
    
    [Header("Sensitivity")]
    public float pcSensitivity = 2f;         // PC mouse sensitivity
    public float touchSensitivity = 1f;      // Mobile touch sensitivity
    
    [Header("Rotation Limits")]
    public float minVerticalAngle = -80f;    
    public float maxVerticalAngle = 80f;     

    // Private variables
    private float xRotation = 0f;
    private Vector2 lastTouchPosition;
    private bool isTouching = false;
    private int activeTouchId = -1;
    private bool isMobile = false;

    void Start()
    {
        if (playerBody == null)
        {
            Debug.LogError("❌ Player Body not assigned!");
        }

        // Check if mobile platform
        #if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
        #endif

        // PC এ original MouseLook ব্যবহার করুন
        if (!isMobile && fpsController != null)
        {
            fpsController.mouseLook.XSensitivity = pcSensitivity;
            fpsController.mouseLook.YSensitivity = pcSensitivity;
            this.enabled = false;  // এই script disable করুন
            Debug.Log("🖱️ Using original MouseLook for PC");
        }
    }

    void Update()
    {
        // শুধু mobile এ এই script কাজ করবে
        if (isMobile)
        {
            HandleMobileCameraRotation();
        }
    }

    void HandleMobileCameraRotation()
    {
        float mouseX = 0f;
        float mouseY = 0f;

        // ========== MOBILE: TOUCH INPUT ==========
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                // ✅ Screen এর ডান দিকে (40% এর বেশি) touch নিন
                bool isRightSide = touch.position.x > Screen.width * 0.4f;

                if (!isRightSide) continue;

                if (touch.phase == TouchPhase.Began)
                {
                    lastTouchPosition = touch.position;
                    isTouching = true;
                    activeTouchId = touch.fingerId;
                    Debug.Log($"👆 Touch began at: {touch.position}");
                }
                else if (touch.phase == TouchPhase.Moved && isTouching && touch.fingerId == activeTouchId)
                {
                    Vector2 delta = touch.position - lastTouchPosition;
                    
                    mouseX = delta.x * touchSensitivity * 0.1f;
                    mouseY = delta.y * touchSensitivity * 0.1f;

                    lastTouchPosition = touch.position;
                    
                    Debug.Log($"📱 Camera moving - X: {mouseX:F2}, Y: {mouseY:F2}");
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (touch.fingerId == activeTouchId)
                    {
                        isTouching = false;
                        activeTouchId = -1;
                        Debug.Log("👋 Touch ended");
                    }
                }
            }
        }

        // ========== APPLY ROTATION ==========
        if (playerBody != null && (mouseX != 0 || mouseY != 0))
        {
            // Vertical rotation (Camera up/down)
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            
            // Horizontal rotation (Player left/right)
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}