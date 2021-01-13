using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Targetable : MonoBehaviour
{
    private bool isOn = false;
    private float timeSinceHit = 0f;
    private new Renderer renderer;

    // The duration the target stays on, 0 means it stays on indefinitely until shot again
    public float activateDuration = 0f; 
    public Material on;
    public Material off;

    private void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Called once per frame
    void Update()
    {
        // if paused
        if (Time.timeScale == 0) { return; }

        timeSinceHit += Time.deltaTime;
        if (activateDuration != 0 && timeSinceHit > activateDuration)
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

    // Toggles the state of the target when hit by rifle
    public void isHit()
    {
        if (activateDuration != 0)
        {
            isOn = true;
            timeSinceHit = 0;
        } 
        else 
        {
            isOn = !isOn;
        }
    }

    // Returns the state of the target
    public bool getStatus() { return isOn; }

}
