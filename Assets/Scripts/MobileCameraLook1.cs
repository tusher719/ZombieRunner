using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MobileCameraLook : MonoBehaviour
{
    public Transform playerBody;
    public RigidbodyFirstPersonController fpsController;
    public float sensitivity = 0.3f;

    private float xRotation = 0f;
    private int touchId = -1;

    void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        Input.multiTouchEnabled = true;
#else
        // On PC, disable this script and use the built-in mouse look
        this.enabled = false;
#endif
    }

    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        HandleTouch();
#endif
    }

    void HandleTouch()
    {
        foreach (Touch t in Input.touches)
        {
            // Start tracking new touch on right half of screen
            if (t.phase == TouchPhase.Began && touchId == -1)
            {
                if (t.position.x > Screen.width * 0.4f)
                {
                    touchId = t.fingerId;
                }
            }
            // Track movement
            else if (t.fingerId == touchId && t.phase == TouchPhase.Moved)
            {
                float rotX = t.deltaPosition.x * sensitivity;
                float rotY = t.deltaPosition.y * sensitivity;

                xRotation -= rotY;

                if (playerBody != null)
                {
                    transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                    playerBody.Rotate(0f, rotX, 0f);
                }
            }
            // Release touch
            else if (t.fingerId == touchId && (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled))
            {
                touchId = -1;
            }
        }
    }
}