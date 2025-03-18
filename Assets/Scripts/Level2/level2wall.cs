using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class level2wall : MonoBehaviour
{
    private List<Vector2Int> parents = new List<Vector2Int>();
    public bool Should_remove(Vector2Int parent, Vector2Int child)
    {
        return (parents.Contains(parent) && parents.Contains(child));
    }
    public void Is_edge(Vector2Int size)
    {
        foreach(Vector2Int parent in parents)
        {
            if(parent.x < 0 || parent.x >= size.x)
            {
                parents.Clear();
                return;
            }
            if(parent.y < 0 || parent.y >= size.y)
            {
                parents.Clear();
                return;
            }
        }
        
    }
    public void Add_parent(Vector2Int parent)
    {
        parents.Add(parent);
    }
}
