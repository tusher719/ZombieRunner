using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MobileCameraLook : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;             
    public RigidbodyFirstPersonController fpsController;
    
    [Header("Sensitivity")]
    public float pcSensitivity = 2f;
    public float touchSensitivity = 1f;
    
    [Header("Rotation Limits")]
    public float minVerticalAngle = -80f;    
    public float maxVerticalAngle = 80f;

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

        #if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
            // ✅ Enable multi-touch
            Input.multiTouchEnabled = true;
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

        // ✅✅✅ FIXED: Check all touches, find right side touch
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                // ✅ Screen এর ডান দিকে (40% এর বেশি) touch check
                bool isRightSide = touch.position.x > Screen.width * 0.4f;

                if (!isRightSide) continue; // Left side ignore করুন

                // ✅ Began phase
                if (touch.phase == TouchPhase.Began)
                {
                    // যদি already active touch না থাকে তাহলে এটা নিন
                    if (activeTouchId == -1)
                    {
                        lastTouchPosition = touch.position;
                        isTouching = true;
                        activeTouchId = touch.fingerId;
                        Debug.Log($"👆 Camera touch started: ID {activeTouchId}");
                    }
                }
                // ✅ Moved phase
                else if (touch.phase == TouchPhase.Moved && touch.fingerId == activeTouchId)
                {
                    Vector2 delta = touch.position - lastTouchPosition;
                    
                    mouseX = delta.x * touchSensitivity * 0.1f;
                    mouseY = delta.y * touchSensitivity * 0.1f;

                    lastTouchPosition = touch.position;
                }
                // ✅ Ended phase
                else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) 
                         && touch.fingerId == activeTouchId)
                {
                    isTouching = false;
                    activeTouchId = -1;
                    Debug.Log("👋 Camera touch ended");
                    break; // Exit loop
                }
            }
        }

        // ✅ Apply rotation
        if (playerBody != null && (mouseX != 0 || mouseY != 0))
        {
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}