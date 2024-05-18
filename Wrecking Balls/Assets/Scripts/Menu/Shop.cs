using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Shop : MonoBehaviour
{
    [SerializeField] List<GameObject> ballList = new List<GameObject>();
    [SerializeField] List<GameObject> giftList = new List<GameObject>();
    [SerializeField] List<GameObject> giftTopList = new List<GameObject>();
    [SerializeField] List<GameObject> ballselector = new List<GameObject>();
    [SerializeField] List<BallType> loadingBall = new List<BallType>();
    List<GameObject> unlockedBall = new List<GameObject>();
    [SerializeField] TextMeshProUGUI coinsText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] ScaleButton buyButton;
    [SerializeField] ScaleButton cancelBuyButton;
    [SerializeField] GameObject blockBack;
    [SerializeField] GameObject coinIcon;
    [SerializeField] Animator lightBall;
    [SerializeField] Material greenMat;
    [SerializeField] Material WhaiteMat;
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject lockBuy;
    public GameObject particleS;

    int presentBall = 0;
    int price = 0;
    int coins;

    private void Start()
    {
        UpdateCoins();
        blockBack.SetActive(false);
        PlayerPrefs.SetInt("unlockedBall0", 1);
        ballselector[PlayerPrefs.GetInt("BallNumber", 0)].gameObject.GetComponent<Renderer>().material = greenMat;

        for (int i = 0; i < ballList.Count; i++)
        {
            if (PlayerPrefs.GetInt("unlockedBall" + i, 0) == 1 && i > 0)
            {
                giftList[i - 1].SetActive(false);
                ballList[i].GetComponent<Animator>().enabled = false;
                ballList[i].transform.localScale = Vector3.one;
                unlockedBall.Add(ballList[i]);
            }
        }
    }

    public void UpdateCoins()
    {
        coins = PlayerPrefs.GetInt("Coin", 0);
        UpdateCoinsText();
    }
    
    void UpdateCoinsText()
    {
        coinsText.text = coins.ToString();
    }
    public void CheckUnlockedBall(int ball)
    {
        if (presentBall != 0) return;
        if (gameManager.audioControl.isActiveAndEnabled) gameManager.audioControl.PressButton();
        if (PlayerPrefs.GetInt("unlockedBall" + ball.ToString(), 0) == 1)
        {
            SelectBall(ball);
        }
        else
        {
            presentBall = ball;
            PresentBall(ball);
        }
    }

    void SelectBall(int ball)
    {
        PlayerPrefs.SetInt("BallNumber", ball);
        foreach (var i in ballselector) 
        {
            i.gameObject.GetComponent<Renderer>().material = WhaiteMat;
        }
        ballselector[ball].gameObject.GetComponent<Renderer>().material = greenMat;


        foreach (var item in loadingBall)
        {
            item.ChangerBall();
        }
    }

    void PresentBall(int ball)
    {
        if (presentBall < 4)
        {
            price = 200;
        }
        else
        {
            price = presentBall * 100 - 100;
        }

        if(price == 1000)
        {
            coinIcon.transform.localPosition = new Vector3(0.55f, -0.175f, 0.007f);
        }
        else
        {
            coinIcon.transform.localPosition = new Vector3(0.5f, -0.175f, 0.007f);
        }
        priceText.text = price.ToString();

        if (PlayerPrefs.GetInt("Coin", 0) >= price)
        {
            lockBuy.SetActive(false);
        }
        else
        {
            lockBuy.SetActive(true);
        }

        ballList[ball].GetComponent<Animator>().SetBool("active", true);
        giftList[ball-1].GetComponent<Animator>().SetBool("active", true);
        buyButton.ShowButton();
        cancelBuyButton.ShowButton();
        blockBack.SetActive(true);
    }

    public void CancelBuy(bool sound)
    {
        if (gameManager.audioControl.isActiveAndEnabled && sound) gameManager.audioControl.PressButton();
        ballList[presentBall].GetComponent<Animator>().SetBool("active", false);
        giftList[presentBall-1].GetComponent<Animator>().SetBool("active", false);
        Invoke("DeselectBall", 0.70f);
        buyButton.HideButton();
        cancelBuyButton.HideButton();
        blockBack.SetActive(false);
    }
    public void BuyBall()
    {
        if (PlayerPrefs.GetInt("Coin", 0) >= price)
        {
            UnlockBall();
            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin", 0) - price);
            StartCoroutine(SubstractCoins(price));
        }
    }
    public void UnlockBall()
    {
        if (gameManager.audioControl.isActiveAndEnabled) gameManager.audioControl.BuyBall();
        lightBall.SetBool("active", true);
        buyButton.HideButton();
        cancelBuyButton.HideButton();
        giftTopList[presentBall - 1].GetComponent<Animator>().SetBool("Buy", true);
        StartCoroutine(HideGift());

        //Efecto de particulas.
        Instantiate(particleS, lightBall.transform.position, Quaternion.identity);
 
        PlayerPrefs.SetInt("unlockedBall" + presentBall.ToString(), 1);
       
        //Invoke("OffAnimator", 0.70f);
        
    }

    IEnumerator HideGift()
    {
        yield return new WaitForSeconds(1);
        //ballList[presentBall].GetComponent<Animator>().SetBool("rotate", true);
        giftList[presentBall - 1].GetComponent<Animator>().SetBool("hide", true);
        lightBall.SetBool("active", false);
        yield return new WaitForSeconds(5);
        giftList[presentBall - 1].SetActive(false);
        CancelBuy(false);
    }

    void OffAnimator()
    {
        foreach(var ball in unlockedBall)
        {
            ball.GetComponent<Animator>().enabled = false;
        }
        DeselectBall();
    }

    void DeselectBall()
    {
        presentBall = 0;
    }
    IEnumerator SubstractCoins(int coin)
    {
        for (int i = 0; i < coin/5; i++)
        {
            coins -= 5;
            UpdateCoinsText();
            yield return new WaitForFixedUpdate();
        }
        UpdateCoins();
    }
}
