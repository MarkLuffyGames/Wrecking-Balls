using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ShotingGui : MonoBehaviour
{
    public GameObject parent;
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(parent.transform.position, gameManager.directionBall,out hit, 7))
        {
            float distance = Vector3.Distance(parent.transform.position, hit.point);
            transform.localScale = new Vector3(transform.localScale.x, distance, transform.localScale.z);
            transform.localPosition = new Vector3(0f, distance / 2, 0f);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, 7, transform.localScale.z);
            transform.localPosition = new Vector3(0, 3.5f, 0);
        }
    }
}
