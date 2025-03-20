using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickandDrop : MonoBehaviour
{
    public Transform playerCam;  
    public Transform collectablePos;  
    public LayerMask collectableLayer; 
    public float objectDistance;

    public GameObject winPanel;

    private CollectAbleObjects collectAbleObjects;  // Reference to currently held object
    private Player_Movement playerMov;
    void Start()
    {
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

    private void nextLevel()
    {
        Debug.Log("Next Level");
    }
}
