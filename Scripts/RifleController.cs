using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RifleController : MonoBehaviour
{
    public Animator anim;
    public Camera playerCam;
    public AudioSource shootAudioSource;
    public AudioSource reloadAudioSource;
    public TextMeshProUGUI ammoDisplay;
    public PlayerController playerController;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    public AudioClip firingSound;
    public AudioClip reloadSound;

    public int rifleMaxAmmoCapacity = 30;
    private bool isADS;
    private bool isReloading;
    private int rifleCurrentAmmo;

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
        if (isReloading) { return; }

        shootAudioSource.clip = firingSound;
        if (isADS)
        {
            anim.Play("Aim Fire", 0, 0f);
        }
        else
        {
            muzzleFlash.Play();
            anim.Play("Fire", 0, 0f);
        }
        shootAudioSource.Play();
        rifleCurrentAmmo--;

        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, Mathf.Infinity, ~(1 << 13)))
        {
            GameObject impactGameObject = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGameObject, 0.5f);

            Targetable target = hit.transform.GetComponent<Targetable>();
            if (target != null)
            {
                target.isHit();
            }
        }
    }

    void ReloadHandler()
    {
        if (isReloading || rifleCurrentAmmo == rifleMaxAmmoCapacity) { return; }
        anim.Play("Reload Out Of Ammo", 0, 0f);
        reloadAudioSource.clip = reloadSound;
        reloadAudioSource.Play();
        rifleCurrentAmmo = rifleMaxAmmoCapacity;
    }

    void ADSHandler()
    {
        isADS = Input.GetKey(playerController.keybinds["aim down sights"]) && !isReloading;

        if (isADS)
        {
            anim.SetBool("Aim", true);
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, playerController.adsFov, playerController.fovSpeed * Time.deltaTime);
        }
        else
        {
            anim.SetBool("Aim", false);
            playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, playerController.defaultFov, playerController.fovSpeed * Time.deltaTime);
        }
    }


    // Update is called once per frame
    void Update()
    {
        // if paused
        if (Time.timeScale == 0) { return; }
        isReloading = anim.GetCurrentAnimatorStateInfo(0).IsName("Reload Out Of Ammo") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
        if (Input.GetKeyDown(playerController.keybinds["reload"]) || rifleCurrentAmmo == 0)
        {
            ReloadHandler();
        }

        SprintHandler();
        ADSHandler();
        if (Input.GetKeyDown(playerController.keybinds["shoot"]) && !isReloading && rifleCurrentAmmo > 0) { WeaponFiredHandler(); }

        ammoDisplay.text = rifleCurrentAmmo + " / " + rifleMaxAmmoCapacity;
    }

    void Start()
    {
        rifleCurrentAmmo = rifleMaxAmmoCapacity;
        shootAudioSource.Stop();
        reloadAudioSource.Stop();
    }
}
