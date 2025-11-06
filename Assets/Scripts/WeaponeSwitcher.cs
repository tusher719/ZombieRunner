using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponeSwitcher : MonoBehaviour
{
    [SerializeField] int currentWeapon = 0;

    // Add this at the top of the class
    private MobileWeaponZoom mobileZoom;

    void Start()
    {
        SetWeaponActive();
        mobileZoom = FindObjectOfType<MobileWeaponZoom>();
    }

    void Update()
    {
        int previousWeapon = currentWeapon;

        ProcessKeyInput();
        ProcessScrollWheel();

        if (previousWeapon != currentWeapon)
        {
            // Auto zoom out when switching weapons
            if (mobileZoom != null && mobileZoom.IsZoomed())
            {
                mobileZoom.ForceZoomOut();
            }
            
            SetWeaponActive();
        }
    }
    private void ProcessScrollWheel()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (currentWeapon >= transform.childCount - 1)
            {
                currentWeapon = 0;
            }
            else
            {
                currentWeapon++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (currentWeapon <= 0)
            {
                currentWeapon = transform.childCount - 1;
            }
            else
            {
                currentWeapon--;
            }
        }
    }

    private void ProcessKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeapon = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeapon = 2;
        }
    }

    private void SetWeaponActive()
    {
        int weaponIndex = 0;
    
        foreach (Transform weapon in transform)
        {
            if (weaponIndex == currentWeapon)
            {
                // Activate current weapon
                weapon.gameObject.SetActive(true);
                
                // Ensure weapon component is enabled and ready
                Weapon weaponScript = weapon.GetComponent<Weapon>();
                if (weaponScript != null)
                {
                    weaponScript.enabled = true;
                }
                
                Debug.Log($"✅ Switched to: {weapon.name}");
            }
            else
            {
                // Deactivate other weapons
                weapon.gameObject.SetActive(false);
            }
            weaponIndex++;
        }
    }

    // ✅ Mobile buttons এর জন্য add করো
    public void SwitchToNextWeapon()
    {
        // Auto zoom out before switching
        if (mobileZoom != null && mobileZoom.IsZoomed())
        {
            mobileZoom.ForceZoomOut();
        }

        currentWeapon++;
        if (currentWeapon >= transform.childCount)
        {
            currentWeapon = 0;
        }
        SetWeaponActive();
    }

    public void SwitchToPreviousWeapon()
    {
        // Auto zoom out before switching
        if (mobileZoom != null && mobileZoom.IsZoomed())
        {
            mobileZoom.ForceZoomOut();
        }
        
        currentWeapon--;
        if (currentWeapon < 0)
        {
            currentWeapon = transform.childCount - 1;
        }
        SetWeaponActive();
    }
    
}

