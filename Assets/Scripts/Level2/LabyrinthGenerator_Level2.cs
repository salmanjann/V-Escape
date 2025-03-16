using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class LabyrinthGenerator_Level2 : MonoBehaviour
{
    public GameObject wall_prefab;
    public GameObject ground_prefab;
    public int seed;
    public bool random_seed;
    public Vector2Int size;
    public int RandomTravel;
    private List<GameObject> walls = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        seed_manage();
        wallplacer();
        groundplacer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void seed_manage()
    {
        if(random_seed)
        {
            seed = UnityEngine.Random.Range(0,(int)Mathf.Pow(2,30));
        }
        UnityEngine.Random.InitState(seed);
    }

    private class parentchildNode
    {
        public Vector2Int parent;
        public Vector2Int child;
        public parentchildNode(Vector2Int parent_, Vector2Int child_)
        {
            parent = parent_;
            child = child_;
        }
    }

    private void wallplacer()
    {
        List<level2wall> components = new List<level2wall>();
        for(int i = 0; i < size.x; i++)
        {
            for(int j = 0; j <= size.y; j++)
            {
                GameObject wall = Instantiate(wall_prefab);
                wall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                wall.transform.position = new Vector3(3f * i + 1.5f, 0f, 0f - 1.5f + 3f * j);
                level2wall component = wall.GetComponent<level2wall>();
                component.Add_parent(new Vector2Int(i, j - 1));
                component.Add_parent(new Vector2Int(i, j));
                component.Is_edge(size);
                wall.transform.parent = this.gameObject.transform;
                // wall.isStatic = true;
                walls.Add(wall);
                components.Add(component);
            }
        }
        for(int i = 0; i < size.y; i++)
        {
            for(int j = 0; j <= size.x; j++)
            {
                GameObject wall = Instantiate(wall_prefab);
                wall.transform.position = new Vector3(0f + 3f * j, 0f, 3f * i);
                level2wall component = wall.GetComponent<level2wall>();
                component.Add_parent(new Vector2Int(j - 1, i));
                component.Add_parent(new Vector2Int(j, i));
                component.Is_edge(size);                
                wall.transform.parent = this.gameObject.transform;
                // wall.isStatic = true;
                walls.Add(wall);
                components.Add(component);
            }
        }

        List<Vector2Int> possible = new List<Vector2Int>();
        for(int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
            {
                possible.Add(new Vector2Int(i,j));
            }
        }

        possible.Remove(new Vector2Int(0,0));
        List<int> to_be_removed = new List<int>();
        List<parentchildNode> parent_child_list = MakePath(new Vector2Int(0,0), new Vector2Int(0,0), possible);
        for(int i = components.Count-1; i >= 0; i--)
        {
            for(int j = 0; j < parent_child_list.Count; j++)
            {
                if(components[i].Should_remove(parent_child_list[j].parent, parent_child_list[j].child))
                {
                    to_be_removed.Add(i);
                    break;
                }
            }
        }

        foreach(int to_be_removed_index in to_be_removed)
        {
            GameObject wall = walls[to_be_removed_index];
            walls.RemoveAt(to_be_removed_index);
            components.RemoveAt(to_be_removed_index);
            Destroy(wall);
        }
    }

    private List<parentchildNode> MakePath(Vector2Int parent, Vector2Int current, List<Vector2Int> possible)
    {
        List<parentchildNode> list = new List<parentchildNode>();
        if(possible.Count == 0)
        {
            parentchildNode node = new parentchildNode(parent,current);
            list.Add(node);
            return list;
        }
        List<Vector2Int> Moves = new List<Vector2Int>();
        for(int i = -1; i <= 1; i += 2)
        {
            for(int j = 0; j <= 1; j++)
            {
                int x_ = 0, y_ = 0;
                if(j == 0)
                {
                    x_ = i;
                }
                else
                {
                    y_ = i;
                }
                Vector2Int newMove = current + new Vector2Int(x_,y_);
                if(newMove.x < 0 || newMove.x >= size.x)
                {
                    continue;
                }
                if(newMove.y < 0 || newMove.y >= size.y)
                {
                    continue;
                }
                Moves.Add(newMove);
            }
        }
        while(Moves.Count > 0)
        {
            int index = UnityEngine.Random.Range(0,Moves.Count);
            Vector2Int move_made = Moves[index];
            Moves.RemoveAt(index);
            if(possible.Contains(move_made))
            {
                possible.Remove(move_made);
                List<parentchildNode> path = MakePath(current, move_made,possible);
                list.AddRange(path);
            }
            else if(!possible.Contains(move_made) && UnityEngine.Random.Range(0,100) < RandomTravel)
            {
                list.Add(new parentchildNode(current,move_made));
            }
        }
        list.Add(new parentchildNode(parent,current));
        return list;
    }

    private void groundplacer()
    {
        for(int i = 0;i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
            {
                GameObject ground = Instantiate(ground_prefab);
                ground.transform.position = new Vector3(1.9f + 4 * i, 0f, 4f * j);
            }
        }
    }
   }
