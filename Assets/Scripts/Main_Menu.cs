using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    public Animator loadin_Animator;
    public RectTransform loadpannel;
    public GameObject mainMenu;
    public GameObject creditPanel;
    public GameObject settingPanel;
    public GameObject controlPanel;
    public GameObject audioPanel;

    public AudioSource audioSource;
    public Slider volumeSlider;

    public Text music;
    public bool isMuted;

    public Text AudioBtn;
    public Text ControlBtn;
    public Text SettingBtn;
    public Text CreditBtn;
    public Text newGameBtn;
    public Text exitGameBtn;

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
        CreditBtn.color = Color.white;
        SettingBtn.color = Color.white;
    }

    public void newGamebtnclicked()
    {
        newGameBtn.color = Color.red;
        CreditBtn.color = Color.white;
        SettingBtn.color = Color.white;
        exitGameBtn.color = Color.white;
        loadin_Animator.SetTrigger("SlideIn");
        Invoke("startLoadingIntro",1f);
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
        settingPanel.SetActive(false);
        controlPanel.SetActive(false);
        audioPanel.SetActive(false);
        CreditBtn.color = Color.red;
        SettingBtn.color = Color.white;
        newGameBtn.color = Color.white;
        exitGameBtn.color = Color.white;
    }

    public void Settings()
    {
        settingPanel.SetActive(true);
        audioPanel.SetActive(true);
        creditPanel.SetActive(false);
        SettingBtn.color = Color.red;
        CreditBtn.color = Color.white;
        newGameBtn.color = Color.white;
        exitGameBtn.color = Color.white;
    }

    public void exitGame()
    {
        exitGameBtn.color = Color.red;
        newGameBtn.color = Color.white;
        CreditBtn.color = Color.white;
        SettingBtn.color = Color.white;
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
