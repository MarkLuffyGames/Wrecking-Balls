using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject SettingMenu;
    GameManager gameManager;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        pauseMenu.SetActive(false);
    }

    public void Pause()
    {
        gameManager.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        gameManager.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void Restart()
    {
        gameManager.timeScale = 1;
        gameManager.gameOver = 0;
        gameManager.gameData.Save(Vector3.zero, gameManager.isSpeedLocked);
        var scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }

    public void Setting()
    {
        SettingMenu.SetActive(true);
    }
    public void Exit()
    {
        //Invoke("Close", 2);
    }

    void Close()
    {
        SettingMenu.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
