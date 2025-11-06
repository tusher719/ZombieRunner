using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Weapon : MonoBehaviour
{
    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;
    [SerializeField] Ammo ammoSlot;
    [SerializeField] AmmoType ammoType;
    [SerializeField] float timeBetweenShots = 0.5f;
    [SerializeField] TextMeshProUGUI ammoText;

    bool canShoot = true;
    private bool shootButtonPressed = false;
    private bool isWeaponReady = false;  // New flag

    private void OnEnable()
    {
        StartCoroutine(InitializeWeapon());
    }

    private IEnumerator InitializeWeapon()
    {
        // Reset states
        canShoot = false;
        shootButtonPressed = false;
        isWeaponReady = false;
        
        Debug.Log($"🔧 Initializing {gameObject.name}...");
        
        // Wait a frame to ensure everything is set up
        yield return null;
        
        // Now weapon is ready
        canShoot = true;
        isWeaponReady = true;
        
        Debug.Log($"✅ {gameObject.name} ready to fire!");
    }

    private void OnDisable()
    {
        // Cleanup when weapon is disabled
        isWeaponReady = false;
        canShoot = false;
        shootButtonPressed = false;
    }

    void Update()
    {
        if (!isWeaponReady) return;  // Don't do anything if weapon not ready
        
        DisplayAmmo();

        // PC only mouse shooting
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0) && canShoot && !IsPointerOverUIElement())
        {
            StartCoroutine(Shoot());
        }
        #endif
        
        // Mobile: button only
        if (shootButtonPressed && canShoot)
        {
            Debug.Log($"🔫 Firing {gameObject.name} - Ammo: {ammoSlot.GetAmmoAmount(ammoType)}");
            StartCoroutine(Shoot());
            shootButtonPressed = false;
        }
    }

    private bool IsPointerOverUIElement()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    public void OnShootButtonPressed()
    {
        if (isWeaponReady && canShoot)
        {
            shootButtonPressed = true;
            Debug.Log($"📱 Shoot pressed on {gameObject.name} (Ready: {isWeaponReady}, CanShoot: {canShoot})");
        }
        else
        {
            Debug.LogWarning($"⚠️ Cannot shoot {gameObject.name} - Ready: {isWeaponReady}, CanShoot: {canShoot}");
        }
    }

    private void DisplayAmmo()
    {
        int currentAmmo = ammoSlot.GetAmmoAmount(ammoType);
        ammoText.text = currentAmmo.ToString();
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        
        if (ammoSlot.GetAmmoAmount(ammoType) > 0)
        {
            PlayMuzzleFlash();
            ProcessRaycast();
            ammoSlot.ReduceCurrentAmmo(ammoType);
            Debug.Log($"💥 Shot fired! Remaining ammo: {ammoSlot.GetAmmoAmount(ammoType)}");
        }
        else
        {
            Debug.Log("⚠️ No ammo!");
        }
        
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    private void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }

    private void ProcessRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range))
        {
            CreateHitImpact(hit);
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target == null) return;
            target.TakeDamage(damage);
        }
    }

    private void CreateHitImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impact, .1f);
    }
}