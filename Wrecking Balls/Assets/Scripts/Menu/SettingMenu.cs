using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] List<ScaleButton> ButtonMenuScaleList = new List<ScaleButton>();
    
    [SerializeField] TextMeshProUGUI soundText;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI nameInput;
    [SerializeField] GameObject changeName;
    [SerializeField] GameObject FPSGameObject;
    [SerializeField] AudioSource audioSource1;
    [SerializeField] AudioSource audioSource2;
    [SerializeField] Slider volumenSlider;
    [SerializeField] UnityGameService unityGameService;
    public bool active;
    private void Start()
    {
        //playerName.text = PlayerPrefs.GetString("PlayerName", "Player");
        //inputText.SetActive(false);
        volumenSlider.value = PlayerPrefs.GetInt("Volumen", 50);
        SetVolumen();
        UpdateName();
    }

    public void ShowButton()
    {
        foreach (var button in ButtonMenuScaleList)
        {
            if (button.gameObject.activeInHierarchy)
            {
                if (button == ButtonMenuScaleList[5]) continue;
                button.ShowButton();
            }
        }
        active = true;
    }

    public void HideButton()
    {
        foreach (var button in ButtonMenuScaleList)
        {
            if (button.gameObject.activeInHierarchy)
            {
                button.HideButton();
            }
        }
        active = false;
    }

    public void ActiveChangeName()
    {
        ButtonMenuScaleList[5].ShowButton();
        ButtonMenuScaleList[7].HideButton();

        // PlayerPrefs.SetString("PlayerName", nameInput.text);
        // AuthenticationService.Instance.UpdatePlayerNameAsync(nameInput.text);
    }

    public void ChangeName()
    {
        PlayerPrefs.SetString("PlayerName", nameInput.text);
        AuthenticationService.Instance.UpdatePlayerNameAsync(nameInput.text);

        ButtonMenuScaleList[5].HideButton();
        ButtonMenuScaleList[7].ShowButton();

        UpdateName();
        unityGameService.GetPlayerName();
    }

    public void SetVolumen()
    {
        audioSource1.volume = volumenSlider.value / 100;
        audioSource2.volume = volumenSlider.value*2 / 100;
        soundText.text = volumenSlider.value.ToString();
        PlayerPrefs.SetInt("Volumen", (int)volumenSlider.value);
    }

    public void UpdateName()
    {
        playerName.text = PlayerPrefs.GetString("PlayerName");
    }
}
