using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject creditPanel;
    public GameObject settingPanel;
    public GameObject controlPanel;
    public GameObject audioPanel;

    public AudioSource audioSource;
    public Slider volumeSlider;

    public Text musicON;
    public Text musicOFF;
    public bool isMuted;

    public Text AudioBtn;
    public Text ControlBtn;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(true);
        creditPanel.SetActive(false);
        settingPanel.SetActive(false);
        controlPanel.SetActive(false);
        audioPanel.SetActive(false);

        if (PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 1);
            LoadVolume();
        }
        else
        {
            LoadVolume();
        }

        if (!PlayerPrefs.HasKey("Muted"))
        {
            PlayerPrefs.SetInt("Muted", 0);
            LoadMusic();
        }
        else
        {
            LoadMusic();
        }
        updateMusicIcon();
        AudioListener.pause = isMuted;
    }

    // Update is called once per frame
    void Update()
    {
        changeVolume();
    }

    public void Backbtn()
    {
        settingPanel.SetActive(false);
        creditPanel.SetActive(false);
    }

    public void newGamebtnclicked()
    {

    }

    public void Credits()
    {
        creditPanel.SetActive(true);
        settingPanel.SetActive(false);
        controlPanel.SetActive(false);
        audioPanel.SetActive(false);
    }

    public void Settings()
    {
        settingPanel.SetActive(true);
        audioPanel.SetActive(true);
        creditPanel.SetActive(false);
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void Controls()
    {
        controlPanel.SetActive(true);
        audioPanel.SetActive(false);
        ControlBtn.color = Color.red;
        AudioBtn.color = Color.white;
    }

    public void Audios()
    {
        audioPanel.SetActive(true);
        controlPanel.SetActive(false);
        AudioBtn.color = Color.red;
        ControlBtn.color = Color.white;
    }

    public void changeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        SaveVolume();
    }

    public void LoadVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }

    public void musicBtn()
    {
        if (isMuted == false)
        {
            isMuted = true;
            AudioListener.pause = true;
        }
        else
        {
            isMuted = false;
            AudioListener.pause = false;
        }
        SaveMusic();
        updateMusicIcon();
    }

     public void updateMusicIcon()
    {
        if(isMuted == false)
        {
            musicON.enabled = true;
            musicOFF.enabled = false;
        }
        else
        {
            musicON.enabled = false;
            musicOFF.enabled = true;
        }
    }

    public void LoadMusic()
    {
        isMuted = PlayerPrefs.GetInt("Muted") == 1;
    }

    public void SaveMusic()
    {
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
    }
}
