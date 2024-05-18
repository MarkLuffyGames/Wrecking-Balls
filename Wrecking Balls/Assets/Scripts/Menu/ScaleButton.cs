using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleButton : MonoBehaviour
{
    float tiempoDeEscalado = 0.33f;
    public void ShowButton()
    {
        StartCoroutine(ButtonSize(Vector3.zero, Vector3.one));
    }

    public void HideButton()
    {
        StartCoroutine(ButtonSize(Vector3.one, Vector3.zero));
    }

    IEnumerator ButtonSize(Vector3 init, Vector3 finish)
    {
        if(transform.localScale == init)
        {
            float tiempoPasado = 0.0f;

            while (tiempoPasado < tiempoDeEscalado)
            {
                // Incrementa el tiempo pasado desde el inicio de la corrutina
                tiempoPasado += Time.deltaTime;

                // Calcula el factor de interpolación (t) basado en el tiempoPasado
                float t = tiempoPasado / tiempoDeEscalado;

                // Usa Vector3.Lerp para interpolar entre Vector3.zero (escala inicial) y Vector3.one (escala final)
                transform.localScale = Vector3.Lerp(init, finish, t);

                yield return null; // Espera hasta el próximo frame
            }

            // Garantiza que la escala sea exactamente 1 al final
            transform.localScale = finish;
        }
    }
}
