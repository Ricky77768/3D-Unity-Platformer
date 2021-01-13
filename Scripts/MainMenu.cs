using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator transition;
    public void playGame()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }
    IEnumerator LoadLevel(int index)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(index);
    }

    public void quitGame()
    {
        Debug.Log("QUIT THE MF GAME");
        Application.Quit();
    }

    public void Start()
    {
        QualitySettings.vSyncCount = 1;
    }

}
