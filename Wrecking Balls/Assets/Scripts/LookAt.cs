using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{

    [SerializeField] GameObject target;
    [SerializeField] Transform shopTransform;
    Rigidbody rb;
    public float speed;
    public float cameraPos;
    bool inShop = false;
    Vector3 defaultPos;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        defaultPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null && !inShop)
        {
            transform.LookAt(target.transform);
        }
        MoveCamera(cameraPos);

    }

    public void MoveCamera(float x)
    {
        cameraPos = x;
        rb.MovePosition(Vector3.MoveTowards(transform.position, new Vector3(x,transform.position.y,transform.position.z),
                speed * Time.deltaTime));
    }

    public void MoveToShop()
    {
        inShop = true;
        MoveCamera(shopTransform.position.x + 2.85f);
        speed = 50f;
    }

    public void HomeMenu()
    {
        MoveCamera(defaultPos.x);
    }

    public void StarGame()
    {
        inShop = false;
        speed = 20f;
    }
}
