using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

public class Pause_Panel : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;
            pausePanel.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;
            pausePanel.SetActive(false);
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        // loadpannel.position = new Vector3(0, loadpannel.position.y, loadpannel.position.z);
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
                Scene sc = this.gameObject.scene;
                LoadScreen loadScreen = temp.GetComponent<LoadScreen>();
                loadScreen.previous = sc.name;
                loadScreen.next = "Main_Menu";
                loadScreen.delay = 0;
            }
        }
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe after handling
    }

    public void Resume()
    {
        isPaused = !isPaused;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
}
