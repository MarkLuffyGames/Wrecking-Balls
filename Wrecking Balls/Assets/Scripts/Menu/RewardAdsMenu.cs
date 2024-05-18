using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardAdsMenu : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isSpeedLocked == "false") CloseRewardAdsMenu();
    }
    public void CloseRewardAdsMenu()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}
