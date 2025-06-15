using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    private bool start;

    [Header("Loading Screen")]
    public Animator loadin_Animator;
    public RectTransform loadpannel;

    [Header("Game Panels")]
    public GameObject startPanel;
    public GameObject mainMenu;
    public GameObject creditPanel;
    public GameObject settingPanel;
    public GameObject controlPanel;
    public GameObject audioPanel;

    [Header("Audios")]
    public AudioSource audioSource;
    public Slider volumeSlider;

    [Header("Texts")]
    private bool isMuted;
    public Text music;
    public Text AudioBtn;
    public Text ControlBtn;
    public Text newGameBtn;
    public Text exitGameBtn;

    void Start()
    {
        start = false;
        startPanel.SetActive(true);
        mainMenu.SetActive(false);
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

    void Update()
    {
        startPress();
        changeVolume();
    }

    private void startPress()
    {
        if(start)
            return;
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            start = true;
            Startbtn();
        }
    }

    public void Backbtn()
    {
        settingPanel.SetActive(false);
        creditPanel.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void newGamebtnclicked()
    {
        newGameBtn.color = Color.red;
        exitGameBtn.color = Color.white;
        loadin_Animator.SetTrigger("SlideIn");
        Invoke("startLoadingIntro", 1f);
    }

    public void Startbtn()
    {
        startPanel.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void startLoadingIntro()
    {
        loadpannel.position = new Vector3(0, loadpannel.position.y, loadpannel.position.z);
        SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Loading")
        {
            GameObject temp = GameObject.Find("EventSystemLoading");
            if (temp != null)
            {
                LoadScreen loadScreen = temp.GetComponent<LoadScreen>();
                loadScreen.previous = "Main_Menu";
                loadScreen.next = "Intro";
                loadScreen.delay = 0;
            }
        }
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe after handling
    }

    public void Credits()
    {
        creditPanel.SetActive(true);
        mainMenu.SetActive(false);
        settingPanel.SetActive(false);
        exitGameBtn.color = Color.white;
    }

    public void Settings()
    {
        settingPanel.SetActive(true);
        audioPanel.SetActive(true);
        mainMenu.SetActive(false);
        creditPanel.SetActive(false);
        exitGameBtn.color = Color.white;
    }

    public void exitGame()
    {
        exitGameBtn.color = Color.red;
        Invoke("Quit", 1f);
    }

    public void Quit()
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
        PlayerPrefs.SetFloat("Volume",volumeSlider.value);
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
            music.text = "ON";
            music.color = Color.red;
        }
        else
        {
            music.text = "OFF";
            music.color = Color.white;
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
