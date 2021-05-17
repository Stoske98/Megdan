using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsControll : MonoBehaviour
{
    [Header("Sound Volume")]
    public Slider audioSlider;
    public AudioSource[] audioSource;
    public Text audioText;

    [Header("Effects Volume")]
    public Slider audioSliderEff;
    public AudioSource[] audioSourceEff;
    public Text audioTextEff;

    [Header("SettingsAnim")]
    public Animator animSettings;
    public Toggle checkFullScreen;
    public Dropdown dropdown;
    public GameObject gamePanel;

    private Resolution[] resolutions;
    private List<string> resol;

    [Header("GameSettings")]
    public Slider audioSliderG;
    public Text audioTextG;
    public Slider audioSliderEffG;
    public Text audioTextEffG;
    public Toggle checkFullScreenG;
    public Dropdown dropdownG;



    #region GameManager Singleton
    private static SettingsControll _instance;

    public static SettingsControll Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        
    }
    #endregion


    private void Start()
    {
        


        foreach (AudioSource audio in audioSource)
        {
            audio.volume = PlayerPrefs.GetFloat("volume");
        }
       // audioSource.volume = 1;
        audioSlider.value = PlayerPrefs.GetFloat("volume");
        audioText.text = "Sound Volume: " + (int)(audioSlider.value * 100);

        foreach (AudioSource audio in audioSourceEff)
        {
            audio.volume = PlayerPrefs.GetFloat("volumeEff");
        }
        //audioSourceEff.volume = 1;
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


    public void SetAudio()
    {
        foreach (var audio in audioSource)
        {
            audio.volume = audioSlider.value;

        }
        audioText.text = "Sound Volume: " + (int)(audioSlider.value * 100);
    }

    public void SetAudioEff()
    {
        foreach (var audio in audioSourceEff)
        {
            audio.volume = audioSliderEff.value;

        }
       
        audioTextEff.text = "Effects Volume: " + (int)(audioSliderEff.value * 100);

    }

    

    public void clickOnButton()
    {
        audioSourceEff[0].Play();
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

    public void PauseGame()
    {
        gamePanel.SetActive(true);
        Time.timeScale = 0;
        
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        gamePanel.SetActive(false);
    }

    public void draftToGame()
    {
      audioSliderG.value = audioSlider.value;
      audioSlider = audioSliderG;

      audioTextG.text = audioText.text.ToString();
      audioText = audioTextG;

      audioSliderEffG.value = audioSliderEff.value;
        audioSliderEff = audioSliderEffG;

      audioTextEffG.text = audioTextEff.text.ToString();
        audioTextEff = audioTextEffG;

        if (checkFullScreen.isOn)
        {
            checkFullScreenG.isOn = true;
        }
        else
        {
            checkFullScreenG.isOn = false;

        }
        checkFullScreen = checkFullScreenG;

      dropdownG.options = dropdown.options;
        dropdownG.value = dropdown.value;
        dropdown = dropdownG;
}

    


}
