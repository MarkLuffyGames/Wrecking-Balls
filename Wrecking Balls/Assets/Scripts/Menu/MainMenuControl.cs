using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour
{
    
    public List<ScaleButton> ButtonMenuScaleList = new List<ScaleButton>();
    [SerializeField] Dashboard dashboard;
    public bool active;
    [SerializeField] GameManager gameManager;
    [SerializeField] playerMove playerMove;
    [SerializeField] GameObject Leaderboard;
    // Start is called before the first frame update
    void Start()
    {
        active = true;
        Invoke("CheckSave", 0);
    }

    public void StartGame()
    {
        if(active == true)
        {
            HideButton();
            dashboard.ShowDashboard();
            gameManager.PlayGame();
        }
        
    }

    void ShowButton()
    {
        
        foreach (var button in ButtonMenuScaleList)
        {
            button.ShowButton();
        }
        active = true;
    }

    public void HideButton()
    {
        foreach (var button in ButtonMenuScaleList)
        {
            button.HideButton();
        }
        active = false;
    }
    
    void CheckSave()
    {
        
        if (PlayerPrefs.GetInt("GameOver", 0) == 1)
        {
            StartGame();
            playerMove.Continue();
        }
        else
        {
            ShowButton();
        }
    }

    public void ShowLeaderboard()
    {
        Leaderboard.GetComponent<Leaderboard>().ShowLeaderboard();
    }

    public void CloseLeaderboard()
    {
        Leaderboard.GetComponent<Leaderboard>().CloseLeaderboard();
    }
}
