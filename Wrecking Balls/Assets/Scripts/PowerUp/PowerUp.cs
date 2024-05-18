using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    GameManager gameManager;
    bool activate = false;
    [SerializeField] GameObject particlePowerUp;
    [SerializeField] GameObject TextPowerUp;
    AudioControl audioControl;

    public string type;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioControl = gameManager.audioControl;
    }


    public void GetPowerUp()
    {
        if (!activate)
        {
            if(type == "AddBall")
            {
                activate = true;
                if (audioControl.isActiveAndEnabled) audioControl.GetBall();
                gameManager.ballForAdd++;
                gameManager.powerUpList.Remove(gameObject);
                Instantiate(particlePowerUp, transform.position, transform.rotation);
                Instantiate(TextPowerUp, transform.position, transform.rotation);
                Destroy(gameObject);
            }else if(type == "AddCoin")
            {
                activate = true;
                if (audioControl.isActiveAndEnabled) audioControl.GetCoin();
                gameManager.coinForAdd++;
                gameManager.UpdateCoinText();
                gameManager.powerUpList.Remove(gameObject);
                Instantiate(particlePowerUp, transform.position, transform.rotation);
                Instantiate(TextPowerUp, transform.position, transform.rotation);
                Destroy(gameObject);
            }
            
        }
    }

    public void GameOver()
    {
        gameManager.powerUpList.Remove(gameObject);
        Destroy(gameObject);
    }
    public IEnumerator CheckPosition()
    {
        yield return new WaitForEndOfFrame();
        if (transform.position.y < -3)
        {
            GetPowerUp();
        }
    }

}
