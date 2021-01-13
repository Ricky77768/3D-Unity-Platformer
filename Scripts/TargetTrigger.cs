using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private int targetsTriggered;
    private int pressureTargetsTriggered;
    private float moveProgress = 0f; // 0 = start point, 1 = endpoint
    
    public List<Targetable> targetsList = new List<Targetable>();
    public List<PressureTarget> pressureTargetList = new List<PressureTarget>();
    public Vector3 move;
    public Quaternion rotation;
    public float moveDuration = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Iterate through each target to see their status
        targetsTriggered = 0;
        pressureTargetsTriggered = 0;

        foreach (Targetable x in targetsList)
        {
            if (x.getStatus()) { targetsTriggered++; }
        }

        foreach (PressureTarget x in pressureTargetList)
        {
            if (x.getStatus()) { pressureTargetsTriggered++; }
        }

        if (targetsTriggered == targetsList.Count && pressureTargetsTriggered == pressureTargetList.Count)
        {
            moveProgress += (1 / moveDuration) * Time.deltaTime;
        }
        else
        {
            moveProgress -= (1 / moveDuration) * Time.deltaTime;
        }
        
        moveProgress = Mathf.Clamp(moveProgress, 0, 1);
        transform.SetPositionAndRotation(originalPosition + move * moveProgress, Quaternion.Slerp(originalRotation, originalRotation * rotation, moveProgress));
    }

}
