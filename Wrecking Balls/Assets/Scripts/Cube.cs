using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Cube : MonoBehaviour
{
    public int point = 0;
    GameManager gameManager;
    public TextMeshProUGUI text;
    public Material mat;
    public TMP_FontAsset fontRed;
    public TMP_FontAsset fontBlue;
    public Color baseColor;
    public GameObject particleS;
    public GameObject particleS1;
    AudioControl audioControl;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
    }
    // Start is called before the first frame update
    void Start()
    {
        mat = gameObject.GetComponent<Renderer>().material;
        gameManager = FindObjectOfType<GameManager>();
        StartCoroutine(IncreaseCubeSize());
        text = GetComponentInChildren<TextMeshProUGUI>();
        audioControl = gameManager.audioControl;

        if (point == 0)
        {
            if (gameManager.level % 100 == 0)
            {
                point = gameManager.level * 2;
            }
            else
            {
                point = gameManager.level;
            }

        }

        baseColor = new Color(0f / 255f, 90f / 255f, 255f / 255f);
        UpdateText();
        UpdateColor();
    }

    // Update is called once per frame
    void Update()
    {
        
           
    }
    /// <summary>
    /// Reduce la puntuacion del cubo y lo destruye al llegar a 0.
    /// </summary>
    /// <param name="ballGameObject"></param>Bola que le da el ultimo toque.
    public void Point(int damage)
    {
        point -= damage;
        UpdateText();
        StartCoroutine("ChangeColor");

        if (point == 0)
        {
            gameObject.layer = 2;
            gameManager.cubeList.Remove(gameObject);
            Instantiate(particleS, transform.position, Quaternion.Euler(
                Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
            Instantiate(particleS1, transform.position, Quaternion.Euler(
                Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
            Destroy(gameObject);
        }
        
    }
    /// <summary>
    /// Atualiza el texto en pantalla de los choques restantes del cubo.
    /// </summary>
    private void UpdateText()
    {
        text.text = point.ToString();
        if(point < 1000) 
        {
            text.fontSize = 0.4f;
        }
        else
        {
            text.fontSize = 0.3f;
        }
        
    }
    public float percent;
    public float intensity;
    private void UpdateColor()
    {
        percent = point * 100f / gameManager.level;
        intensity = percent * 50 / 100f;

        mat.SetColor("_EmissionColor", baseColor * intensity);
    }

    IEnumerator ChangeColor()
    {
        mat.SetColor("_EmissionColor", new Color(32f, 0, 0, 1000f));
        text.font = fontRed;
        yield return new WaitForFixedUpdate();
        UpdateColor();
        text.font = fontBlue;
    }

    public bool CheckGameOver()
    {
        UpdateColor();
        if (transform.position.y < -2.1f)
        {
            return true;
        }
        return false;
    }

    public void GameOver()
    {
        gameManager.cubeList.Remove(gameObject);
        Instantiate(particleS, transform.position, Quaternion.Euler(
            Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
        Instantiate(particleS1, transform.position, Quaternion.Euler(
            Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
        Destroy(gameObject);
    }

    float tiempoDeEscalado = 0.33f;
    IEnumerator IncreaseCubeSize()
    {
        float tiempoPasado = 0.0f;

        while (tiempoPasado < tiempoDeEscalado)
        {
            // Incrementa el tiempo pasado desde el inicio de la corrutina
            tiempoPasado += Time.deltaTime;

            // Calcula el factor de interpolación (t) basado en el tiempoPasado
            float t = tiempoPasado / tiempoDeEscalado;

            // Usa Vector3.Lerp para interpolar entre Vector3.zero (escala inicial) y Vector3.one (escala final)
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

            yield return null; // Espera hasta el próximo frame
        }

        // Garantiza que la escala sea exactamente 1 al final
        transform.localScale = Vector3.one;
    }

}
