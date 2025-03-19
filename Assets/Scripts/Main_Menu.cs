using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Main_Menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject creditPanel;
    public GameObject settingPanel;
    public GameObject controlPanel;
    public GameObject audioPanel;
    /*
    public Slider volumeSlider;

    public AudioSource audioSource;
    public AudioClip background;*/

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(true);
        creditPanel.SetActive(false);
        settingPanel.SetActive(false);
        controlPanel.SetActive(false);
        audioPanel.SetActive(false);
        /*
        if (PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 1);
            Load();
        }
        else
        {
            Load();
        }
        audioSource.clip = background;
        audioSource.Play();*/
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }

    public void Audios()
    {
        audioPanel.SetActive(true);
        controlPanel.SetActive(false);
    }

    /*
    public void changeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }

    public void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }

    public void playButton()
    {
        audioSource.Play();
    }*/
}
