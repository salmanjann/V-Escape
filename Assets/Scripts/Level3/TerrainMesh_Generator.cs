using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainMesh_Generator : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2Int size;

    private bool shape_created;

    // Start is called before the first frame update
    void Start()
    {
        shape_created = false;
        meshFilter = this.AddComponent<MeshFilter>();
        meshRenderer = this.AddComponent<MeshRenderer>();
        Create_shape();
    }

    // Update is called once per frame
    void Update()
    {
        Create_shape();   
    }

    private void Create_shape()
    {
        if(shape_created)
            return;
        if(vertices == null)
            return;
        shape_created = true;
        // int vert = 0;

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
