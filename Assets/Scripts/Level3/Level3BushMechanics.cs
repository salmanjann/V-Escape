using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3BushMechanics : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.transform.position;
        this.transform.position = new Vector3(pos.x,pos.y - 1f*Time.deltaTime,pos.z);
    }
    void OnTriggerEnter(Collider collision)
    {
      Destroy(this.GetComponent<BoxCollider>());  
      Destroy(this.GetComponent<Level3BushMechanics>());  
    }
}
