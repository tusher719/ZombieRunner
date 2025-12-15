using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] Camera FPCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;
    [SerializeField] float timeBetweenShots = 0.5f;
    
    [Header("Effects")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;
    
    [Header("Ammo System")]
    [SerializeField] Ammo ammoSlot;
    [SerializeField] AmmoType ammoType;
    [SerializeField] TextMeshProUGUI ammoText;
    
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip emptySound;
    [Range(0f, 1f)]
    public float fireVolume = 0.7f;
    
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
        
        // Get or add AudioSource component
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Configure AudioSource
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0.5f;
        audioSource.volume = 1f;
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

    // Called by mobile shoot button
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
            // Play fire sound
            PlayFireSound();
            
            // Notify crosshair IMMEDIATELY when shooting starts
            if (crosshair != null)
            {
                crosshair.OnShoot();
            }
            
            PlayMuzzleFlash();
            ProcessRaycast();
            ammoSlot.ReduceCurrentAmmo(ammoType);
        }
        else
        {
            // Play empty sound when no ammo
            PlayEmptySound();
        }
        
        yield return new WaitForSeconds(timeBetweenShots);
        canShoot = true;
    }

    private void PlayMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
    }

    private void ProcessRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, range))
        {
            CreateHitImpact(hit);
            
            // Try to damage enemy (no tag check needed!)
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }

    private void CreateHitImpact(RaycastHit hit)
    {
        if (hitEffect != null)
        {
            GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 0.1f);
        }
    }

    void PlayFireSound()
    {
        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound, fireVolume);
        }
    }

    void PlayEmptySound()
    {
        if (audioSource != null && emptySound != null)
        {
            audioSource.PlayOneShot(emptySound, fireVolume * 0.5f);
        }
    }
}