using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickandDrop : MonoBehaviour
{

    public Animator loadin_Animator;
    public RectTransform loadpannel;
    public int level;
    public Transform playerCam;  
    public Transform collectablePos;  
    public LayerMask collectableLayer; 
    public float objectDistance;

    public GameObject winPanel;

    private CollectAbleObjects collectAbleObjects;  // Reference to currently held object
    private Player_Movement playerMov;
    void Start()
    {
        Player_Movement player_Movement = this.GetComponent<Player_Movement>();
        loadin_Animator = player_Movement.loadin_Animator;
        loadpannel = player_Movement.loadpannel;
        winPanel.gameObject.SetActive(false);
        playerMov = this.GetComponent<Player_Movement>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Check if holding an object
            if (collectAbleObjects != null)
            {
                // Drop the held object
                collectAbleObjects.Drop();
                collectAbleObjects = null;
            }
            else
            {
                // Raycast to detect objects
                if (Physics.Raycast(playerCam.position, playerCam.forward, out RaycastHit hit, objectDistance))
                {
                    GameObject hitObject = hit.transform.gameObject;

                    // If the object has tag "Flash", delete it
                    if (hitObject.CompareTag("Flash"))
                    {
                        Destroy(hitObject);
                        playerMov.increaseFlash(); 
                        return;  // Stop further execution
                    }

                    if (hitObject.CompareTag("Artifact"))
                    {
                        Destroy(hitObject);
                        winPanel.gameObject.SetActive(true);
                        Invoke("nextLevel", 1f);
                        return;  // Stop further execution
                    }

                    // If the object is on the Collectable layer, pick it up
                    if (((1 << hitObject.layer) & collectableLayer) != 0)
                    {
                        if (hitObject.TryGetComponent(out collectAbleObjects))
                        {
                            collectAbleObjects.Grab(collectablePos);
                        }
                    }
                }
            }
        }
    }

    private string nextscene()
    {
        switch(level)
        {
            case 1:
            return "Labyrinth";
            case 2:
            return "Forest";
            default:
            return "Main_Menu";
        }
    }

    private void nextLevel()
    {
        Debug.Log("Next Level");
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
                loadScreen.previous = this.gameObject.scene.name;
                loadScreen.next = nextscene();
                loadScreen.delay = 1;
            }
        }
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe after handling
    }
}
