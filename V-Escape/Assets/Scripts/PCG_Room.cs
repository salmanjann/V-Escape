using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PCG_Room : MonoBehaviour
{
    [Header("Room Configuration")]
    public Vector2 roomSize = new Vector2(4.0f, 4.0f);
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
    private int wallCountX;
    private int wallCountY;
    private float wallSpacing = 4.0f;
    private float pillarOffset = 0.25f;
    List<Matrix4x4> wallMatrices;
    List<Matrix4x4> wallMatricesDoor;
    List<Matrix4x4> wallMatricesBroken;
    List<Matrix4x4> pillars;
    List<Matrix4x4> floor;

    private Matrix4x4[] wallMatrixArray;
    private Matrix4x4[] wallMatrixDoorArray;
    private Matrix4x4[] wallMatrixBrokenArray;

    // Start is called before the first frame update
    void Start()
    {
        ValidateComponents();
        EnableGPUInstancing();
        CreateRoom();

        prevSize = roomSize;
        prevSeed = seed;
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
    }

    void RenderRoom()
    {
        RenderWalls();
        RenderPillars();
        RenderFloor();
    }

    #region Wall Generation
    struct WallConfig
    {
        public int Count;
        public Vector3 StartPosition;
        public Vector3 Direction;
        public Quaternion Rotation;
    }
    void AssignWalls(Matrix4x4 mat)
    {
        int rand = Mathf.FloorToInt(Random.Range(0f, 3f));
        if (rand < 1)
        {
            wallMatrices.Add(mat);
        }
        else if (rand < 2)
        {
            wallMatricesDoor.Add(mat);
        }
        else
        {
            wallMatricesBroken.Add(mat);
        }
    }
    void CreateWalls()
    {
        // Initial Seed
        Random.InitState(seed);

        // Matirces for meshes
        wallMatrices = new List<Matrix4x4>();
        wallMatricesDoor = new List<Matrix4x4>();
        wallMatricesBroken = new List<Matrix4x4>();

        int roomSizeNewX = ((int)roomSize.x) / 2;
        int roomSizeNewY = ((int)roomSize.y) / 2;

        wallCountX = roomSizeNewX >= 2 ? roomSizeNewX / 2 : 1;
        wallCountY = roomSizeNewY >= 2 ? roomSizeNewY / 2 : 1;

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

        foreach (var config in wallConfigs)
        {
            for (int i = 0; i < config.Count; i++)
            {
                Vector3 wallTransform = config.StartPosition + config.Direction * (wallSpacing * i);
                Matrix4x4 mat = Matrix4x4.TRS(wallTransform, config.Rotation, Vector3.one);
                AssignWalls(mat);
            }
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
    }

    void RenderPillars()
    {
        if (pillars.Count > 0)
        {
            Matrix4x4[] pillarArray = pillars.ToArray();
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

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        // Calculate the bottom-left corner position of the grid
        Vector3 Left = new Vector3(-roomSize.x / 2, 0, 0.5f);
        Vector3 Right = new Vector3(roomSize.x / 2, 0, 0.5f);
        Vector3 Bottom = new Vector3(0, 0, 0.5f);
        Vector3 Top = new Vector3(0, 0, roomSize.y + 0.5f);

        // Draw vertical lines
        for (int x = 0; x <= roomSize.x; x++)
        {
            Gizmos.DrawLine(Bottom + new Vector3(-roomSize.x / 2 + x * 1f, 0, 0), Top + new Vector3(-roomSize.x / 2 + x * 1f, 0, 0));
        }

        // Draw horizontal lines starting from -roomSize.x/2 to +roomSize.x
        for (int z = 0; z <= roomSize.y; z++)
        {
            Gizmos.DrawLine(Left + new Vector3(0, 0, z * 1.0f), Right + new Vector3(0, 0, z * 1.0f));
        }
    }

    #region Cell Class
    enum CellTag  {Inside, Outside};
    enum CellSideTag  {North,South,East, West};
    public class Cell{
        Vector3 postion;
        CellTag zone;
        CellSideTag side;

        Cell(Vector3 _position, CellTag _zone, CellSideTag _side){
            postion = _position;
            zone = _zone;
            side = _side;
        }

        public override string ToString()
        {
            return postion + " " + zone + " " + side;
        }

    };

    #endregion

}