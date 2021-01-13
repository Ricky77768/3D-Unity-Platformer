using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System;

public class PlayerController : MonoBehaviour
{
    // Player objects
    public CharacterController cc;
    public Camera playerCamera;
    public Transform groundCollider;
    public LayerMask groundMask;
    public GameObject rifle;
    public GameObject sidearm;
    
    // Movement variables
    private Vector3 move = Vector3.zero;
    private Vector3 slopeMove = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 impactForce = Vector3.zero;
    private int wallrunState;
    private bool isSliding;
    private bool isForcedSliding;
    private bool isGrounded;
    private float xMove;
    private float zMove;
    private float xAerialInfluence = 0f;
    private float zAerialInfluence = 0f;
    private float collisionAngle;
    private float timeSinceSlideable;
    private GameObject currentCheckpoint;

    public float speed = 10f;
    public float walkSpeed = 10f;
    public float sprintSpeed = 20f;
    public float groundDistance = 0.4f;
    public float jumpHeight = 4.5f;
    public float gravity = -39.24f;
    public float xAerialMovementInfluence = 1.2f;
    public float zAerialMovementInfluence = 0.7f;

    public float wallJumpForce = 60f;
    public float wallRunCameraRotationAmount = 20f;
    public float terminalWallrunFallVelocity = -4f;
    public float impactEnergyConsumptionRate = 2.5f;

    public float minCollisionAngle = 5f;
    public float maxCollisionAngle = 50f;
    public float slideIntensity = 0.2f;
    public float slideVelocityFalloff = 2f;
    public float slideTriggerGracePeriod = 0.8f;

    // FOV variables
    public float defaultFov = 60.0f;
    public float adsFov = 20.0f;
    public float fovSpeed = 15.0f;

    private Vector3 cameraOriginalPos;

    public Dictionary<string, KeyCode> keybinds;

    private int ignoreLayerMask = ~(1 << 11 | 1 << 12);

