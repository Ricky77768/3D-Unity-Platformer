using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SidearmController : MonoBehaviour
{
    public Animator anim;
    public Camera playerCam;
    public AudioSource shootAudioSource;
    public AudioSource reloadAudioSource;
    public TextMeshProUGUI ammoDisplay;
    public PlayerController playerController;
    public ParticleSystem muzzleFlash;

    public AudioClip firingSound;
    public AudioClip reloadSound;

    public int sidearmMaxAmmoCapacity = 1;
    public float impactForceMagnitude = 75f;

    private bool isReloading;
    private int sidearmCurrentAmmo;
    

    void SprintHandler()
    {
        if (playerController.speed == playerController.sprintSpeed && playerController.grounded())
        {
            anim.SetBool("Run", true);
        }
        else
        {
            anim.SetBool("Run", false);
        }
    }

    void WeaponFiredHandler()
    {
        if (isReloading || playerController.isWallRun() != 0) { return; }

        shootAudioSource.clip = firingSound;
        anim.Play("Fire", 0, 0f);
        muzzleFlash.Play();
        playerController.AddImpact(-playerCam.transform.forward, impactForceMagnitude);
  
        shootAudioSource.Play();
        sidearmCurrentAmmo--;
        playerController.resetTimeSinceslidable();
    }

    void ReloadHandler()
    {
        if (isReloading || sidearmCurrentAmmo == sidearmMaxAmmoCapacity) { return; }
        anim.Play("Reload Out Of Ammo", 0, 0f);
        reloadAudioSource.clip = reloadSound;
        reloadAudioSource.Play();
        sidearmCurrentAmmo = sidearmMaxAmmoCapacity;
    }

    // Update is called once per frame
    void Update()
    {
        // if paused
        if (Time.timeScale == 0) { return; }
        isReloading = anim.GetCurrentAnimatorStateInfo(0).IsName("Reload Out Of Ammo") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
        if (Input.GetKeyDown(playerController.keybinds["reload"]))
        {
            ReloadHandler();
        }

        SprintHandler();
        if (Input.GetKeyDown(playerController.keybinds["shoot"]) && !isReloading && sidearmCurrentAmmo > 0) { WeaponFiredHandler(); }

        ammoDisplay.text = sidearmCurrentAmmo + " / " + sidearmMaxAmmoCapacity;
    }

    void Start()
    {
        sidearmCurrentAmmo = sidearmMaxAmmoCapacity;
        shootAudioSource.Stop();
        reloadAudioSource.Stop();
    }
}
