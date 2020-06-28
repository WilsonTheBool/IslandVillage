using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    public void ExitGame()
    {
        Application.Quit();

    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);

    }
}
