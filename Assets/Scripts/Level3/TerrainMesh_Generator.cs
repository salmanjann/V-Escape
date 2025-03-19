using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainMesh_Generator : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Vector3[] vertices;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = this.AddComponent<MeshFilter>();
        meshRenderer = this.AddComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnDrawGizmos()
    {
        if(vertices != null)
        {
            foreach(Vector3 vertex in vertices)
            {
                Gizmos.DrawSphere(vertex+this.transform.position,0.1f);
            }
        }
    }
}
