using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashboard : MonoBehaviour
{
    bool show = false;
    public void ShowDashboard()
    {
        StartCoroutine(Down());
        show = true;
    }

    public void HideDashboard()
    {
        StartCoroutine(Up());
        show = false;
    }
    IEnumerator Up()
    {
        if (show)
        {
            int count = 120 / PlayerPrefs.GetInt("FPS", 60);
            for (int i = 0; i < 80 / count; i++)
            {
                transform.position = transform.position + new Vector3(0, 0.025f * count, 0);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator Down()
    {
        if(!show)
        {
            int count = 120 / PlayerPrefs.GetInt("FPS", 60);
            for (int i = 0; i < 80 / count; i++)
            {
                transform.position = transform.position + new Vector3(0, -0.025f * count, 0);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
