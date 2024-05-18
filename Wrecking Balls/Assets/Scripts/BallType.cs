using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallType : MonoBehaviour
{
    [SerializeField] GameObject[] ballTypes;
    private void Awake()
    {
        ChangerBall();
    }

    public void ChangerBall()
    {
        int selctedBall = PlayerPrefs.GetInt("BallNumber", 0);
        foreach (var ball in ballTypes)
        {
            if(ball == ballTypes[selctedBall])
            {
                ball.SetActive(true);
            }
            else
            {
                ball.SetActive(false);
            }
        }
    }
}
