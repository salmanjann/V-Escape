using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyRing : MonoBehaviour
{
    public int currentFloor; // Assign this when spawning

    private void OnDestroy()
    {
        // Find Proc_Gen in the scene and update key count
        Proc_Gen procGen = FindObjectOfType<Proc_Gen>();
        if (procGen != null)
        {
            procGen.CollectKey(currentFloor);
        }
    }
}
