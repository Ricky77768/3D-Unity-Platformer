using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSens = 100f;
    public float cameraBobSpeed = 14f;
    public float bobAmount = 0.06f;
    public Transform playerBody;
    public float zRotation = 0f;

    private float xRotation = 0f;
    private float currentMouseSens;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentMouseSens = mouseSens;
        currentMouseSens = mouseSens;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * currentMouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * currentMouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, zRotation);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void changeMouseSens(float changeFactor)
    {
        currentMouseSens = mouseSens * changeFactor;
    }
}
