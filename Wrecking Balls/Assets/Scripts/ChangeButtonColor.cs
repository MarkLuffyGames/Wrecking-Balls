using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class ChangeButtonColor : MonoBehaviour
{
    public Material defaultMat;
    public Material newMat;
    TextMeshProUGUI text;
    public TMP_FontAsset fontRed;
    public TMP_FontAsset fontBlue;
    Image image;
    public bool isAutomatic = true;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RedColor()
    {
        image.material = newMat;
        text.font = fontRed;
        if (isAutomatic)
        {
            Invoke("BlueColor", 0.2f);
        }
        
    }
    public void BlueColor()
    {
        image.material = defaultMat;
        text.font = fontBlue;
    }
}
