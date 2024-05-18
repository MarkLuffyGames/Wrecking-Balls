using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveToPointShot : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI text;
    public int remainingBalls = 0;
    private void Awake()
    {
       
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        text = GetComponent<TextMeshProUGUI>();
        UpdateValues();
    }
    /// <summary>
    /// Resta las bolas que van siendo disparadas y actualiza el texto en pantalla.
    /// </summary>
    public void SubtractBall()
    {
        remainingBalls--;
        text.text = $"x{remainingBalls}";
        if(remainingBalls == 0)
        {
            gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Actualiza los valores para mostrar las bolas restantes y ajustar la nueva posision en pantalla.
    /// </summary>
    public void UpdateValues()
    {
        if(gameManager.ballList.Count > 0 ) transform.position 
                = gameManager.ballList[0].transform.position + new Vector3(1.2f, 0.2f, -1);
        if (transform.position.x > 3.75f) transform.position = new Vector3(3.75f, transform.position.y ,-1);
        remainingBalls = gameManager.ballList.Count;
        text.text = $"x{remainingBalls}";
    }

}
