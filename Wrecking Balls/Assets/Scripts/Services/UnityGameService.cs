using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class UnityGameService : MonoBehaviour
{
    [SerializeField] SettingMenu settingMenu;
    [SerializeField] TextMeshProUGUI m_ExceptionText;
    [SerializeField] TextMeshProUGUI m_NotificationText;

    [SerializeField] List<TextMeshProUGUI> rank = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> rankNames = new List<TextMeshProUGUI>();
    [SerializeField] List<TextMeshProUGUI> rankScore = new List<TextMeshProUGUI>();

    public TMP_FontAsset fontRed;
    public TMP_FontAsset fontBlue;

    public GameObject waitPause;
    public GameObject error;
    public GameObject notificationGameObject;

    public TextMeshProUGUI youRank;
    public TextMeshProUGUI youName;
    public TextMeshProUGUI youScore;

    bool openLeaderboard = false;

    async void Awake()
    {
        waitPause.SetActive(false);
        notificationGameObject.SetActive(false);
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log($"Unity services initialization: {UnityServices.State}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        //PlayerAccountService.Instance.SignedIn += SignInWithUnity;
    }

    private void Start()
    {

        //Shows if a cached session token exist
        Debug.Log($"Cached Session Token Exist: {AuthenticationService.Instance.SessionTokenExists}");

        AuthenticationService.Instance.SignedIn += () =>
        {
            //Shows how to get a playerID
            Debug.Log($"PlayedID: {AuthenticationService.Instance.PlayerId}");

            //Shows how to get an access token
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

            if(PlayerPrefs.GetString("PlayerName", "Player") == "Player")
            {
                PlayerPrefs.SetString("PlayerName", "Player");
                AuthenticationService.Instance.UpdatePlayerNameAsync("Player");
            }

            GetPlayerName();

            waitPause.SetActive(false);
            if (openLeaderboard) GetPlayerScore();
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Signed Out!");
        };

        AuthenticationService.Instance.SignInFailed += errorResponse =>
        {
            Debug.LogError($"Sign in anonymously failed with error code: {errorResponse.ErrorCode}");
            waitPause.SetActive(false);
            error.SetActive(true);
            m_ExceptionText.text = $"{errorResponse.GetType().Name}: {errorResponse.Message}";
            Invoke("Desactive", 2);
        };

        OnClickSignIn();
    }

    public async void GetPlayerName()
    {
        string playerName = await GetPlayerNameAsync();
        string player = playerName.Substring(0, playerName.Length - 5);

        if (player == PlayerPrefs.GetString("PlayerName"))
        {
            PlayerPrefs.SetString("PlayerName", playerName);
            settingMenu.UpdateName();
        }
        else if(playerName == PlayerPrefs.GetString("PlayerName"))
        {
            settingMenu.UpdateName();
        }
        else
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(PlayerPrefs.GetString("PlayerName"));
            GetPlayerName();
        }
    } 
    private async Task<string> GetPlayerNameAsync()
    {
        var playerNameTask = AuthenticationService.Instance.GetPlayerNameAsync();

        // Wait for the task to complete and get the result
        string playerName = await playerNameTask;

        // Return the player name
        return playerName;

    }
    public async void OnClickSignIn()
    {
        waitPause.SetActive(true);
        try
        {
            await SignInWithNotifications();
        }
        catch (RequestFailedException ex)
        {
            waitPause.SetActive(false);
            Debug.LogError($"Sign in anonymously failed with error code: {ex.ErrorCode}");
            error.SetActive(true);
            m_ExceptionText.text = $"{ex.GetType().Name}: {ex.Message}";
            Invoke("Desactive", 2);
        }
    }

    void Desactive()
    {
        error.SetActive(false);
    }
    public void OnClickSignOut()
    {
        AuthenticationService.Instance.SignOut();
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /*public async void StartSignInAsync()
    {
        if (PlayerAccountService.Instance.IsSignedIn)
        {
            SignInWithUnity();
            return;
        }

        try
        {
            await PlayerAccountService.Instance.StartSignInAsync();
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            SetException(ex);
        }
    }

    async void SignInWithUnity()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(PlayerAccountService.Instance.AccessToken);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            SetException(ex);
        }
    }

    void SetException(Exception ex)
    {
        m_ExceptionText.text = ex != null ? $"{ex.GetType().Name}: {ex.Message}" : "";
    }

    public void SignOut()
    {
        AuthenticationService.Instance.SignOut();
        PlayerAccountService.Instance.SignOut();
    }*/


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    const string LeaderboardId = "Top_Scores";
    int Offset { get; set; }
    int Limit { get; set; }
    int RangeLimit { get; set; }
    List<string> FriendIds { get; set; }


    public async void AddScore(int score, bool scores)
    {
        var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, score);
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));

        if(scores) GetPlayerScore();

    }
    LeaderboardEntry player = null;
    public async void GetScores()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));

        foreach (var result in scoresResponse.Results) 
        {
            rank[result.Rank].text = (result.Rank + 1).ToString();
            rankNames[result.Rank].text = result.PlayerName;
            rankScore[result.Rank].text = result.Score.ToString();

            if(player != null)
            {
                if(player.Rank == result.Rank)
                {
                    rank[result.Rank].font = fontRed;
                    rankNames[result.Rank].font = fontRed;
                    rankScore[result.Rank].font = fontRed;
                }
                else
                {
                    rank[result.Rank].font = fontBlue;
                    rankNames[result.Rank].font = fontBlue;
                    rankScore[result.Rank].font = fontBlue;
                }
            }
        }
        if(player.Rank > 9)
        {
            youRank.text = (player.Rank +1).ToString();
            youName.text = player.PlayerName;
            youScore.text = player.Score.ToString();
            youRank.font = fontRed;
            youName.font = fontRed;
            youScore.font = fontRed;
        }
        waitPause.SetActive(false);
    }
    public async void GetPaginatedScores()
    {
        Offset = 10;
        Limit = 10;
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { Offset = Offset, Limit = Limit });
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetPlayerScore()
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            waitPause.SetActive(true);
            try
            {
                var scoreResponse =
               await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
                Debug.Log(JsonConvert.SerializeObject(scoreResponse));

                if (PlayerPrefs.GetInt("record", 0) > scoreResponse.Score)
                {
                    AddScore(PlayerPrefs.GetInt("record"), true);
                }
                else
                {
                    player = scoreResponse;
                    GetScores();
                }

            }
            catch (Exception e)
            {
                if(e.Message == "Leaderboard entry could not be found")
                {
                    AddScore(PlayerPrefs.GetInt("record"), true);
                }
                else
                {
                    error.SetActive(true);
                    m_ExceptionText.text = $"{e.GetType().Name}: {e.Message}";
                    Invoke("Desactive", 2);
                    waitPause.SetActive(false);
                }
            }
        }
        else
        {
            OnClickSignIn();
            openLeaderboard = true;
        }
        
    }

    public async void GetPlayerRange()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetPlayerRangeAsync(LeaderboardId, new GetPlayerRangeOptions { RangeLimit = RangeLimit });
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public async void GetScoresByPlayerIds()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresByPlayerIdsAsync(LeaderboardId, FriendIds);
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }

    public void ClearLeaderboardEntry()
    {
        for (int i = 0; i < 10; i++)
        {
            rank[i].font = fontBlue;
            rank[i].text = "-";
            rankNames[i].text = "";
            rankScore[i].text = "";
        }
        youRank.text = "";
        youName.text = "";
        youScore.text = "";
        youRank.font = fontBlue;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    async Task SignInWithNotifications()
    {
        List<Notification> notifications = null;
        try
        {
            // Sign the Player In, Anonymously in this example
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            // Verify the LastNotificationDate
            var lastNotificationDate = AuthenticationService.Instance.LastNotificationDate;
            long storedNotificationDate = GetLastNotificationReadDate();
            // Compruebe si la fecha de LastNotification está disponible y es mayor que la última notificación leída
            if (lastNotificationDate != null && long.Parse(lastNotificationDate) > storedNotificationDate)
            {
                // Recuperar las notificaciones del backendRecuperar las notificaciones del backend
                notifications = await AuthenticationService.Instance.GetNotificationsAsync();
            }
        }
        catch (AuthenticationException e)
        {
            // Read notifications from the banned player exception
            notifications = e.Notifications;
            // Notify the player with the proper error message
            Debug.LogException(e);
        }
        catch (Exception e)
        {
            // Notify the player with the proper error message
            Debug.LogException(e);
        }

        if (notifications != null)
        {
            // Display notifications
            notificationGameObject.SetActive(true);

            m_NotificationText.text = notifications[0].Message;
            foreach (Notification notification in notifications)
            {
                m_NotificationText.text = notification.Message;
                OnNotificationRead(notification);
            }
        }
    }

    void OnNotificationRead(Notification notification)
    {
        long storedNotificationDate = GetLastNotificationReadDate();
        var notificationDate = long.Parse(notification.CreatedAt);
        if (notificationDate > storedNotificationDate)
        {
            SaveNotificationReadDate(notificationDate);
        }
    }

    void SaveNotificationReadDate(long notificationReadDate)
    {
        // Almacene notificationReadDate, por ejemplo: PlayerPrefs
        PlayerPrefs.SetString("NotificationReadDate", notificationReadDate.ToString());
        PlayerPrefs.Save();
    }

    long GetLastNotificationReadDate()
    {
        // Recupere la notificationReadDate que se almacenó en SaveNotificationReadDate, por ejemplo: PlayerPrefs
        if (PlayerPrefs.HasKey("NotificationReadDate"))
        {
            string valorGuardado = PlayerPrefs.GetString("NotificationReadDate");
            if (long.TryParse(valorGuardado, out long numeroLargo))
            {
                return numeroLargo;
            }
            else
            {
                Debug.LogError("No se pudo convertir el valor guardado a long.");
                return 0;
            }
        }
        return 0;
    }
}
