using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main_Menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject controls;
    public GameObject settings;

    public Slider volumeSlider;



    public Text sound;
    private bool isMuted;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
        controls.SetActive(false);

        if (PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 1);
            loadVoulme();
        }
        else
        {
            loadVoulme();
        }

        if (!PlayerPrefs.HasKey("Muted"))
        {
            PlayerPrefs.SetInt("Muted", 0);
            loadSound();
        }
        else
        {
            loadSound();
        }
        updateSoundText();
        AudioListener.pause = isMuted;
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

    public void backbtn()
    {
        settings.SetActive(false);
        controls.SetActive(false);
    }

    public void playbtnclicked()
    {
        SceneManager.LoadScene("Labyrinth");
        // Debug.Log("Play");
    }

    public void exitbtnclicked()
    {
        Application.Quit();
    }
    
    public void changeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        saveVolume();
    }

    public void loadVoulme()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
    }

    public void saveVolume()
    {
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
    }

    public void soundBtn()
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
        saveSound();
        updateSoundText();
    }

    public void loadSound()
    {
        isMuted = PlayerPrefs.GetInt("Muted") == 1;
    }

    public void saveSound()
    {
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
    }

    public void updateSoundText()
    {
        if (isMuted == false)
        {
            sound.text = "ON";
        }
        else
        {
            sound.text = "OFF";
        }
    }
}
