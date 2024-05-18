using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSource2;
    [SerializeField] AudioClip impact;
    [SerializeField] AudioClip getCoin;
    [SerializeField] AudioClip getBall;
    [SerializeField] AudioClip appearCube;
    [SerializeField] AudioClip pressButton;
    [SerializeField] AudioClip buyBall;
    [SerializeField] AudioClip destroyBlock;

    float time;
    float timeDelay = 0.05f;
    
    
    public void Play()
    {
        if(Time.time - time > timeDelay)
        {
            audioSource.clip = impact;
            audioSource.Play();
            time = Time.time;
        }
    }
    public void GetCoin()
    {
        audioSource2.clip = getCoin;
        audioSource2.Play();
    }
    public void GetBall()
    {
        audioSource.clip = getBall;
        audioSource.Play();
    }
    public void AppearCube()
    {
        audioSource2.clip = appearCube;
        audioSource2.Play();
    }

    public void PressButton()
    {
        audioSource2.clip = pressButton;
        audioSource2.Play();
    }

    public void BuyBall()
    {
        audioSource2.clip = buyBall;
        audioSource2.Play();
    }

    public void DestroyBlock()
    {
        audioSource2.clip = destroyBlock;
        audioSource2.Play();
    }
}
