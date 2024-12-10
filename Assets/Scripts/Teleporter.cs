using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Paired Portal")]
    public GameObject Pair;
    [Header("Custom Material")]
    public Material material;
    
    [Header("3d Objects")]
    public GameObject[] Objects;
    // Start is called before the first frame update
    void Start()
    {
        if(material != null)
        {
            for(int i = 0; i < Objects.Length; i++)
            {
                var renderer = Objects[i].GetComponent<Renderer>();
                renderer.material = material;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
    }
}
