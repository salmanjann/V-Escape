using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private float delay;
    private string nextScene;
    public Animator loadin_Animator;
    public RectTransform loadpannel;
    public string sceneName;
    public GameObject camera;
    public GameObject canvas;
    public Text mainMenu;
    public Text retry;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MainMenu()
    {
        mainMenu.color = Color.red;
        retry.color = Color.white;
        loadin_Animator.SetTrigger("SlideIn");
        nextScene = "Main_Menu";
        delay = 0;

        Invoke("startLoadingIntro",1f);
    }

    public void Retry()
    {
        retry.color = Color.red;
        mainMenu.color = Color.white;
        loadin_Animator.SetTrigger("SlideIn");
        nextScene = sceneName;
        delay = 1;

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
                loadScreen.previous = "GameOver";
                loadScreen.next = nextScene;
                loadScreen.delay = delay;
            }
        }
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe after handling
    }
}
