using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureTarget : MonoBehaviour
{
    private bool isOn = false;
    private float timeSinceTriggered = 0f;
    private new Renderer renderer;

    // The duration the pressure playe it stays on, 0 means it stays on as long as the player is on it
    public float activateDuration = 0f;
    public Material on;
    public Material off;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // if paused
        if (Time.timeScale == 0) { return; }

        timeSinceTriggered += Time.deltaTime;
        if (timeSinceTriggered > activateDuration + 5f * Time.deltaTime)
        {
            isOn = false;
        }

        if (isOn)
        {
            renderer.material = on;
        }
        else
        {
            renderer.material = off;
        }
    }
    
    // Toggles the state of the pressure plate when touching player model
    public void isTriggered()
    {
        isOn = true;
        timeSinceTriggered = 0;
    }

    // Returns the state of the target
    public bool getStatus() { return isOn; }
}
