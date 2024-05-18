using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] float timeDestroy;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyGameObject", timeDestroy);
    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
