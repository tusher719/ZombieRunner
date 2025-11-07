using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class MobileWeaponZoom : MonoBehaviour
{
    [Header("References")]
    public Camera fpsCamera;
    public RigidbodyFirstPersonController fpsController;
    
    [Header("Zoom Settings")]
    public float zoomedOutFOV = 60f;
    public float zoomedInFOV = 20f;
    public float zoomedOutSensitivity = 2f;
    public float zoomedInSensitivity = 0.5f;
    
    private bool isZoomed = false;
    private WeaponZoom currentWeaponZoom;

    void Start()
    {
        if (fpsCamera == null)
        {
            fpsCamera = Camera.main;
        }
            
        if (fpsController == null)
        {
            fpsController = FindObjectOfType<RigidbodyFirstPersonController>();
        }
    }

    void Update()
    {
        // Find current active weapon's zoom component
        UpdateCurrentWeaponZoom();
    }

    void UpdateCurrentWeaponZoom()
    {
        // Find active weapon
        WeaponeSwitcher switcher = FindObjectOfType<WeaponeSwitcher>();
        if (switcher != null)
        {
            Transform weapons = switcher.transform;
            foreach (Transform weapon in weapons)
            {
                if (weapon.gameObject.activeSelf)
                {
                    currentWeaponZoom = weapon.GetComponent<WeaponZoom>();
                    break;
                }
            }
        }
    }

    // Called from UI button
    public void OnZoomPressed()
    {
        // Check if current weapon has zoom capability
        if (currentWeaponZoom != null)
        {
            isZoomed = !isZoomed;
            
            if (isZoomed)
            {
                ZoomIn();
            }
            else
            {
                ZoomOut();
            }
        }
    }

    void ZoomIn()
    {
        if (fpsCamera != null)
        {
            fpsCamera.fieldOfView = zoomedInFOV;
        }
        
        if (fpsController != null)
        {
            fpsController.mouseLook.XSensitivity = zoomedInSensitivity;
            fpsController.mouseLook.YSensitivity = zoomedInSensitivity;
        }
        
        isZoomed = true;
    }

    void ZoomOut()
    {
        if (fpsCamera != null)
        {
            fpsCamera.fieldOfView = zoomedOutFOV;
        }
        
        if (fpsController != null)
        {
            fpsController.mouseLook.XSensitivity = zoomedOutSensitivity;
            fpsController.mouseLook.YSensitivity = zoomedOutSensitivity;
        }
        
        isZoomed = false;
    }
    
    // Called when weapon switches
    void OnDisable()
    {
        ForceZoomOut();
    }

    // Public method for external calls
    public void ForceZoomOut()
    {
        if (isZoomed)
        {
            ZoomOut();
        }
    }

    // Public check method
    public bool IsZoomed()
    {
        return isZoomed;
    }
}