    private void Awake()
    {
        // Default keybinds
        if (!PlayerPrefs.HasKey("move forward")) {PlayerPrefs.SetString("move forward", KeyCode.W.ToString());}
        if (!PlayerPrefs.HasKey("move backward")) {PlayerPrefs.SetString("move backward", KeyCode.S.ToString());}
        if (!PlayerPrefs.HasKey("strafe left")) {PlayerPrefs.SetString("strafe left", KeyCode.A.ToString());}
        if (!PlayerPrefs.HasKey("strafe right")) {PlayerPrefs.SetString("strafe right", KeyCode.D.ToString());}
        if (!PlayerPrefs.HasKey("jump")){PlayerPrefs.SetString("jump", KeyCode.Space.ToString());}
        if (!PlayerPrefs.HasKey("sprint")){PlayerPrefs.SetString("sprint", KeyCode.LeftShift.ToString());}
        if (!PlayerPrefs.HasKey("slide")){PlayerPrefs.SetString("slide", KeyCode.LeftControl.ToString());}
        if (!PlayerPrefs.HasKey("shoot")){PlayerPrefs.SetString("shoot", KeyCode.Mouse0.ToString());}
        if (!PlayerPrefs.HasKey("reload")){PlayerPrefs.SetString("reload", KeyCode.R.ToString());}
        if (!PlayerPrefs.HasKey("aim down sights")){PlayerPrefs.SetString("aim down sights", KeyCode.Mouse1.ToString());}
        if (!PlayerPrefs.HasKey("swap to rifle")){PlayerPrefs.SetString("swap to rifle", KeyCode.Alpha1.ToString());}
        if (!PlayerPrefs.HasKey("swap to pistol")){PlayerPrefs.SetString("swap to pistol", KeyCode.Alpha2.ToString());}

        PlayerPrefs.Save();

        // Get user keybinds from PlayerPrefs
        keybinds = new Dictionary<string, KeyCode>();

        keybinds["move forward"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("move forward"));
        keybinds["move backward"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("move backward"));
        keybinds["strafe left"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("strafe left"));
        keybinds["strafe right"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("strafe right"));
        keybinds["jump"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jump"));
        keybinds["sprint"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprint"));
        keybinds["slide"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("slide"));
        keybinds["shoot"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("shoot"));
        keybinds["reload"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("reload"));
        keybinds["aim down sights"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("aim down sights"));
        keybinds["swap to rifle"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("swap to rifle"));
        keybinds["swap to pistol"] = (KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("swap to pistol"));
    }

    private void Start()
    {
        cameraOriginalPos = playerCamera.transform.localPosition; 
    }

    // Main update function (once per frame)
    void Update()
    {
        // if paused
        if (Time.timeScale == 0) { return; }

        if (transform.position.y < -35f)
        {
            returnToCheckpoint();
        }

        SprintHandler();
        WeaponSwapHandler();
        isGrounded = Physics.Raycast(groundCollider.position, Vector3.down, out RaycastHit underneathObject, 0.4f, ignoreLayerMask);
        wallrunState = isWallRun();

        BoostPanelChecker(underneathObject);
        PressureTargetChecker(underneathObject);

        // Check wallrun state 
        if (wallrunState == 1 && zMove > 0.9f && !isGrounded)
        {
            playerCamera.GetComponent<PlayerLook>().zRotation = Mathf.Lerp(playerCamera.GetComponent<PlayerLook>().zRotation, wallRunCameraRotationAmount, 5f * Time.deltaTime);
            gravity = -15f;

            if (velocity.y < terminalWallrunFallVelocity)
            {
                velocity.y = terminalWallrunFallVelocity;
            }

            if (Input.GetKeyDown(keybinds["jump"]))
            {
                AddImpact(-transform.right + transform.up * 0.25f + transform.forward * 0.25f, wallJumpForce);
                timeSinceSlideable = 0;
            }
        }
        else if (wallrunState == 2 && zMove > 0.9f && !isGrounded)
        {
            playerCamera.GetComponent<PlayerLook>().zRotation = Mathf.Lerp(playerCamera.GetComponent<PlayerLook>().zRotation, -wallRunCameraRotationAmount, 5f * Time.deltaTime);
            gravity = -15f;

            if (velocity.y < terminalWallrunFallVelocity)
            {
                velocity.y = terminalWallrunFallVelocity;
            }

            if (Input.GetKeyDown(keybinds["jump"]))
            {
                AddImpact(transform.right + transform.up * 0.25f, wallJumpForce);
                timeSinceSlideable = 0;
            }
        }
        else
        {
            playerCamera.GetComponent<PlayerLook>().zRotation = Mathf.Lerp(playerCamera.GetComponent<PlayerLook>().zRotation, 0f, 5f * Time.deltaTime);
            gravity = -39.24f;
        }

        // If grounded, reset fall speed
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (isGrounded)
        {
            if (Input.GetKey(keybinds["move forward"])) { zMove = Mathf.Lerp(zMove, 1f, 15f * Time.deltaTime); }
            else if (Input.GetKey(keybinds["move backward"])) { zMove = Mathf.Lerp(zMove, -1f, 15f * Time.deltaTime); } 
            else { zMove = Mathf.Lerp(zMove, 0f, 15f * Time.deltaTime); }

            if (Input.GetKey(keybinds["strafe right"])) { xMove = Mathf.Lerp(xMove, 1f, 15f * Time.deltaTime); }
            else if (Input.GetKey(keybinds["strafe left"])) { xMove = Mathf.Lerp(xMove, -1f, 15f * Time.deltaTime); }
            else { xMove = Mathf.Lerp(xMove, 0f, 15f * Time.deltaTime); }

            //xMove = Input.GetAxis("Horizontal");
            //zMove = Input.GetAxis("Vertical");
            timeSinceSlideable += Time.deltaTime;

            xAerialInfluence = 0f;
            zAerialInfluence = 0f;
        }
        else
        {
            if (Input.GetKey(keybinds["move forward"])) { zAerialInfluence = Mathf.Lerp(zAerialInfluence, 1f, 15f * Time.deltaTime); }
            else if (Input.GetKey(keybinds["move backward"])) { zAerialInfluence = Mathf.Lerp(zAerialInfluence, -1f, 15f * Time.deltaTime); }
            else { zAerialInfluence = Mathf.Lerp(zAerialInfluence, 0f, 15f * Time.deltaTime); }

            if (Input.GetKey(keybinds["strafe right"])) { xAerialInfluence = Mathf.Lerp(xAerialInfluence, 1f, 15f * Time.deltaTime); }
            else if (Input.GetKey(keybinds["strafe left"])) { xAerialInfluence = Mathf.Lerp(xAerialInfluence, -1f, 15f * Time.deltaTime); }
            else { xAerialInfluence = Mathf.Lerp(xAerialInfluence, 0f, 15f * Time.deltaTime); }

            if (Mathf.Abs(zAerialInfluence) < 0.001f)
            {
                zAerialInfluence = 0f;
            }

            if (Mathf.Abs(xAerialInfluence) < 0.001f)
            {
                xAerialInfluence = 0f;
            }

            if (xAerialInfluence > 0)
            {
                xMove = Mathf.Lerp(xMove, 1f, xAerialMovementInfluence * Time.deltaTime);
            }
            else if (xAerialInfluence < 0)
            {
                xMove = Mathf.Lerp(xMove, -1f, xAerialMovementInfluence * Time.deltaTime);
            }
            else
            {
                xMove = Mathf.Lerp(xMove, 0f, xAerialMovementInfluence * 2f * Time.deltaTime);
            }

            if (zAerialInfluence > 0)
            {
                zMove = Mathf.Lerp(zMove, 1f, zAerialMovementInfluence * Time.deltaTime);
            }
            else if (zAerialInfluence < 0)
            {
                zMove = Mathf.Lerp(zMove, -1f, zAerialMovementInfluence * Time.deltaTime);
            }
            else
            {
                zMove = Mathf.Lerp(zMove, 0f, zAerialMovementInfluence * 2f * Time.deltaTime);
            }
        }

        if (Mathf.Abs(zMove) < 0.001f)
        {
            zMove = 0f;
        }

        if (Mathf.Abs(xMove) < 0.001f)
        {
            xMove = 0f;
        }

        move = transform.right * xMove + transform.forward * zMove;

        // Transform slope and slide velocity onto plane using surface normal
        RaycastHit downwardsProjection;
        Physics.Raycast(groundCollider.position, Vector3.down, out downwardsProjection, 1.2f, ignoreLayerMask);

        if (!downwardsProjection.Equals(null))
        {
            move = Vector3.ProjectOnPlane(move, downwardsProjection.normal);
            collisionAngle = Vector3.Angle(Vector3.up, downwardsProjection.normal);
            if (isSliding || isForcedSliding)
            {
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, cameraOriginalPos + Vector3.down * 2f, 5f * Time.deltaTime);
                Vector3 slide = new Vector3((1f - downwardsProjection.normal.y) * downwardsProjection.normal.x * slideIntensity, 0, (1f - downwardsProjection.normal.y) * downwardsProjection.normal.z * slideIntensity);
                if (slopeMove == Vector3.zero && move.y < 0)
                {
                    slopeMove = move;
                }
                slopeMove += Vector3.ProjectOnPlane(slide, downwardsProjection.normal);
            }
            else
            {
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, cameraOriginalPos, 5f * Time.deltaTime);
            }
        }
        
        // Jumping mechanism
        if (Input.GetKeyDown(keybinds["jump"]) && isGrounded)
        {
            isSliding = false;
            timeSinceSlideable = 0;
            velocity.y = Mathf.Sqrt(-2f * jumpHeight * gravity);
        }

        if (Physics.Raycast(transform.position, transform.up, 2f, ignoreLayerMask))
        {
            velocity.y -= jumpHeight;
        }

        velocity.y += gravity * Time.deltaTime;

        // Impact force mechanism
        impactForce = Vector3.Lerp(impactForce, Vector3.zero, impactEnergyConsumptionRate * Time.deltaTime);
        if (impactForce.magnitude > 0.2F) { cc.Move((move * speed + slopeMove * speed + velocity + impactForce) * Time.deltaTime); }
        else { cc.Move((move * speed + slopeMove * speed + velocity) * Time.deltaTime); }

        // Sliding mechanism
        if (Input.GetKeyDown(keybinds["slide"]))
        {
            isSliding = slideTriggerGracePeriod - timeSinceSlideable > 0 && isGrounded;
        }

        if (isSliding && collisionAngle < minCollisionAngle)
        {
            isSliding = false;
        }

        if (!isSliding && isGrounded)
        {
            slopeMove = Vector3.Lerp(slopeMove, Vector3.zero, slideVelocityFalloff * Time.deltaTime);
        }

        // Forces the player to slide down steep slopes
        isForcedSliding = collisionAngle > maxCollisionAngle;

        // Negate velocity when moving into a wall
        if (Physics.Raycast(transform.position, slopeMove, 1f, ignoreLayerMask))
        {
            slopeMove = Vector3.zero;
        }

        // Negate impactY force when exiting slope
        if (collisionAngle < minCollisionAngle)
        {
            slopeMove.y = 0;
        }
    }

