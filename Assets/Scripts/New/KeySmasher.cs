using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySmasher : MonoBehaviour
{
    [SerializeField] private Proc_Gen proc_GenRef;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Y)){
            proc_GenRef.CollectKey(proc_GenRef.currentFloor-1,true);
        }

        if(Input.GetKey(KeyCode.U)){
            proc_GenRef.MovePlayerToGround();
        }
    }
}
