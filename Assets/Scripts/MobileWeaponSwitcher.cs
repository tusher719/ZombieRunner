using UnityEngine;

public class MobileWeaponSwitcher : MonoBehaviour
{
    private WeaponeSwitcher weaponSwitcher;
    private MobileWeaponZoom weaponZoom;

    void Start()
    {
        weaponSwitcher = FindObjectOfType<WeaponeSwitcher>();
        weaponZoom = FindObjectOfType<MobileWeaponZoom>();
        
        if (weaponSwitcher == null)
        {
            Debug.LogError("❌ WeaponeSwitcher not found!");
        }
        else
        {
            Debug.Log("✅ Weapon switcher connected!");
        }
    }

    // Single button to cycle through weapons
    public void SwitchWeapon()
    {
        if (weaponSwitcher != null)
        {
            // First, disable zoom if active
            if (weaponZoom != null && weaponZoom.IsZoomed())
            {
                weaponZoom.ForceZoomOut();
            }
            
            // Switch to next weapon
            weaponSwitcher.SwitchToNextWeapon();
            Debug.Log("🔄 Weapon switched");
        }
    }
}