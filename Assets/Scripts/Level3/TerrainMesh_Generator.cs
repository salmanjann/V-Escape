using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainMesh_Generator : MonoBehaviour
{
    private Rigidbody rb;
    private MeshCollider collider;
    private Mesh mesh;
    public Material material;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2Int size;
    private Vector2[] UVs;

    private bool shape_created;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        shape_created = false;
        rb = this.AddComponent<Rigidbody>();
        collider = this.AddComponent<MeshCollider>();
        meshFilter = this.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        mesh.name = "Terrain";

        rb.isKinematic = true;
        rb.useGravity = false;
        collider.sharedMesh = mesh;
        collider.convex = false;
        
        meshRenderer = this.AddComponent<MeshRenderer>();
        
        // Assign a basic material (default Unity material)
        meshRenderer.material = material;

        Create_shape();
        Update_Mesh();
    }

    // Update is called once per frame
    void Update()
    {   

    }

    private void Create_shape()
    {
        if (shape_created)
            return;
        if (vertices == null)
            return;
        shape_created = true;

        triangles = new int[size.x * size.y * 6];

        int vert = 0;
        int tris = 0;

        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                // Reverse triangle order for proper front-face rendering
                triangles[tris + 0] = vert + 1;
                triangles[tris + 1] = vert + size.x + 1;
                triangles[tris + 2] = vert + 0;
                triangles[tris + 3] = vert + size.x + 2;
                triangles[tris + 4] = vert + size.x + 1;
                triangles[tris + 5] = vert + 1;

                vert++;
                tris += 6;
            }
            vert++;
        }
        UVs = new Vector2[vertices.Length];
        for(int i = 0; i <= size.x; i++)
        {
            for(int j = 0; j <= size.y; j++)
            {
                UVs[i] = new Vector2((float)i / size.x, (float)j / size.y);
            }
        }
    }

    private void Update_Mesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        // mesh.uv = UVs;
        mesh.RecalculateNormals();
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
