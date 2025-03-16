using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
// using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class PCG_Room : MonoBehaviour
{
    [Header("Room Configuration")]
    public Vector2 roomSize = new Vector2(4.0f, 4.0f);
    public int nDoors = 3;
    private int currentDoors = 0;
    private Vector2 prevSize;

    [Header("Meshes")]
    public Mesh wallMesh;
    public Mesh wallMeshDoor;
    public Mesh wallMeshBroken;
    public Mesh pillarMesh;
    public Mesh floorMesh;

    [Header("Materials")]
    public Material stoneDark;
    public Material stone;
    public Material texture;

    [Header("Randomization")]
    public int seed = 1234;
    private int prevSeed;

    [Header("Other Variables")]
    public Color gridColor = Color.green;
    int wallCountX;
    int wallCountY;
    float wallSpacing = 4.0f;
    float pillarOffset = 0.25f;
    List<Matrix4x4> wallMatrices;
    List<Matrix4x4> wallMatricesDoor;
    List<Matrix4x4> wallMatricesBroken;
    List<Matrix4x4> pillars;
    List<Matrix4x4> floor;

    Matrix4x4[] wallMatrixArray;
    Matrix4x4[] wallMatrixDoorArray;
    Matrix4x4[] wallMatrixBrokenArray;
    Matrix4x4[] pillarArray;

    // Room Positions
    int stairsCorner;
    public GameObject stairsPrefab;
    private GameObject prefabHolder;

    // First Inside Room
    List<Matrix4x4> wallMatrices1;
    List<Matrix4x4> wallMatricesDoor1;
    List<Matrix4x4> wallMatricesBroken1;

    Matrix4x4[] wallMatrixArray1;
    Matrix4x4[] wallMatrixDoorArray1;
    Matrix4x4[] wallMatrixBrokenArray1;

    // Start is called before the first frame update
    void Start()
    {
        ValidateComponents();
        EnableGPUInstancing();

        prevSize = roomSize;
        prevSeed = seed;

        CreateRoom();
    }

    // Update is called once per frame
    void Update()
    {
        if (roomSize != prevSize || seed != prevSeed)
        {
            CreateRoom();

            prevSize = roomSize;
            prevSeed = seed;
        }
        RenderRoom();
    }

    void ValidateComponents()
    {
        if (wallMesh == null || wallMeshDoor == null || wallMeshBroken == null || pillarMesh == null || floorMesh == null)
        {
            Debug.LogError("One or more meshes are not assigned in PCG_Room.");
        }

        if (stoneDark == null || stone == null || texture == null)
        {
            Debug.LogError("One or more materials are not assigned in PCG_Room.");
        }
    }

    void EnableGPUInstancing()
    {
        stoneDark.enableInstancing = true;
        stone.enableInstancing = true;
        texture.enableInstancing = true;
    }

    void CreateRoom()
    {
        CreateWalls();
        CreatePillars();
        CreateFloor();
        CreateColliders();
    }

    void RenderRoom()
    {
        RenderWalls();
        RenderPillars();
        RenderFloor();
        RenderInsideFirstRoom();
    }

    #region Wall Generation
    struct WallConfig
    {
        public int Count;
        public Vector3 StartPosition;
        public Vector3 Direction;
        public Quaternion Rotation;
    }
    void AssignWalls(Matrix4x4 mat, ref int jDoors, int i, int count)
    {
        int rand = Mathf.FloorToInt(Random.Range(0f, 3f));
        if ((rand == 2 && currentDoors >= nDoors) || (rand == 2 && jDoors == 1))
        {
            while (rand == 2)
            {
                rand = Mathf.FloorToInt(Random.Range(0f, 3f));
            }
        }
        if (currentDoors < nDoors && i == count - 1 && jDoors == 0)
            rand = 2;
        if (rand == 0)
        {
            wallMatrices.Add(mat);
        }
        else if (rand == 1)
        {
            wallMatricesBroken.Add(mat);
        }
        else
        {
            currentDoors++;
            jDoors++;
            wallMatricesDoor.Add(mat);
        }
    }

    void createStairs()
    {
        stairsCorner = Mathf.FloorToInt(Random.Range(0f, 4f));
        // stairsCorner = 2;

        if (prefabHolder != null)
        {
            Destroy(prefabHolder);
        }
        Vector3 stairsTransform = new Vector3(0f, 0f, 0f);
        if (stairsCorner == 0)
        {
            stairsTransform = new Vector3(-(roomSize.x / 2) + 2, 0f, 0f);
        }
        else if (stairsCorner == 1)
        {
            stairsTransform = new Vector3(-(roomSize.x / 2) - 0.5f, 0f, (wallCountY - 1) * 4 + 2.5f);
        }
        else if (stairsCorner == 2)
        {
            stairsTransform = new Vector3(+(roomSize.x / 2) - 2, 0f, wallCountY * wallSpacing + 1f);
        }
        else if (stairsCorner == 3)
        {
            stairsTransform = new Vector3((roomSize.x / 2) + 0.5f, 0f, 2.5f);
        }

        Quaternion rotation = Quaternion.Euler(0f, 90f * (stairsCorner + 1) % 360, 0f);

        prefabHolder = Instantiate(stairsPrefab, stairsTransform, rotation);

    }
    void FirstInsideRoom()
    {
        Quaternion rotation = Quaternion.Euler(0f, 90f * (stairsCorner + 1) % 360, 0f);

        if (stairsCorner == 0)
        {
            int jDoors = 0;
            for (int j = 0; j < wallCountY - 1; j++)
            {
                Vector3 transform = new Vector3(-(roomSize.x / 2) + 0.5f + wallSpacing * 2, 0f, 6.5f + j * 4);
                Matrix4x4 mat = Matrix4x4.TRS(transform, rotation, Vector3.one);
                int rand = Mathf.FloorToInt(Random.Range(0f, 3f));
                if (j == wallCountY - 3 && jDoors == 0)
                    rand = 0;
                if (rand == 0)
                {
                    jDoors++;
                    wallMatricesDoor1.Add(mat);
                }
                else if (rand == 1)
                {

                    wallMatrices1.Add(mat);
                }
                else if (rand == 2)
                {

                    wallMatricesBroken1.Add(mat);
                }
            }
        }

        if (stairsCorner == 1)
        {
            int jDoors = 0;
            for (int j = 0; j < wallCountX - 1; j++)
            {
                Vector3 transform = new Vector3(-(roomSize.x / 2) + 6.0f + 4 * j, 0f, roomSize.y - 8);
                Matrix4x4 mat = Matrix4x4.TRS(transform, rotation, Vector3.one);
                int rand = Mathf.FloorToInt(Random.Range(0f, 3f));
                if (j == wallCountY - 3 && jDoors == 0)
                    rand = 0;
                if (rand == 0)
                {
                    jDoors++;
                    wallMatricesDoor1.Add(mat);
                }
                else if (rand == 1)
                {

                    wallMatrices1.Add(mat);
                }
                else if (rand == 2)
                {

                    wallMatricesBroken1.Add(mat);
                }
            }
        }

        if (stairsCorner == 2)
        {
            int jDoors = 0;
            for (int j = 0; j < wallCountY - 1; j++)
            {
                Vector3 transform = new Vector3(roomSize.x / 2 - 0.5f - wallSpacing * 2, 0f, roomSize.y - 5.5f - 4f * j);
                Matrix4x4 mat = Matrix4x4.TRS(transform, rotation, Vector3.one);
                int rand = Mathf.FloorToInt(Random.Range(0f, 3f));
                if (j == wallCountY - 3 && jDoors == 0)
                    rand = 0;
                if (rand == 0)
                {
                    jDoors++;
                    wallMatricesDoor1.Add(mat);
                }
                else if (rand == 1)
                {

                    wallMatrices1.Add(mat);
                }
                else if (rand == 2)
                {

                    wallMatricesBroken1.Add(mat);
                }
            }
        }

        // Convert lists to arrays for rendering
        wallMatrixArray1 = wallMatrices1.ToArray();
        wallMatrixDoorArray1 = wallMatricesDoor1.ToArray();
        wallMatrixBrokenArray1 = wallMatricesBroken1.ToArray();


    }
    void RenderInsideFirstRoom()
    {
        // Render them

        if (wallMatrixArray1.Length > 0)
        {
            Graphics.DrawMeshInstanced(wallMesh, 0, texture, wallMatrixArray1, wallMatrixArray1.Length);
        }

        if (wallMatrixDoorArray1.Length > 0)
        {
            Graphics.DrawMeshInstanced(wallMeshDoor, 0, texture, wallMatrixDoorArray1, wallMatrixDoorArray1.Length);

        }

        if (wallMatrixBrokenArray1.Length > 0)
        {
            Graphics.DrawMeshInstanced(wallMeshBroken, 0, texture, wallMatrixBrokenArray1, wallMatrixBrokenArray1.Length);
        }
    }
    void CreateWalls()
    {
        // Initial Seed
        Random.InitState(seed);
        currentDoors = 0;

        // Matirces for meshes
        wallMatrices = new List<Matrix4x4>();
        wallMatricesDoor = new List<Matrix4x4>();
        wallMatricesBroken = new List<Matrix4x4>();

        // Matirces for meshes
        wallMatrices1 = new List<Matrix4x4>();
        wallMatricesDoor1 = new List<Matrix4x4>();
        wallMatricesBroken1 = new List<Matrix4x4>();

        int roomSizeNewX = ((int)roomSize.x) / 2;
        int roomSizeNewY = ((int)roomSize.y) / 2;

        wallCountX = roomSizeNewX >= 2 ? roomSizeNewX / 2 : 1;
        wallCountY = roomSizeNewY >= 2 ? roomSizeNewY / 2 : 1;

        createStairs();
        FirstInsideRoom();

        // Define wall axes configurations
        var wallConfigs = new List<WallConfig>
        {
            new WallConfig
            {
                Count = wallCountX,
                StartPosition = transform.position + new Vector3(-(roomSizeNewX - 2), 0, 0),
                Direction = Vector3.right,
                Rotation = Quaternion.identity
            },
            new WallConfig
            {
                Count = wallCountY,
                StartPosition = transform.position + new Vector3(-roomSizeNewX - 0.5f, 0, 2.5f),
                Direction = Vector3.forward,
                Rotation = Quaternion.Euler(0f, 90f, 0f)
            },
            new WallConfig
            {
                Count = wallCountX,
                StartPosition = transform.position + new Vector3(-(roomSizeNewX - 2), 0, wallCountY * wallSpacing + 1f),
                Direction = Vector3.right,
                Rotation = Quaternion.identity
            },
            new WallConfig
            {
                Count = wallCountY,
                StartPosition = transform.position + new Vector3(roomSizeNewX + 0.5f, 0, 2.5f),
                Direction = Vector3.forward,
                Rotation = Quaternion.Euler(0f, 90f, 0f)
            }
        };

        int wallNumber = 0;

        foreach (var config in wallConfigs)
        {
            int jDoors = 0;
            for (int i = 0; i < config.Count; i++)
            {
                if (wallNumber == stairsCorner)
                {
                    if (wallNumber == 0 && (i == 0 || i == 1))
                    {
                        continue;
                    }
                    else if (wallNumber == 1 && (i == wallCountY - 1 || i == wallCountY - 2))
                    {
                        continue;
                    }
                    else if (wallNumber == 2 && (i == wallCountX - 1 || i == wallCountX - 2))
                    {
                        continue;
                    }
                    else if (wallNumber == 3 && (i == 0 || i == 1))
                    {
                        continue;
                    }

                }
                Vector3 wallTransform = config.StartPosition + config.Direction * (wallSpacing * i);
                Matrix4x4 mat = Matrix4x4.TRS(wallTransform, config.Rotation, Vector3.one);
                AssignWalls(mat, ref jDoors, i, config.Count);
            }
            wallNumber++;
        }


        // Convert lists to arrays for rendering
        wallMatrixArray = wallMatrices.ToArray();
        wallMatrixDoorArray = wallMatricesDoor.ToArray();
        wallMatrixBrokenArray = wallMatricesBroken.ToArray();
    }

    void RenderWalls()
    {
        if (wallMatrixArray.Length > 0)
        {
            Graphics.DrawMeshInstanced(wallMesh, 0, texture, wallMatrixArray, wallMatrixArray.Length);
        }

        if (wallMatrixDoorArray.Length > 0)
        {
            Graphics.DrawMeshInstanced(wallMeshDoor, 0, texture, wallMatrixDoorArray, wallMatrixDoorArray.Length);

        }

        if (wallMatrixBrokenArray.Length > 0)
        {
            Graphics.DrawMeshInstanced(wallMeshBroken, 0, texture, wallMatrixBrokenArray, wallMatrixBrokenArray.Length);
        }
    }
    #endregion

    #region Pillar Genration
    void CreatePillars()
    {
        pillars = new List<Matrix4x4>();

        Vector3[] pillarPositions = new Vector3[]
        {
            new Vector3(-roomSize.x / 2 - pillarOffset, 0,pillarOffset),
            new Vector3(roomSize.x / 2 + pillarOffset, 0,pillarOffset),
            new Vector3(-roomSize.x / 2 - pillarOffset, 0, wallCountY * wallSpacing + 0.75f),
            new Vector3(roomSize.x / 2 + pillarOffset, 0, wallCountY * wallSpacing + 0.75f)
        };

        foreach (var pos in pillarPositions)
        {
            Matrix4x4 mat = Matrix4x4.TRS(pos, transform.rotation, Vector3.one);
            pillars.Add(mat);
        }

        pillarArray = pillars.ToArray();

    }

    void RenderPillars()
    {
        if (pillars.Count > 0)
        {
            Graphics.DrawMeshInstanced(pillarMesh, 0, texture, pillarArray, pillarArray.Length);
        }
    }

    #endregion

    #region Floor Generation
    void CreateFloor()
    {
        int gridSizeX = wallCountX * 2 + 1;
        int gridSizeY = wallCountY * 2 + 1;

        floor = new List<Matrix4x4>();

        for (int i = 0; i < gridSizeY; i++)
        {
            for (int j = 0; j < gridSizeX; j++)
            {
                Vector3 tileTransform = new Vector3(-roomSize.x / 2 + 2 * j, -1, 0.5f + 2 * i);

                Matrix4x4 mat = Matrix4x4.TRS(tileTransform, transform.rotation, new Vector3(1, 1, 1));

                floor.Add(mat);
            }
        }
    }

    void RenderFloor()
    {
        if (floor.Count > 0)
        {
            Matrix4x4[] floorArray = floor.ToArray();
            Graphics.DrawMeshInstanced(floorMesh, 0, stone, floorArray, floorArray.Length);
        }
    }

    #endregion

    #region Colliders
    // Remove all existing MeshColliders before adding new ones
    void ClearExistingChilds()
    {
        // Find and destroy all child objects
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }
    }

    // Create colliders for doors
    void CreateColliders()
    {
        ClearExistingChilds();

        // Door Colliders
        // Parent Door Collider object
        GameObject doorColliders = new GameObject("Door Colliders");
        doorColliders.transform.parent = this.transform;
        for (int i = 0; i < wallMatrixDoorArray.Length; i++)
        {
            Vector3 position = wallMatrixDoorArray[i].GetColumn(3);
            // Debug.Log("Position " + position.ToString());
            Quaternion rotation = Quaternion.LookRotation(wallMatrixDoorArray[i].GetColumn(2), wallMatrixDoorArray[i].GetColumn(1));

            // Create a child GameObject for each door collider
            GameObject doorColliderObj = new GameObject($"Door Collider_{i}");
            doorColliderObj.transform.parent = doorColliders.transform;
            doorColliderObj.transform.position = position;
            doorColliderObj.transform.rotation = rotation;

            MeshCollider meshCollider = doorColliderObj.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = wallMeshDoor;

        }

        // Floor Collider
        Vector3 floorPosition = new Vector3(0, -0.5f, roomSize.y / 2 + 0.5f);

        // Create a child GameObject for floor collider
        GameObject floorColliderObj = new GameObject("FloorCollider");
        floorColliderObj.transform.parent = this.transform;
        floorColliderObj.transform.position = floorPosition;

        BoxCollider boxCollider = floorColliderObj.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(roomSize.x + 2, 1, roomSize.y + 2);

        // Pillar Colliders
        // Parent Pillar Collider object
        GameObject pillarColliders = new GameObject("Pillar Colliders");
        pillarColliders.transform.parent = this.transform;
        for (int i = 0; i < pillarArray.Length; i++)
        {
            Vector3 position = pillarArray[i].GetColumn(3);

            // Create a child GameObject for each pillar collider
            GameObject pillarColliderObj = new GameObject($"PillarCollider_{i}");
            pillarColliderObj.transform.parent = pillarColliders.transform;
            pillarColliderObj.transform.position = position;

            BoxCollider pillarCollider = pillarColliderObj.AddComponent<BoxCollider>();
            pillarCollider.size = new Vector3(1.5f, 4.0f, 1.5f);
            pillarCollider.center = new Vector3(0.0f, 2.0f, 0.0f);
        }

        // Parent object for left right colliders
        GameObject leftRightColliders = new GameObject("Left Right Colliders");
        leftRightColliders.transform.parent = this.transform;

        // Parent object for full wall colliders
        GameObject fullWallColliders = new GameObject("Full Wall Colliders");
        fullWallColliders.transform.parent = this.transform;
        // Colliders for remaining walls
        for (int i = 1, j = 1; i <= 4; i++)
        {
            if (i <= nDoors)
            {
                float doorPosition = (i % 2 == 1) ? wallMatrixDoorArray[i - 1].GetColumn(3).x : wallMatrixDoorArray[i - 1].GetColumn(3).z;

                int leftWallCount = (i % 2 == 1) ? (int)Mathf.Abs(-roomSize.x / 2 + 2 - doorPosition) / 4 : (int)(roomSize.y - doorPosition) / 4;

                int rightWallCount = (i % 2 == 1) ? (int)Mathf.Abs(roomSize.x / 2 - 2 - doorPosition) / 4 : (int)(doorPosition - 2.5f) / 4;

                if (leftWallCount > 0)
                {
                    float positionX;
                    float positionZ;
                    Quaternion rotation = (i % 2 == 1) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 90, 0);
                    if (i % 2 == 1)
                    {
                        positionX = doorPosition - 2 - Mathf.Abs(-roomSize.x / 2 - (doorPosition - 2)) / 2;
                        positionZ = (i == 1) ? 0.0f : roomSize.y + 1.0f;
                    }
                    else
                    {
                        positionZ = (doorPosition + 2 + roomSize.y + 0.5f) / 2;
                        positionX = (i == 2) ? -roomSize.x / 2 - 0.5f : roomSize.x / 2 + 0.5f;

                    }
                    GameObject leftColliderObj = new GameObject($"Left {i}");
                    leftColliderObj.transform.parent = leftRightColliders.transform;
                    leftColliderObj.transform.position = new Vector3(positionX, 2.0f, positionZ);
                    leftColliderObj.transform.rotation = rotation;

                    BoxCollider leftCollider = leftColliderObj.AddComponent<BoxCollider>();
                    leftCollider.size = new Vector3(leftWallCount * 4.0f, 4.0f, 1.0f);
                }

                if (rightWallCount > 0)
                {
                    float positionX;
                    float positionZ;
                    Quaternion rotation = (i % 2 == 1) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 90, 0);
                    if (i % 2 == 1)
                    {
                        positionX = doorPosition + 2 + Mathf.Abs(roomSize.x / 2 - (doorPosition + 2)) / 2;
                        positionZ = (i == 1) ? 0.0f : roomSize.y + 1.0f;
                    }
                    else
                    {
                        positionZ = (doorPosition - 2 + 0.5f) / 2; ;
                        positionX = (i == 2) ? -roomSize.x / 2 - 0.5f : roomSize.x / 2 + 0.5f;
                    }
                    GameObject rightColliderObj = new GameObject($"Right {i}");
                    rightColliderObj.transform.parent = leftRightColliders.transform;
                    rightColliderObj.transform.position = new Vector3(positionX, 2.0f, positionZ);
                    rightColliderObj.transform.rotation = rotation;

                    BoxCollider rightCollider = rightColliderObj.AddComponent<BoxCollider>();
                    rightCollider.size = new Vector3(rightWallCount * 4.0f, 4.0f, 1.0f);
                }
            }
            else
            {
                float positionX;
                float positionZ;
                Quaternion rotation = (i % 2 == 1) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 90, 0);

                if (i % 2 == 1)
                {
                    positionX = 0.0f;
                    positionZ = (i == 1) ? 0.0f : roomSize.y + 1.0f;
                }
                else
                {
                    positionX = (i == 2) ? -roomSize.x / 2 - 0.5f : roomSize.x / 2 + 0.5f;
                    positionZ = roomSize.y / 2 + 0.5f;
                }

                GameObject fullColliderObj = new GameObject($"Full Wall Collider {j}");
                fullColliderObj.transform.parent = fullWallColliders.transform;
                fullColliderObj.transform.position = new Vector3(positionX, 2.0f, positionZ);
                fullColliderObj.transform.rotation = rotation;

                BoxCollider fullCollider = fullColliderObj.AddComponent<BoxCollider>();
                fullCollider.size = (i % 2 == 1) ? new Vector3(wallCountX * 4.0f, 4.0f, 1.0f) : new Vector3(wallCountY * 4.0f, 4.0f, 1.0f);

                j++;
            }
        }

        // Broken Wall Colliders
        // Parent BrokenWall Collider object
        // GameObject brokenWallColliders = new GameObject("Broken Wall Colliders");
        // brokenWallColliders.transform.parent = this.transform;
        // for (int i = 0; i < wallMatrixBrokenArray.Length; i++)
        // {
        //     Vector3 position = wallMatrixBrokenArray[i].GetColumn(3);
        //     Debug.Log("Position " + position.ToString());
        //     Quaternion rotation = Quaternion.LookRotation(wallMatrixBrokenArray[i].GetColumn(2), wallMatrixBrokenArray[i].GetColumn(1));

        //     // Create a child GameObject for each brokenWall collider
        //     GameObject brokenWallColliderObj = new GameObject($"Broken Wall Collider_{i}");
        //     brokenWallColliderObj.transform.parent = brokenWallColliders.transform;
        //     brokenWallColliderObj.transform.position = position;
        //     brokenWallColliderObj.transform.rotation = rotation;

        //     MeshCollider meshCollider = brokenWallColliderObj.AddComponent<MeshCollider>();
        //     meshCollider.sharedMesh = wallMeshBroken;

        // }

        // Simple Wall Collider
        // Parent Wall Collider object
        // GameObject wallColliders = new GameObject("Wall Matrix Colliders");
        // wallColliders.transform.parent = this.transform;
        // for (int i = 0; i < wallMatrixArray.Length; i++)
        // {
        //     Vector3 position = wallMatrixArray[i].GetColumn(3);
        //     Debug.Log("Position " + position.ToString());
        //     Quaternion rotation = Quaternion.LookRotation(wallMatrixArray[i].GetColumn(2), wallMatrixArray[i].GetColumn(1));

        //     // Create a child GameObject for each wall collider
        //     GameObject wallColliderObj = new GameObject($"Wall Matrix Collider_{i}");
        //     wallColliderObj.transform.parent = wallColliders.transform;
        //     wallColliderObj.transform.position = position;
        //     wallColliderObj.transform.rotation = rotation;

        //     BoxCollider boxColliderWall = wallColliderObj.AddComponent<BoxCollider>();
        //     boxColliderWall.size = new Vector3(4.0f, 4.0f, 1.0f);
        //     boxColliderWall.center = new Vector3(0.0f, 2.0f, 0.0f);
        // }

    }
    #endregion
}