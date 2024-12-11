using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickandDrop : MonoBehaviour
{
    public GameObject WinPanel;
    public Transform playerCam;
    public Transform collectablePos;
    public LayerMask layername;
    public float objectDistance;

    private CollectAbleObjects collectAbleObjects;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(collectAbleObjects == null)
            {
                if (Physics.Raycast(playerCam.position, playerCam.forward, out RaycastHit raycasthit, objectDistance, layername))
                {
                    if (raycasthit.transform.TryGetComponent(out collectAbleObjects))
                    {
                        collectAbleObjects.Grab(collectablePos);
                        if(raycasthit.collider.CompareTag("Artifact"))
                        {
                            WinPanel.SetActive(true);
                            Invoke("LevelProgressToRoom",3f);
                        }
                    }
                }
            }
            else 
            {
                collectAbleObjects.Drop();
                collectAbleObjects = null;
            }       
        }
    }
    private void LevelProgressToRoom()
    {
        SceneManager.LoadScene("Practice_PCG");
    }
}
