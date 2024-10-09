using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
    // This will be the random seed used by the Generator 
    public int seed;
    // In case the user doesnt want the seed to be custom one and instead wants a random seed 
    public bool useCustomSeed;

    // The Prefabs 
    public GameObject Hall_ground;
    public GameObject Wall;

    // Start is called before the first frame update
    void Start()
    {
        // Make an empty canvas on which the blueprint will be drawn using that 2d blueprint objects in 3d will be placed 
        int canvas_size = 80;
        // set the max size the hall can have (bad approach but gotta roll with this one)
        int hall_size = 20;

        // sets that hold integer ids describing what section on canvas is their region
        HashSet<int> Hall_ground_set = new HashSet<int>();
        HashSet<int> Wall_set = new HashSet<int>();

        // This is to make unique blocks on the canvas as it increments once a block is drawin using this id
        int Items_counter = 1;

        // if the user wants random seed assign a random value to the seed variable so that it is also visible to the debugger in inspector
        if(!useCustomSeed)
        {
            seed = (int)System.DateTime.Now.Ticks;
        }
        // make a random variable using that seed
        System.Random random = new System.Random(seed);

        // Make an empty canvas on which the blueprint will be drawn 
        int[,] array = new int[canvas_size, canvas_size];
        // initialize the canvas with zeros as 0 will be the value for empty space in the blue pring in this method
        for(int i = 0; i < canvas_size; i++)
            for(int j = 0; j < canvas_size; j++)
                array[i,j] = 0; 

        // assign starting position on canvas to the Hall
        Vector2 start_hall = new Vector2(random.Next(canvas_size-hall_size-1),random.Next(canvas_size-hall_size-1));
        // assign the hall its size and make sure its odd so that the hall will always havea center point.
        Vector2 size = new Vector2(random.Next(hall_size/8,hall_size/2)*2 + 1,random.Next(hall_size/8,hall_size/2)*2 + 1);

        // Assign Hall area on the canvas by drawing it manually then increment the item counter
        for(int i = (int)start_hall.x; i < start_hall.x + size.x; i++)
            for(int j = (int)start_hall.y; j < (int)start_hall.y + size.y; j++)
                array[i,j] = Items_counter; 
        Hall_ground_set.Add(Items_counter);
        Items_counter++;

        // draw walls for that block covering the borders horizontally and vertically
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

        // this is just for debugger to know how many blocks are spawned in the scene at the current time
        int counter = 0;

        // Empty parent nodes for the blocks on map so that they can be easily distinguished using inspector
        GameObject Hall_Floor_Parent = new GameObject("Hall Floor");
        GameObject Wall_Parent = new GameObject("Wall");

        // Finally scan the whole blueprint and spawn objects accordingly
        for(int i = 0; i < canvas_size; i++)
        {   
            for(int j = 0; j < canvas_size; j++)
                {
                    // if the id is in the hall section spawn a hall floor
                    if(Hall_ground_set.Contains(array[i,j]))
                    {
                        var temp = Instantiate(Hall_ground);
                        temp.transform.position = new Vector3(i,0,j);
                        temp.transform.SetParent(Hall_Floor_Parent.transform);
                        counter++;
                    }
                    // if the id is in the wall section spawn a wall
                    else if(Wall_set.Contains(array[i,j]))
                    {
                        var temp = Instantiate(Wall);
                        temp.transform.position = new Vector3(i,0,j);
                        temp.transform.SetParent(Wall_Parent.transform);
                        counter++;
                    }
                }
        }
        // show how many objects just spawned
        Debug.Log(counter.ToString());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
