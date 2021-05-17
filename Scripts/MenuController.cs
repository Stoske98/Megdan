using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Sound Volume")]
    public Slider audioSlider;
    public AudioSource audioSource;
    public Text audioText;

    [Header("Effects Volume")]
    public Slider audioSliderEff;
    public AudioSource audioSourceEff;
    public Text audioTextEff;

    [Header("SettingsAnim")]
    public Animator animSettings;
    public Toggle checkFullScreen;

    public Dropdown dropdown;

    private Resolution[] resolutions;
    private List<string> resol;





    private void Start()
    {

        if (!PlayerPrefs.HasKey("volume"))
        {
            PlayerPrefs.SetFloat("volume", 1);
            
        }
        if (!PlayerPrefs.HasKey("volumeEff"))
        {
            PlayerPrefs.SetFloat("volumeEff", 1);
        }
        if (!PlayerPrefs.HasKey("checkbox"))
        {
            PlayerPrefs.SetInt("checkbox", 1);
        }
        if (!PlayerPrefs.HasKey("widthRes"))
        {
            PlayerPrefs.SetInt("widthRes", Screen.currentResolution.width);
        }
        if (!PlayerPrefs.HasKey("heightRes"))
        {
            PlayerPrefs.SetInt("heightRes", Screen.currentResolution.height);
        }


        audioSource.volume = PlayerPrefs.GetFloat("volume");
        audioSlider.value = PlayerPrefs.GetFloat("volume");
        audioText.text = "Sound Volume: " + (int)(audioSlider.value * 100);

        audioSourceEff.volume = PlayerPrefs.GetFloat("volumeEff");
        audioSliderEff.value = PlayerPrefs.GetFloat("volumeEff");
        audioTextEff.text = "Effects Volume: " + (int)(audioSliderEff.value * 100);

        Screen.fullScreen = true;

        resolutions = Screen.resolutions;

        int position = 0;
        int counter = -1;
        resol = new List<string>();
        foreach (var res in resolutions)
        {
            counter++;
            if (PlayerPrefs.GetInt("widthRes") == res.width && PlayerPrefs.GetInt("heightRes") == res.height)
            {
                position = counter;
            }
            resol.Add(res.width + "x" + res.height + " : " + res.refreshRate + "Hz");
            
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(resol);
        dropdown.value = position;



        if (PlayerPrefs.GetInt("checkbox") == 1)
        {
            checkFullScreen.isOn = true;
        }
        else
        {
            checkFullScreen.isOn = false;
        }

        Screen.SetResolution(PlayerPrefs.GetInt("widthRes"), PlayerPrefs.GetInt("heightRes"), Screen.fullScreen);

        


    }

    public void setScreen(bool fullScreen)
    {
        if (fullScreen)
        {
            PlayerPrefs.SetInt("checkbox", 1);
        }
        else
        {
            PlayerPrefs.SetInt("checkbox", 0);
        }
        Screen.fullScreen = fullScreen;
    }

    public void ExitGame()
    {
        if (Screen.fullScreen)
        {
            PlayerPrefs.SetInt("checkbox", 1);
        }
        else
        {
            PlayerPrefs.SetInt("checkbox", 0);
        }

        PlayerPrefs.SetInt("widthRes", Screen.currentResolution.width);
        PlayerPrefs.SetInt("heightRes", Screen.currentResolution.height);
        PlayerPrefs.SetFloat("volume", audioSlider.value);
        PlayerPrefs.SetFloat("volumeEff", audioSliderEff.value);
        Application.Quit();
    }

    public void PlayGame()
    {
        if (Screen.fullScreen)
        {
            PlayerPrefs.SetInt("checkbox", 1);
        }
        else
        {
            PlayerPrefs.SetInt("checkbox", 0);
        }

        PlayerPrefs.SetInt("widthRes", Screen.currentResolution.width);
        PlayerPrefs.SetInt("heightRes", Screen.currentResolution.height);
        PlayerPrefs.SetFloat("volume", audioSlider.value);
        PlayerPrefs.SetFloat("volumeEff", audioSliderEff.value);
        SceneManager.LoadScene("Game");
    }


    public void SetAudio()
    {
        audioSource.volume = audioSlider.value;
        audioText.text = "Sound Volume: " + (int)(audioSlider.value * 100);
    }

    public void SetAudioEff()
    {
        audioSourceEff.volume = audioSliderEff.value;
        audioTextEff.text = "Effects Volume: " + (int)(audioSliderEff.value * 100);
    }

    public void clickOnButton()
    {
        audioSourceEff.Play();
    }

    public void openSettings()
    {
        animSettings.Play("OpenSettingsAnim");
    }
    public void closeSettings()
    {
        animSettings.Play("CloseSettingMenu");
    }

    public void SetResolution()
    {
        int currentIndex = dropdown.value;
        PlayerPrefs.SetInt("widthRes", resolutions[currentIndex].width);
        PlayerPrefs.SetInt("heightRes", resolutions[currentIndex].height);
        Screen.SetResolution(resolutions[currentIndex].width, resolutions[currentIndex].height, Screen.fullScreen);
    }




}
