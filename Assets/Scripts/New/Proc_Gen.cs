using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class Proc_Gen : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] Corner_Prefabs;
    [SerializeField] private GameObject[] doorPrefabs;
    [SerializeField] private GameObject[] normalWallPrefabs;
    [SerializeField] private GameObject[] windowedWallPrefabs;
    [SerializeField] private DecorationAsset[] decorationAssets;
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject ceilingTilePrefab;
    [SerializeField] private GameObject lightCeilingPrefab;
    [SerializeField] private GameObject stairsPrefab;
    [SerializeField] private GameObject trapDoorPrefab;
    [SerializeField] private GameObject keyRingPrefab;
    [SerializeField] private GameObject artifact;
    [SerializeField] private GameObject battery;
    [SerializeField] private Proc_Gen_UI procGenUI;
    [SerializeField] private Minimap minimap;
    [SerializeField] private GameObject minimapUI;
    [SerializeField] private Player_Movement player_MovementRef;

    private int seed;
    [SerializeField, Range(3, 5)] private int nFloors;
    private GameObject[][] wallPrefabs;

    // Change Detectors
    private int prevSeed, prevNFloors;
    public GameObject player;
    Floor[] floors;
    public int currentFloor;
    int prevFloor;
    float currentY;
    int[] keyRings;
    void Awake()
    {
        wallPrefabs = new GameObject[3][];
        wallPrefabs[0] = doorPrefabs;
        wallPrefabs[1] = normalWallPrefabs;
        wallPrefabs[2] = windowedWallPrefabs;
        seed = (int)UnityEngine.Random.Range(0, math.pow(2, 30));
        UnityEngine.Random.InitState(seed);
        nFloors = UnityEngine.Random.Range(3, 6);
        prevSeed = seed;
        prevNFloors = nFloors;
        keyRings = new int[nFloors];
    }

    void Start()
    {
        player.GetComponent<Rigidbody>().position = new Vector3(12, (nFloors - 1) * 4 + 0.55f, -34f);

        currentFloor = (int)player.GetComponent<Rigidbody>().position.y / 4 + 1;
        prevFloor = currentFloor;

        minimap.PlaceTrapDoor(currentFloor);

        procGenUI.UpdateCurrentFloor(currentFloor);
        // Debug.Log("Current Floor " + currentFloor.ToString());
        currentY = player.GetComponent<Rigidbody>().position.y;
        procGenUI.UpdateTrapDoorImage("red");
        Generate();
    
        player_MovementRef.minutesToDecrease = keyRings[currentFloor - 1];
        // player_MovementRef.increaseFlash();
    }

    void Update()
    {
        // If any key parameter changes, update all three and generate once
        if (seed != prevSeed || nFloors != prevNFloors)
        {
            prevSeed = seed;
            prevNFloors = nFloors;
            Generate();
        }

        if (player.GetComponent<Rigidbody>().position.y != currentY)
        {
            currentY = player.GetComponent<Rigidbody>().position.y;
            currentFloor = (int)player.GetComponent<Rigidbody>().position.y / 4 + 1;
            if (currentFloor != prevFloor)
            {
                prevFloor = currentFloor;
                player_MovementRef.minutesToDecrease = keyRings[currentFloor - 1];
                player_MovementRef.increaseFlash();
                minimap.PlaceTrapDoor(currentFloor);

                procGenUI.UpdateCurrentFloor(currentFloor);
                procGenUI.UpdateTrapDoorImage("red");
                procGenUI.UpdateRemainingKeys(keyRings[currentFloor - 1]);
                if (currentFloor == 1)
                {
                    player_MovementRef.minutesToDecrease = 1;
                    // procGenUI.UpdateTrapDoorImage("red",true);
                    minimapUI.SetActive(false);
                    procGenUI.UpdateKeysText("Collect Artifact");
                    procGenUI.UpdateRemainingKeys(-1, true);
                }
            }

            // Debug.Log("Current Floor " + currentFloor.ToString());
        }
    }

    public void MovePlayerToGround()
    {
        player.GetComponent<Rigidbody>().transform.position = new Vector3(7.5f, 0.55f, -33.5f);
    }
    public void CollectKey(int _currentFloor, bool isKeySmasher = false)
    {
        if (isKeySmasher)
        {
            keyRings[_currentFloor] = 0;
            procGenUI.UpdateRemainingKeys(keyRings[_currentFloor]);
        }
        else
        {
            keyRings[_currentFloor]--;
            procGenUI.UpdateRemainingKeys(keyRings[_currentFloor]);
        }
        // Debug.Log("Remaing keys " + keyRings[_currentFloor].ToString());
        if (keyRings[_currentFloor] == 0)
        {
            procGenUI.UpdateTrapDoorImage("green");
            GameObject floor = GameObject.Find($"Floor_{currentFloor}");
            if (floor != null)
            {
                Transform trapDoor = floor.transform.Find("Trap_Door");
                if (trapDoor != null)
                {
                    Destroy(trapDoor.gameObject);
                }
                else
                {
                    Debug.LogWarning($"Trap_Door not found under Floor_{_currentFloor}");
                }
            }
            else
            {
                Debug.LogWarning($"Floor_{currentFloor} not found!");
            }
        }
    }
    void SpawnPrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, string name)
    {
        GameObject instance = Instantiate(prefab, position, rotation, parent);
        instance.name = name;
    }

    void SpawnKey(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, string name, int _floor)
    {
        GameObject keyObject = Instantiate(prefab, position, rotation, parent);
        keyObject.name = name;
        KeyRing keyScript = keyObject.GetComponent<KeyRing>();

        if (keyScript != null)
        {
            keyScript.currentFloor = _floor; // Assign the floor number
        }
    }

    string GetWallName(int index) => index switch
    {
        0 => "Front_Wall",
        1 => "Left_Wall",
        2 => "Back_Wall",
        3 => "Right_Wall",
        _ => "Wall"
    };
    void DestroyAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    void Generate()
    {
        DestroyAllChildren(this.transform);

        int pWidth = 15;
        int pLength = 17;

        floors = new Floor[nFloors];

        for (int floor = 0; floor < nFloors; floor++)
        {
            int floorNumber = floor;
            floors[floor] = new Floor(floorNumber, this.transform, decorationAssets, pWidth, pLength);

            int[] _doorWall = new int[1];
            _doorWall[0] = 0;

            int nKeyRings = UnityEngine.Random.Range(2, 6);
            if (floor == 0)
                keyRings[floor] = 2;
            else
                keyRings[floor] = nKeyRings;
            List<Ground> availableTiles = new List<Ground>();

            if (floor < nFloors - 1)
            {
                Stairs stairs = new Stairs(floor, floor % 2, pWidth, pLength);
                SpawnPrefab(stairsPrefab, stairs.position, stairs.rotation, floors[floor].baseRoom.room.transform, $"Stairs_{floor + 1}");
            }
            if (floor > 0)
            {
                Vector3 position;
                if (floor % 2 == 1)
                {
                    position = new Vector3(-27f, 4 * floorNumber, -31.6f);
                }
                else
                {
                    position = new Vector3(+27f, 4 * floorNumber, -31.6f);
                }
                // Debug.Log("Spawning on floor " + floor.ToString());
                SpawnPrefab(trapDoorPrefab, position, Quaternion.identity, floors[floor].baseRoom.room.transform, "Trap_Door");
            }

            // Spawn floor tiles
            for (int i = 0; i < floors[floor].baseRoom.ground.GetLength(0); i++)
            {
                for (int j = 0; j < floors[floor].baseRoom.ground.GetLength(1); j++)
                {
                    SpawnPrefab(floorTilePrefab, floors[floor].baseRoom.ground[i, j].position, Quaternion.identity, floors[floor].baseRoom.floorParent, $"Ground_{i}_{j}");

                    if (floors[floor].baseRoom.ground[i, j].zoneTag == Ground.Zone.Hallway)
                    {
                        availableTiles.Add(floors[floor].baseRoom.ground[i, j]);
                    }
                }
            }

            DestroyTrapDoor(floor, floor % 2);


            int ceilingCount = 0; // Track how many ceilings have been placed
            int nextLightTile = UnityEngine.Random.Range(15, 30); // Randomly choose the next light tile position

            for (int i = 0; i < floors[floor].ceilings.GetLength(0); i++)
            {
                for (int j = 0; j < floors[floor].ceilings.GetLength(1); j++)
                {
                    GameObject prefabToUse = (ceilingCount == nextLightTile) ? lightCeilingPrefab : ceilingTilePrefab;

                    SpawnPrefab(prefabToUse, floors[floor].ceilings[i, j].position, Quaternion.identity, floors[floor].ceilingParent.transform, $"Ceiling_{i}_{j}");

                    if (ceilingCount == nextLightTile)
                    {
                        nextLightTile += UnityEngine.Random.Range(8, 15);
                    }

                    ceilingCount++;
                }
            }

            // Create corners and walls
            for (int i = 0; i < 4; i++)
            {
                SpawnPrefab(Corner_Prefabs[floors[floor].baseRoom.corners[i].cornerPrefabNumber], floors[floor].baseRoom.corners[i].position, floors[floor].baseRoom.corners[i].rotation, floors[floor].baseRoom.cornerParent, $"Corner_{i + 1}");

                int nWalls = (i == 0 || i == 2) ? floors[floor].baseRoom.width : floors[floor].baseRoom.length;

                Transform wallSubParent = CreateParent(GetWallName(i), floors[floor].baseRoom.wallParent);
                for (int j = 0; j < nWalls; j++)
                {
                    GameObject prefab = wallPrefabs[floors[floor].baseRoom.walls[i].row[j]][floors[floor].baseRoom.walls[i].col[j]];
                    SpawnPrefab(prefab, floors[floor].baseRoom.walls[i].positions[j], floors[floor].baseRoom.walls[i].rotation, wallSubParent, $"Wall_{j + 1}");
                }
            }

            //Place Decorations
            for (int i = 0; i < floors[floor].baseRoom.props.Count; i++)
            {
                SpawnPrefab(floors[floor].baseRoom.props[i].asset.prefab, floors[floor].baseRoom.props[i].position, floors[floor].baseRoom.props[i].rotation, floors[floor].baseRoom.decorationParent, $"Item_{i + 1}");
            }

            for (int l = 0; l < floors[floor].nRooms; l++)
            {
                SpawnRoomPrefabs(floors[floor].rooms[l], floor);
                for (int i = 0; i < floors[floor].rooms[l].ground.GetLength(0); i++)
                {
                    for (int j = 0; j < floors[floor].rooms[l].ground.GetLength(1); j++)
                    {
                        if (floors[floor].rooms[l].ground[i, j].zoneTag != Ground.Zone.Forbidden)
                        {
                            availableTiles.Add(floors[floor].rooms[l].ground[i, j]);
                        }
                    }
                }
            }

            List<Ground> selectedTiles = availableTiles.OrderBy(x => UnityEngine.Random.value).Take(nKeyRings + 1).ToList();

            SpawnPrefab(battery, selectedTiles[selectedTiles.Count - 1].position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, floors[floor].baseRoom.room.transform, "Battery");

            if (floor > 0)
            {
                for (int i = 0; i < nKeyRings; i++)
                {
                    SpawnKey(keyRingPrefab, selectedTiles[i].position + new Vector3(0f, 1.5f, 0f), Quaternion.identity, floors[floor].baseRoom.room.transform, $"Key_{i + 1}", floor);
                }

            }
            else
            {
                SpawnPrefab(artifact, selectedTiles[0].position + new Vector3(0f, 1.5f, 0f), Quaternion.identity, floors[floor].baseRoom.room.transform, "Artifact");
                keyRings[0] = 0;
            }
        }
        procGenUI.UpdateRemainingKeys(keyRings[currentFloor - 1]);
    }
    void DestroyTrapDoor(int floor, int stairsCorner)
    {
        if (floor == 0) return;

        Transform floorParent = transform.Find($"Floor_{floor + 1}");
        if (floorParent == null) return;

        // Destroy ground objects
        Transform groundParent = floorParent.Find("Ground");
        if (groundParent != null)
        {
            int totalRows = floors[floor].baseRoom.ground.GetLength(0);
            int totalCols = floors[floor].baseRoom.ground.GetLength(1);

            int[] targetRows = { totalRows - 1, totalRows - 2 };
            int[] targetCols = stairsCorner == 0 ? new int[] { totalCols - 2, totalCols - 3, totalCols - 4 } : new int[] { 1, 2, 3 }; // Swap conditions for stairsCorner

            foreach (int row in targetRows)
            {
                foreach (int col in targetCols)
                {
                    Transform groundTile = groundParent.Find($"Ground_{row}_{col}");
                    if (groundTile != null)
                    {
                        Destroy(groundTile.gameObject);
                    }
                }
            }
        }

        // Destroy ceiling objects
        floorParent = transform.Find($"Floor_{floor}");
        if (floorParent == null) return;
        Transform ceilingParent = floorParent.Find("Ceilings");
        if (ceilingParent != null)
        {
            int totalRows = floors[floor].ceilings.GetLength(0);
            int totalCols = floors[floor].ceilings.GetLength(1);

            int lastRow = totalRows - 1;
            int[] targetCols = stairsCorner == 0 ? new int[] { totalCols - 1, totalCols - 2 } : new int[] { 0, 1 }; // Swap conditions for stairsCorner

            foreach (int col in targetCols)
            {
                Transform ceilingTile = ceilingParent.Find($"Ceiling_{lastRow}_{col}");
                if (ceilingTile != null)
                {
                    Destroy(ceilingTile.gameObject);
                }
            }
        }
    }
    void SpawnRoomPrefabs(Room room, int floor)
    {
        // Debug.Log("Floor Number " + floor.ToString());
        // Debug.Log("Room Cetner " + room.center);
        // Create corners and walls
        for (int i = 0; i < 4; i++)
        {
            // Debug.Log("Room Cetner " + room.corners[i].position);
            SpawnPrefab(
                Corner_Prefabs[room.corners[i].cornerPrefabNumber],
                room.corners[i].position,
                room.corners[i].rotation,
                room.cornerParent,
                $"Corner_{i + 1}"
            );

            int nWalls = (i == 0 || i == 2) ? room.width : room.length;
            Transform wallSubParent = CreateParent(GetWallName(i), room.wallParent);

            for (int j = 0; j < nWalls; j++)
            {
                GameObject prefab = wallPrefabs[room.walls[i].row[j]][room.walls[i].col[j]];
                SpawnPrefab(prefab, room.walls[i].positions[j], room.walls[i].rotation, wallSubParent, $"Wall_{j + 1}");
            }
        }

        //Place Decorations
        for (int i = 0; i < room.props.Count; i++)
        {
            SpawnPrefab(room.props[i].asset.prefab, room.props[i].position, room.props[i].rotation, room.decorationParent, $"Item_{i + 1}");
        }
    }
    Transform CreateParent(string name, Transform parent = null)
    {
        GameObject obj = new GameObject(name);
        if (parent) obj.transform.SetParent(parent);
        return obj.transform;
    }
    // void OnDrawGizmos()
    // {
    //     if (groundPortion == null) return;
    //     if (groundPortion.baseRoom == null)
    //     {
    //         return;
    //     }

    //     for (int i = 0; i < groundPortion.baseRoom.ground.GetLength(0); i++)
    //     {
    //         for (int j = 0; j < groundPortion.baseRoom.ground.GetLength(1); j++)
    //         {
    //             groundPortion.baseRoom.ground[i, j].DrawGizmo();
    //         }
    //     }

    // }

}
