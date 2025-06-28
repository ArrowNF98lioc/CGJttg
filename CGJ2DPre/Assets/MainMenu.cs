using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void OpenMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void ReturnHome()
    {
        Debug.Log("Return Home");
        SceneManager.LoadSceneAsync(1);
    }
}