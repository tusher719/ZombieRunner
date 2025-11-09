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

    [Header("Crosshair Reference")]
    private CrosshairController crosshair;

    bool canShoot = true;
    private bool shootButtonPressed = false;
    private bool isWeaponReady = false;

    private void OnEnable()
    {
        StartCoroutine(InitializeWeapon());
    }

    void Start()
    {
        // Find crosshair once at start
        crosshair = FindObjectOfType<CrosshairController>();
    }

    private IEnumerator InitializeWeapon()
    {
        // Reset states
        canShoot = false;
        shootButtonPressed = false;
        isWeaponReady = false;
        
        // Wait a frame to ensure everything is set up
        yield return null;
        
        // Update crosshair range when weapon is enabled/switched
        if (crosshair != null)
        {
            crosshair.SetWeaponRange(range);
        }
        
        // Now weapon is ready
        canShoot = true;
        isWeaponReady = true;
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
        if (!isWeaponReady) return;
        
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
            // Notify crosshair IMMEDIATELY when shooting starts
            if (crosshair != null)
            {
                crosshair.OnShoot();
            }
            
            PlayMuzzleFlash();
            ProcessRaycast();
            ammoSlot.ReduceCurrentAmmo(ammoType);
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