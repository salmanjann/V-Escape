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
        Update_Mesh();
    }

    // Update is called once per frame
    void Update()
    {
        // Create_shape();   
    }

    private void Create_shape()
    {
        if(shape_created)
            return;
        if(vertices == null)
            return;
        shape_created = true;

        triangles = new int[size.x * size.y * 6];
        
        int vert = 0;
        int tris = 0;

        
        for(int x = 0; x < size.x; x++)
        {
            for(int z = 0; z < size.y; z++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + size.x + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + size.x + 1;
                triangles[tris + 5] = vert + size.x + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }

    }

    private void Update_Mesh()
    {
        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.triangles = triangles;

        meshFilter.mesh.RecalculateNormals();
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
