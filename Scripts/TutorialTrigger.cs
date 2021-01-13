using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public bool isExitTrigger; // Check if this is the end trigger of a tutorial text
    public int textIndex;
    public List<GameObject> tutorialTexts = new List<GameObject>();
    public static List<GameObject> lameTest = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject x in tutorialTexts)
        {
            x.SetActive(false);
        }
        UpdateInput();
    }

    void OnTriggerEnter(Collider player)
    {
        if (player.tag == "Player")
        {
            if (!isExitTrigger)
            {
                if (tutorialTexts[textIndex] != null)
                {
                    tutorialTexts[textIndex].SetActive(true);
                }
            }
            else
            {
                Destroy(tutorialTexts[textIndex]);
            }
        }
    }

    public void UpdateInput()
    {
        for (int i = 0; i < tutorialTexts.Count; i++)
        {
            if (tutorialTexts[i] == null) { continue; }
            TextMeshProUGUI textBox = tutorialTexts[i].GetComponent<TextMeshProUGUI>();
            if (textBox == null) { continue; }
            switch (i)
            {
                case 0:
                    textBox.SetText("Move the mouse to look around, " + PlayerPrefs.GetString("move forward") + PlayerPrefs.GetString("strafe left") + PlayerPrefs.GetString("move backward") + PlayerPrefs.GetString("strafe right") + " to move on ground and in air, " + PlayerPrefs.GetString("sprint") + " to sprint, and " + PlayerPrefs.GetString("jump") + " to jump. Green slopes make you zoom in a direction.");
                    break;
                case 1:
                    textBox.SetText("Press " + PlayerPrefs.GetString("swap to pistol") + " to switch to the \"Yeet Pistol\", which can launch you in the opposite direction of where you are looking at when you shoot (" + PlayerPrefs.GetString("shoot") + "). Press " + PlayerPrefs.GetString("reload") + " to reload.");
                    break;
                case 2:
                    textBox.SetText("When sprint jumping (" + PlayerPrefs.GetString("sprint") + " & " + PlayerPrefs.GetString("jump") + ") onto a wall, you can wallrun by moving while looking parallel to the wall. Jumping (" + PlayerPrefs.GetString("jump") + ") again will cause you to leap off the wall perpendicularly.");
                    break;
                case 3:
                    textBox.SetText("Press " + PlayerPrefs.GetString("slide") + " when on a slope to slide down. You need to be sprinting recently.");
                    break;
                case 4:
                    textBox.SetText("Press " + PlayerPrefs.GetString("swap to rifle") + " to swtich to the rifle, ADS (" + PlayerPrefs.GetString("aim down sights") + ") and shoot (" + PlayerPrefs.GetString("shoot") + ") to activate the targets. Reload (" + PlayerPrefs.GetString("reload") + ") if needed. Targets and pressure plates change the environment. Some stays on indefinitely after activation, but others deactivates after some time.");
                    break;
                case 5:
                    textBox.SetText("Let's put everything together in a short course.");
                    break;
                case 6:
                    textBox.SetText("Congratulations, you have completed the tutorial. YA YEET");
                    break;
            }
        }
        
    }
}
