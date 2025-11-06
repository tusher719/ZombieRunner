using UnityEngine;

public class MobileShootButton : MonoBehaviour
{
    private Transform weaponsParent;

    void Start()
    {
        // Find Weapons parent GameObject
        weaponsParent = GameObject.Find("Weapons")?.transform;
        
        if (weaponsParent == null)
        {
            Debug.LogError("❌ Weapons parent not found!");
        }
        else
        {
            Debug.Log("✅ Mobile shoot button connected!");
        }
    }

    // Called from UI Button
    public void OnShootPressed()
    {
        if (weaponsParent == null) return;

        // Find currently active weapon
        Weapon activeWeapon = null;
        
        foreach (Transform weapon in weaponsParent)
        {
            if (weapon.gameObject.activeSelf)
            {
                activeWeapon = weapon.GetComponent<Weapon>();
                if (activeWeapon != null)
                {
                    Debug.Log($"📱 Found active weapon: {weapon.name}");
                    break;
                }
            }
        }

        // Fire the active weapon
        if (activeWeapon != null)
        {
            activeWeapon.OnShootButtonPressed();
        }
        else
        {
            Debug.LogWarning("⚠️ No active weapon found!");
        }
    }
}