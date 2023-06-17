using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.sceneManagement;
public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Game Scene");

    }

    public void Quit()
    {
        Application.Quit();
    }
}
