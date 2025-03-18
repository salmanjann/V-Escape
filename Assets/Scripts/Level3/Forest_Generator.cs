using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Forest_Generator : MonoBehaviour
{
    private Mesh[] meshes;
    private GameObject forest;
    Vector3[] vertices;
    int[] triangles;
    public Vector2Int size;
    // Start is called before the first frame update
    void Start()
    {
        meshes = new Mesh[2];
        generate_Mesh(0);
        generate_Mesh(1);
        this.AddComponent<LODGroup>();

        create_shape();
        update_mesh();
    }

    private void generate_Mesh(int x)
    {
        
        forest = new GameObject("forest_LOD"+x.ToString());
        forest.transform.parent = this.transform;
        forest.AddComponent<MeshRenderer>();
        forest.AddComponent<MeshFilter>();
        forest.GetComponent<MeshFilter>().mesh = meshes[x];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void create_shape()
    {
        vertices = new Vector3[(size.x + 1) * (size.y + 1)];
        for(int x = 0, i = 0; x <= size.x; x++)
        {
            for(int z = 0; z <= size.y; z++, i++)
            {
                vertices[i] = new Vector3(x,0,z);
            }
        }
    }
    private void update_mesh()
    {
        for(int i = 0; i < meshes.Length; i++)
        {
            GameObject temp = GameObject.Find("forest_LOD"+i.ToString());
            var x = temp.GetComponent<MeshFilter>();
            x.mesh = meshes[i];
            meshes[i].Clear();

            meshes[i].vertices = vertices;
            meshes[i].triangles = triangles;

            meshes[i].RecalculateNormals();
            x.mesh = meshes[i];

        }
    }

    private void OnDrawGizmos()
    {
        if(vertices == null)
            return;
        for(int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i],0.1f);
        }  
    }

}
