using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Forest_Generator : MonoBehaviour
{
    public Material material;
    Vector3[,] vertices;
    public Vector2Int size;
    public Vector2Int objArrayCount;
    private Vector2Int terrain_size;
    private List<GameObject> forests;
    // Start is called before the first frame update
    void Start()
    {
        terrain_size = new Vector2Int(size.x + 1 + ((size.x) * (objArrayCount.x - 1)), size.y + 1  + ((size.y) * (objArrayCount.y - 1)));
        vertices = new Vector3[terrain_size.x,terrain_size.y];
        forests = new List<GameObject>();
        Create_Vertices();
        Create_All_Terrain_Objects();
        vertices = null;
    }

    private void Create_Vertices()
    {
        for(int i = 0; i < terrain_size.x; i++)
        {
            for(int j = 0; j < terrain_size.y; j++)
            {
                float y = Mathf.PerlinNoise(i * 0.3f,j * 0.3f);
                vertices[i,j] = new Vector3(i,y,j);
            }
        }
    }

    private void Create_All_Terrain_Objects()
    {
        for(int i = 0; i < objArrayCount.x; i++)
        {
            for(int j = 0; j < objArrayCount.y; j++)
            {
                Create_Terrain_Object(i, j);
            }
        }
    }

    private void Create_Terrain_Object(int x, int y)
    {
        GameObject parent = new GameObject("Forest_" + (x * objArrayCount.y + y).ToSafeString());
        parent.transform.parent = this.transform;
        
        int iStart = x * size.x;
        int jStart = y * size.y;

        // +1 because the size defines the count of quads, so we need +1 vertices
        Vector3[] _vertices = new Vector3[(size.x + 1) * (size.y + 1)];

        int index = 0;
        for (int i = iStart; i <= iStart + size.x; i++)
        {
            for (int j = jStart; j <= jStart + size.y; j++)
            {
                _vertices[index++] = vertices[i, j];
            }
        }
        
        GameObject obj = new GameObject(parent.name + "_LOD0");
        obj.transform.parent = parent.transform;
        TerrainMesh_Generator component = obj.AddComponent<TerrainMesh_Generator>();

        component.vertices = _vertices;
        component.size = size;
        component.material = material;
    }

    public void OnDrawGizmos()
    {
        // if(vertices != null)
        // {
        //     for(int i = 0; i < terrain_size.x; i++)
        //     {
        //         for(int j = 0; j < terrain_size.y; j++)
        //         {
        //             Gizmos.DrawSphere(vertices[i,j],0.1f);
        //         }
        //     }
        // }
    }
}
