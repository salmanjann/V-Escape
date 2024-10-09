using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
    public int seed;
    public bool useCustomSeed;
    public GameObject Hall_ground;
    public GameObject Wall;

    // Start is called before the first frame update
    void Start()
    {
        int canvas_size = 80;
        int hall_size = 20;
        HashSet<int> Hall_ground_set = new HashSet<int>();
        HashSet<int> Wall_set = new HashSet<int>();
        int Items_counter = 1;

        if(!useCustomSeed)
        {
            seed = (int)System.DateTime.Now.Ticks;
        }
        System.Random random = new System.Random(seed);

        int[,] array = new int[canvas_size, canvas_size];
        for(int i = 0; i < canvas_size; i++)
            for(int j = 0; j < canvas_size; j++)
                array[i,j] = 0; 

        Vector2 start_hall = new Vector2(random.Next(canvas_size-hall_size-1),random.Next(canvas_size-hall_size-1));
        Vector2 size = new Vector2(random.Next(hall_size/8,hall_size/2)*2 + 1,random.Next(hall_size/8,hall_size/2)*2 + 1);

        for(int i = (int)start_hall.x; i < start_hall.x + size.x; i++)
            for(int j = (int)start_hall.y; j < (int)start_hall.y + size.y; j++)
                array[i,j] = Items_counter; 
        Hall_ground_set.Add(Items_counter);
        Items_counter++;

        for(int i = (int)start_hall.x; i < start_hall.x + size.x; i++)
        {
            array[i,(int)start_hall.y] = Items_counter;
            array[i,(int)start_hall.y + (int)size.y] = Items_counter;
        }
        for(int i = (int)start_hall.y; i < start_hall.y + size.y; i++)
        {
            array[(int)start_hall.x,i] = Items_counter;
            array[(int)start_hall.x + (int)size.x,i] = Items_counter;
        }
        Wall_set.Add(Items_counter);
        Items_counter++;

        int counter = 0;

        GameObject Hall_Floor_Parent = new GameObject("Hall Floor");
        GameObject Wall_Parent = new GameObject("Wall");
        for(int i = 0; i < canvas_size; i++)
        {   
            for(int j = 0; j < canvas_size; j++)
                {
                    if(Hall_ground_set.Contains(array[i,j]))
                    {
                        var temp = Instantiate(Hall_ground);
                        temp.transform.position = new Vector3(i,0,j);
                        temp.transform.SetParent(Hall_Floor_Parent.transform);
                        counter++;
                    }
                    else if(Wall_set.Contains(array[i,j]))
                    {
                        var temp = Instantiate(Wall);
                        temp.transform.position = new Vector3(i,0,j);
                        temp.transform.SetParent(Wall_Parent.transform);
                        counter++;
                    }
                }
        }

        Debug.Log(counter.ToString());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