    // Handles sprinting
    void SprintHandler()
    {
        if (!isGrounded) { return; }

        if (Input.GetKey(keybinds["sprint"]) && zMove > 0f && !(playerCamera.fieldOfView < defaultFov - 1))
        {
            if (isGrounded)
            {
                speed = sprintSpeed;
                if (!isSliding)
                {
                    timeSinceSlideable = 0;
                }
            }

            if (Physics.Raycast(transform.position, transform.forward, 1f, ignoreLayerMask))
            {
                speed = sprintSpeed / 3f;
            }
        }
        else
        {
            if (isGrounded)
            {
                speed = walkSpeed;
            }

            if (Physics.Raycast(transform.position, transform.forward, 1f, ignoreLayerMask))
            {
                speed = walkSpeed / 3f;
            }
        }
    }

    // Handles weapon swapping
    void WeaponSwapHandler()
    {
        if (Input.GetKeyDown(keybinds["swap to rifle"]))
        {
            rifle.SetActive(true);
            sidearm.SetActive(false);
        }

        if (Input.GetKeyDown(keybinds["swap to pistol"]))
        {
            playerCamera.fieldOfView = defaultFov;
            rifle.SetActive(false);
            sidearm.SetActive(true);
        }
    }

    void BoostPanelChecker(RaycastHit rh)
    {
        if (rh.transform == null) { return; }
        BoostPanel bp = rh.transform.GetComponent<BoostPanel>();
        if (bp == null) { return; }

        Vector3 boostDir = -rh.transform.forward;
        Vector3.ProjectOnPlane(boostDir, rh.normal);
        AddImpact(boostDir, 3f);
    }

