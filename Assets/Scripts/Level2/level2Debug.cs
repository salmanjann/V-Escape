using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class level2Debug : MonoBehaviour
{
    public GameObject player;
    public GameObject artifact;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            GoToArtifact();
        }
    }

    private void GoToArtifact()
    {
        Vector3 position = artifact.transform.position;
        position.y += 2f;
        player.transform.position = position;
    }
}
