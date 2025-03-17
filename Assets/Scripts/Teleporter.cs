using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Paired Teleporter")]
    public GameObject Pair;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void DeleteConditions()
    {
        if(transform.position.y<10f)
        {
            Destroy(Pair);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider obj)
    {
        if(obj.CompareTag("Player"))
        {
            // add functionality for the teleportation from player
            var player_gimmics = obj.GetComponent<PlayerGimmics>();
            var position = Pair.transform.position;
            var coordinates = new Vector3(position.x,obj.transform.position.y,position.z);
            player_gimmics.TeleportActivation(coordinates);
        }
        else if(obj.CompareTag("Wall"))
        {
            var temp = this.transform.position;
            this.transform.position = new Vector3(temp.x + 1f, temp.y, temp.z + 1f);
        }
    }
}
