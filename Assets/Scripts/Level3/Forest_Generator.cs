using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Forest_Generator : MonoBehaviour
{
    public List<GameObject> Rocks_prefabs;
    public GameObject Tree_prefab;
    public GameObject Small_Bush_Prefab;
    public Material material;
    Vector3[,] vertices;
    public Vector2Int size;
    public Vector2Int objArrayCount;
    private Vector2Int terrain_size;
    private List<GameObject> forests;
    List<GameObject> spawned;
    // Start is called before the first frame update
    void Start()
    {
        terrain_size = new Vector2Int(size.x + 1 + ((size.x) * (objArrayCount.x - 1)), size.y + 1  + ((size.y) * (objArrayCount.y - 1)));
        vertices = new Vector3[terrain_size.x,terrain_size.y];
        forests = new List<GameObject>();
        Create_Vertices();
        Create_All_Terrain_Objects();
        vertices = null;
        Objects_Spawner();
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

    private void Objects_Spawner()
    {
        spawned = new List<GameObject>();
        Spawn_Trees();
        Spawn_Rocks();
        Spawn_Bushes();
    }

    private void Spawn_Trees()
    {
        GameObject parent = new GameObject("Trees");
        int number_of_objects = (terrain_size.x * terrain_size.y) / (7 * 7);
        for(int i = 0; i < number_of_objects; i++)
        {
            GameObject obj = Instantiate(Tree_prefab);
            obj.transform.Rotate(0f, UnityEngine.Random.Range(0f, 359.9f), 0f);
            float size = UnityEngine.Random.Range(0.4f,1f);
            obj.transform.localScale = new Vector3(size, size, size);
            obj.transform.parent = parent.transform;
            int tries = 0;
            do
            {
                if(tries > 100)
                {
                    Destroy(obj);
                    break;
                }
                Vector3 pos = Vector3.zero;
                pos.x = UnityEngine.Random.Range(0 + 5f, terrain_size.x-5f);
                pos.y = 1f;
                pos.z = UnityEngine.Random.Range(0 + 5f, terrain_size.y-5f);
                obj.transform.position = pos;
            }while(!is_Far_Enough(obj,spawned, 2.5f * size));
            spawned.Add(obj);
        }
    }

    private void Spawn_Rocks()
    {
        GameObject parent = new GameObject("Rocks");
        int number_of_objects = (terrain_size.x * terrain_size.y) / (30 * 30);
        for(int i = 0; i < number_of_objects; i++)
        {
            int choice = (int)UnityEngine.Random.Range(0,Rocks_prefabs.Count - 0.1f);
            GameObject obj = Instantiate(Rocks_prefabs[choice]);
            obj.transform.Rotate(0f, UnityEngine.Random.Range(0f, 359.9f), 0f);
            float size = UnityEngine.Random.Range(0.05f,1f);
            obj.transform.localScale = new Vector3(size, size, size);
            obj.transform.parent = parent.transform;
            int tries = 0;
            do
            {
                if(tries > 5)
                {
                    Destroy(obj);
                    break;
                }
                Vector3 pos = Vector3.zero;
                pos.x = UnityEngine.Random.Range(0 + 5f, terrain_size.x-5f);
                pos.y = 1f;
                pos.z = UnityEngine.Random.Range(0 + 5f, terrain_size.y-5f);
                obj.transform.position = pos;
                tries++;
            }while(!is_Far_Enough(obj,spawned, 5.5f * size));
            spawned.Add(obj);
        }
    }

    private void Spawn_Bushes()
    {
        GameObject parent = new GameObject("Bushes");
        int number_of_objects = (terrain_size.x * terrain_size.y) / (2 * 2);
        for(int i = 0; i < number_of_objects; i++)
        {
            GameObject obj = Instantiate(Small_Bush_Prefab);
            obj.transform.Rotate(0f, UnityEngine.Random.Range(0f, 359.9f), 0f);
            float size = UnityEngine.Random.Range(0.2f,5f);
            obj.transform.localScale = new Vector3(size, size, size);
            obj.transform.parent = parent.transform;
            int tries = 0;
            do
            {
                if(tries > 100)
                {
                    Destroy(obj);
                    break;
                }
                Vector3 pos = Vector3.zero;
                pos.x = UnityEngine.Random.Range(0 + 5f, terrain_size.x-5f);
                pos.y = 1f;
                pos.z = UnityEngine.Random.Range(0 + 5f, terrain_size.y-5f);
                obj.transform.position = pos;
            }while(!is_Far_Enough(obj,spawned, 0.5f * size));
            spawned.Add(obj);
        }
    }

    private bool is_Far_Enough(GameObject obj, List<GameObject> objs, float min_distance)
    {
        if(obj==null)
            return true;
        if(objs.Count==0)
            return true;

        foreach(GameObject temp in objs)
        {
            if(Vector3.Distance(obj.transform.position, temp.transform.position) <= min_distance)
                return false;
        }

        return true;
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
