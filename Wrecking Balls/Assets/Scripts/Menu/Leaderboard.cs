using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] List<ScaleButton> leaderboardEntry = new List<ScaleButton>();

    public void ShowLeaderboard()
    {
        foreach (var entry in leaderboardEntry) 
        {
            entry.ShowButton();
        }
    }
    public void CloseLeaderboard()
    {
        foreach (var entry in leaderboardEntry)
        {
            entry.HideButton();
        }
    }
}
