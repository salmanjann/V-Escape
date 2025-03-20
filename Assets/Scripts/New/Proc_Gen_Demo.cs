using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Proc_Gen_Demo : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject[] Corner_Prefabs;
    [SerializeField] private GameObject[] doorPrefabs;
    [SerializeField] private GameObject[] normalWallPrefabs;
    [SerializeField] private GameObject[] windowedWallPrefabs;
    [SerializeField] private DecorationAsset[] decorationAssets;
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject stairsPrefab;
    [SerializeField] private GameObject keyRingPrefab;
    [SerializeField] private GameObject artifact;
    [SerializeField] private GameObject battery;
    [SerializeField, Range(61, 65)] private int seed;
    private int nFloors;
    private GameObject[][] wallPrefabs;

    // Change Detectors
    private int prevSeed, prevNFloors;
    Floor[] floors;
    void Awake()
    {
        wallPrefabs = new GameObject[3][];
        wallPrefabs[0] = doorPrefabs;
        wallPrefabs[1] = normalWallPrefabs;
        wallPrefabs[2] = windowedWallPrefabs;
        nFloors = 1;
        prevSeed = seed;
    }

    void Start()
    {
        Generate();
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

    }
    void SpawnPrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, string name)
    {
        GameObject instance = Instantiate(prefab, position, rotation, parent);
        instance.name = name;
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

    void Generate()
    {
        UnityEngine.Random.InitState(seed);

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

            List<Ground> availableTiles = new List<Ground>();

            if (floor < nFloors - 1)
            {
                Stairs stairs = new Stairs(floor, floor % 2, pWidth, pLength);
                SpawnPrefab(stairsPrefab, stairs.position, stairs.rotation, floors[floor].baseRoom.room.transform, $"Stairs_{floor + 1}");
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

            int nKeyRings = UnityEngine.Random.Range(2, 6);
            List<Ground> selectedTiles = availableTiles.OrderBy(x => UnityEngine.Random.value).Take(nKeyRings + 1).ToList();

            SpawnPrefab(battery, selectedTiles[selectedTiles.Count - 1].position + new Vector3(0f, 0.5f, 0f), Quaternion.identity, floors[floor].baseRoom.room.transform, "Battery");


            for (int i = 0; i < nKeyRings; i++)
            {
                SpawnKey(keyRingPrefab, selectedTiles[i].position + new Vector3(0f, 1.5f, 0f), Quaternion.identity, floors[floor].baseRoom.room.transform, $"Key_{i + 1}", floor);
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
