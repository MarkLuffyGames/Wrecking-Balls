using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class MoveDown : MonoBehaviour
{
    Rigidbody rb;
    float speed = 10f;
    public Vector3 targetPos;
    GameManager gameManager;
    public bool inPosition; 

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void NewPos()
    {
        targetPos = transform.position + Vector3.down;
        inPosition = false;
        StartCoroutine("MoverAbajo");
    }

    IEnumerator MoverAbajo()
    {
        if (Vector3.Distance(transform.position, targetPos) >= 0.1f)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, targetPos,
            speed * Time.deltaTime));
            yield return new WaitForEndOfFrame();
            StartCoroutine("MoverAbajo");
        }
        else
        {
            transform.position = targetPos;
            inPosition = true;
        }
    }
}
