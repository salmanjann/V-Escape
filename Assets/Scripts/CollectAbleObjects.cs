using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectAbleObjects : MonoBehaviour
{
    private Rigidbody objectRB;
    private Transform grabPos;

    private void Awake()
    {
        objectRB = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (grabPos != null)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position,  grabPos.position, Time.deltaTime * lerpSpeed);
            objectRB.MovePosition(newPosition);
        }
    }

    public void Grab(Transform collectablePos)
    {
        this.grabPos = collectablePos;
       objectRB.useGravity = false;   
    }

    public void Drop()
    {
        this.grabPos = null;
        objectRB.useGravity = true;
    }
}
