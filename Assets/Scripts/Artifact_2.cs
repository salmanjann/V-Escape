using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact_2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // rotate();
    }
    private void rotate()
    {
        float rotation_speed = 1f;
        transform.Rotate(new Vector3(rotation_speed/4 * Time.deltaTime, rotation_speed/2 * Time.deltaTime, rotation_speed * Time.deltaTime));
        
    }
    private void OnTriggerEnter(Collider obj)
    {
        if(obj.CompareTag("Wall"))
        {
            var temp = this.transform.position;
            this.transform.position = new Vector3(temp.x + 1f, temp.y, temp.z + 1f);
        }
    }
}
