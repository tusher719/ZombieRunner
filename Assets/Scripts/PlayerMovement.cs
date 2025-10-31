using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mobile Controls")]
    public FixedJoystick joystick;
    
    [Header("References")]
    public RigidbodyFirstPersonController fpsController;
    
    [Header("Settings")]
    public bool useMobileControls = true;

    void Update()
    {
        if (useMobileControls && joystick != null)
        {
            // Mobile joystick input কে inject করো FPS Controller এ
            float horizontal = joystick.Horizontal;
            float vertical = joystick.Vertical;
            
            // Input magnitude check
            if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
            {
                // FPS Controller এ input pass করো
                Vector3 move = transform.right * horizontal + transform.forward * vertical;
                fpsController.GetComponent<Rigidbody>().AddForce(move.normalized * fpsController.movementSettings.ForwardSpeed, ForceMode.Acceleration);
            }
        }
    }
}