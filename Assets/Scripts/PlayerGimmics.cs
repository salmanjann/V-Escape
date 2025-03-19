using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGimmics : MonoBehaviour
{
    private Vector3 temp_coordinates;
    private bool CanTeleport;
    // Start is called before the first frame update
    void Start()
    {
        CanTeleport = true;
    }

    public void TeleportActivation(Vector3 coordinates)
    {
        if(!CanTeleport)
        {
            return;
        }
        temp_coordinates = coordinates;
        // add animation for teleport on canvas
        // make the teleportation happen after a seconds time
        Invoke("Teleport",1f);
    }
    private void Teleport()
    {
        // change coordinates to teleported location
        transform.position = temp_coordinates;
        CanTeleport = false;
        Invoke("AllowTeleport",5f);
    }
    private void AllowTeleport()
    {
        CanTeleport = true;
    }
}
