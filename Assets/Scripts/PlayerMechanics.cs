using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMechanics : MonoBehaviour
{
    public GameObject Camera;
    private GameObject MergedBody;
    private float playerSpeed;
    private Vector3 cameraDifference;
    // Start is called before the first frame update
    void Start()
    {
        // First make the camera and the Player cobine into the same object so that they can be syncronized
        MergedBody = new GameObject("Player Entity");
        this.transform.SetParent(MergedBody.transform);
        Camera.transform.SetParent(MergedBody.transform);
        // assigning standard speed to the player.
        playerSpeed = 4f;
        // calculate distance from camera to maintain it
        cameraDifference = this.transform.position - Camera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Maintain_Camera_Coordinates();
        Walk();
    }

    private void Walk()
    {
        // make sure the forward backward go in same direction player is facing and the side ways movement is handled accordingly
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            Vector3 movement = new Vector3(Mathf.Sin(MergedBody.transform.eulerAngles.y * Mathf.Deg2Rad) * playerSpeed, 0, Mathf.Cos(MergedBody.transform.eulerAngles.y * Mathf.Deg2Rad) * playerSpeed);
            if(Input.GetKey(KeyCode.S))
            {
                movement *= -1;
            }
            Vector3 currentPos = MergedBody.transform.position;
            currentPos += movement * Time.deltaTime;
            MergedBody.transform.position = currentPos;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            Vector3 movement = new Vector3(Mathf.Cos(MergedBody.transform.eulerAngles.y * Mathf.Deg2Rad) * playerSpeed, 0, Mathf.Sin(MergedBody.transform.eulerAngles.y * Mathf.Deg2Rad) * playerSpeed);
            if (Input.GetKey(KeyCode.A))
            {
                movement *= -1;
            }
            Vector3 currentPos = MergedBody.transform.position;
            currentPos += movement * Time.deltaTime;
            MergedBody.transform.position = currentPos;
        }
    }

    private void Turn_Around()
    {
        // make rotation upon the whole game object accordingly
    }

    private void Maintain_Camera_Coordinates()
    {
        Camera.transform.position = this.transform.position - cameraDifference;
    }
}
