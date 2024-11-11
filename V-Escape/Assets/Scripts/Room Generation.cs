using System.Collections;
using System.Collections.Generic;
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
    public GameObject Room_ground;
    public GameObject Wall;

    // Awake is called at the time of scene load
    void Awake()
    {
        // Make an empty canvas on which the blueprint will be drawn using that 2d blueprint objects in 3d will be placed 
        int canvas_size = 10000/10;
        // set the max size the hall can have (bad approach but gotta roll with this one)
        int hall_size = 800/10;
        // set the max size the room can have (bad approach but gotta roll with this one)
        int room_size = 100/10;

        // sets that hold integer ids describing what section on canvas is their region
        HashSet<int> Hall_ground_set = new HashSet<int>();
        HashSet<int> Room_ground_set = new HashSet<int>();
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
        Vector2 start_hall = new Vector2(random.Next(canvas_size/4,canvas_size/4*3),random.Next(canvas_size/4,canvas_size/4*3));
        // assign the hall its size and make sure its odd so that the hall will always havea center point.
        Vector2 size = new Vector2(random.Next(hall_size/4,hall_size/2)*2 + 1,random.Next(hall_size/16,hall_size/8)*2 + 1);

        // Assign Hall area on the canvas by drawing it manually then increment the item counter
        for(int i = (int)start_hall.x; i < start_hall.x + size.x; i++)
            for(int j = (int)start_hall.y; j < (int)start_hall.y + size.y; j++)
                array[i,j] = Items_counter; 
        Hall_ground_set.Add(Items_counter);
        Items_counter++;

        // Check how many rooms can be placed along the hall in this way
        Vector2 roomsfittable = new Vector2(size.x/room_size,size.y/room_size);

        // Draw rooms on x wise sides if space is available on canvas and add a certain probablity that the room will not be made at all
        for(int i = 0; i < roomsfittable.x; i++)
        {
            // Needs to be done for both sides of the hallway
            for(int j = -1; j <= 1; j+=2)
            {
                // random chance that room will not be made at all
                if(random.Next(100)<25)
                {
                    continue;
                }
                Vector2 current_room_size = new Vector2(random.Next(room_size/4,room_size/2)*2 + 1,random.Next(room_size/4,room_size/2)*2 + 1);
                int addfactor = 0;
                // depending on the side length needs ot be added or subtracted so these factors will be taken care at this line of the code by simply multiplying the factors with the lengths so that if its the sides turn to add to its length only then shall it be added or subtracted.
                if(j == 1)
                {
                    addfactor = 1;
                }
                int subtractfactor = 0;
                if(j == -1)
                {
                    subtractfactor = 1;
                }
                Vector2 min = new Vector2(start_hall.x + room_size * i,start_hall.y + j + ((size.y - 1) * addfactor) - (subtractfactor * current_room_size.y-1));
                Vector2 max = new Vector2(start_hall.x + room_size * i + current_room_size.x,start_hall.y + j + current_room_size.y + ((size.y - 1) * addfactor) - (subtractfactor * current_room_size.y-1));

                
                // see if the Room is going out of bounds. in that case dont draw the room at all
                if(min.x < 0 && min.y < 0 && min.x >= canvas_size && min.y >= canvas_size)
                {
                    continue;
                }
                else if(max.x < 0 && max.y < 0 && max.x >= canvas_size && max.y >= canvas_size)
                {
                    continue;
                }
                // draw the room on canvas if all has passed
                for(int a = (int)min.x; a < max.x; a++)
                {
                    for(int b = (int)min.y; b != max.y;)
                    {
                        array[a,b] = Items_counter;
                        if(b<max.y)
                            b++;
                        if(b>max.y)
                            b--;
                    }
                }
                Room_ground_set.Add(Items_counter);
                Items_counter++;
            }
        }

        // do the same as above but on y side

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
        // list of rooms as all rooms need to be placed differently
        List<GameObject> Room_Floor_Parents = new List<GameObject>();

        foreach(int room in Room_ground_set)
        {
            GameObject temp = new GameObject("Room Floor "+room.ToSafeString());
            Room_Floor_Parents.Add(temp);
        }

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
                // if the id is in the room section spawn a wall
                else if(Room_ground_set.Contains(array[i,j]))
                {
                    var temp = Instantiate(Room_ground);
                    temp.transform.position = new Vector3(i,0,j);
                    GameObject roomobj = null;
                    // place the room floor tiles in their respective parent object
                    foreach (GameObject o in Room_Floor_Parents)
                    {
                        if(o.name == "Room Floor "+array[i,j].ToSafeString())
                        {
                            roomobj = o;
                            break;
                        }
                    } 
                    temp.transform.SetParent(roomobj.transform);
                    counter++;
                }
            }
        }
        // show how many objects just spawned
        Debug.Log(counter.ToString());
        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
