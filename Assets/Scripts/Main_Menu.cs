using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Main_Menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject controls;
    public GameObject settings;

    /*public Slider volumeSlider;

    public AudioSource audioSource;
    public AudioClip background;*/

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        controls.SetActive(false);
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

    public void settingbtn()
    {
        settings.SetActive(true);
        controls.SetActive(false);
    }

    public void controlsbtn()
    {
        controls.SetActive(true);
        settings.SetActive(false);
    }

    public void backtbn()
    {
        settings.SetActive(false);
        controls.SetActive(false);
    }

    public void playtbtnclicked()
    {
        //SceneManager.LoadScene(1);
        Debug.Log("Play");
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