    void PressureTargetChecker(RaycastHit rh)
    {
        if (rh.transform == null) { return; }
        PressureTarget pt = rh.transform.GetComponent<PressureTarget>();
        if (pt == null) { return; }
        pt.isTriggered();
    }

    // Return wall run state
    public int isWallRun()
    {
        RaycastHit hit;
        if (Physics.Raycast(groundCollider.position, groundCollider.forward, 1f, ignoreLayerMask) || speed != sprintSpeed) 
        {
            return 0;
        }

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.right, out hit, 1f, ignoreLayerMask))
        {
            if (hit.transform == transform) { return 0; }
            return 1;
        }

        if (Physics.Raycast(playerCamera.transform.position, -playerCamera.transform.right, out hit, 1f, ignoreLayerMask))
        {
            if (hit.transform == transform) { return 0; }
            return 2;
        }
        return 0;
    }

    // Return isGrounded boolean
    public bool grounded()
    {
        return isGrounded;
    }

    // Add impact force in a specified direction and mangitude
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        impactForce += dir.normalized * force / 1.0f;
    }

    // Reset time since slidable from another class
    public void resetTimeSinceslidable()
    {
        timeSinceSlideable = 0;
    }

    // Set the current checkpoint
    public void setCurrentCheckpoint(GameObject cp)
    {
        currentCheckpoint = cp;
    }

    // Return to the last checkpoint
    public void returnToCheckpoint()
    {
        transform.position = currentCheckpoint.transform.position;
    }
}
