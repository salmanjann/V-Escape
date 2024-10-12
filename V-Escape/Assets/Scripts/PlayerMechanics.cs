using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMechanics : MonoBehaviour
{
    public GameObject Camera;
    private GameObject MergedBody;
    // Start is called before the first frame update
    void Start()
    {
        // First make the camera and the Player cobine into the same object so that they can be syncronized
        MergedBody = new GameObject("Player Entity");
        this.transform.SetParent(MergedBody.transform);
        Camera.transform.SetParent(MergedBody.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Walk()
    {
        // make sure the forward backward go in same direction player is facing and the side ways movement is handled accordingly
    }

    private void Turn_Around()
    {
        // make rotation upon the whole game object accordingly
    }
}
