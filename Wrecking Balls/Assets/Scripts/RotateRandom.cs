using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRandom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(Random.Range(1, 3), Random.Range(1, 3), Random.Range(1, 3) * Time.deltaTime);
    }
}
