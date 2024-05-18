using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject prefab;

    // Update is called once per frame
    void Start()
    {
       

    }
    GameObject a;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) 
        {
            La();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Destroy(a);

        }

    }


    void La()
    {
        bool a = true;
        Vector3 randomPos = new Vector3(Random.Range(-11, 11), Random.Range(-7, 7), Random.Range(15, 20));

        var ab = Instantiate(prefab, randomPos,
                         Quaternion.Euler(Random.Range(-20, 20), Random.Range(-20, 20), 0));

        ab.GetComponent<Cube>().point = Random.Range(1, 99);


    }
}
