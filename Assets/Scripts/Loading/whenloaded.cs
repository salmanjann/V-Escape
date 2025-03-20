using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whenloaded : MonoBehaviour
{
    public Animator loadin_Animator;
    // Start is called before the first frame update
    void Start()
    {
        loadin_Animator.SetTrigger("SlideOut");
    }
}
