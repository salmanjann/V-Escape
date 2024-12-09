using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCG_Labyrinth : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField]
    Vector2 Grid = new Vector2(1f, 1f); // Size of whole map
    [Header("Labyrinth Configuration")]
    [SerializeField]
    Vector2 labyrinthLengthSize = new Vector2(1f, 1f); // Size of whole map
    Vector2 prevLabSize;
    Vector2 prevGrid;

    [Header("Meshes and Materials")]
    [SerializeField]
    Mesh wallMesh;
    [SerializeField]
    Material texture;

    [Header("Randomization")]
    [SerializeField]

    int seed = 1234;
    int prevSeed;

    // Other Vars
    int wallCountX;
    int wallCountY;
    float wallSpacing = 4.0f;
    List<Matrix4x4> wallMatrices;

    Matrix4x4[] wallMatrixArray;
    List<Matrix4x4[]> wallMatrixArrayList;

    private Vector2 labyrinthSize;

    // Start is called before the first frame update
    void Start()
    {
        AssignLabyrinthSize();
        prevLabSize = labyrinthSize;
        prevSeed = seed;
        texture.enableInstancing = true;
        CreateWalls();

    }
    // Update is called once per frame
    void Update()
    {
        AssignLabyrinthSize();
        if (labyrinthSize != prevLabSize || seed != prevSeed || prevGrid != Grid)
        {
            prevGrid = Grid;
            prevLabSize = labyrinthSize;
            prevSeed = seed;
            CreateWalls();
        }
        RenderWalls();
    }

    // Convert the user given size to a size that is compatible with the meshes
    private void AssignLabyrinthSize()
    {
        labyrinthSize = labyrinthLengthSize * wallSpacing;
    }
    #region Wall Generation
    struct WallConfig
    {
        public int Count;
        public Vector3 StartPosition;
        public Vector3 Direction;
        public Quaternion Rotation;
    }

    void CreateWalls()
    {
        // Initial Seed
        Random.InitState(seed);

        wallMatrices = new List<Matrix4x4>();
        wallMatrixArrayList = new List<Matrix4x4[]>();

        int labyrinthSizeNewX = ((int)labyrinthSize.x) / 2;
        int labyrinthSizeNewY = ((int)labyrinthSize.y) / 2;

        wallCountX = labyrinthSizeNewX >= 2 ? labyrinthSizeNewX / 2 : 1;
        wallCountY = labyrinthSizeNewY >= 2 ? labyrinthSizeNewY / 2 : 1;

        for (int i = 0; i < (int)Grid.x; i++)
        {
            for (int j = 0; j < (int)Grid.y; j++)
            {
                // Offset each grid cell
                // Vector3 gridOffset = new Vector3(i * (labyrinthSizeNewX + wallSpacing), 0, j * (labyrinthSizeNewY + wallSpacing));
                Vector3 gridOffset = new Vector3(i * (labyrinthSize.x - 2 + wallSpacing), 0, j * (labyrinthSize.y - 2 + wallSpacing));

                // Define wall axes configurations
                var wallConfigs = new List<WallConfig>
                {
                    new WallConfig
                    {
                        Count = wallCountX,
                        StartPosition = transform.position + gridOffset + new Vector3(-(labyrinthSizeNewX - 2), 0, 0),
                        Direction = Vector3.right,
                        Rotation = Quaternion.identity
                    },
                    new WallConfig
                    {
                        Count = wallCountY,
                        StartPosition = transform.position + gridOffset + new Vector3(-labyrinthSizeNewX - 0.5f, 0, 2.5f),
                        Direction = Vector3.forward,
                        Rotation = Quaternion.Euler(0f, 90f, 0f)
                    },
                    new WallConfig
                    {
                        Count = wallCountX,
                        StartPosition = transform.position + gridOffset + new Vector3(-(labyrinthSizeNewX - 2), 0, wallCountY * wallSpacing + 1f),
                        Direction = Vector3.right,
                        Rotation = Quaternion.identity
                    },
                    new WallConfig
                    {
                        Count = wallCountY,
                        StartPosition = transform.position + gridOffset + new Vector3(labyrinthSizeNewX + 0.5f, 0, 2.5f),
                        Direction = Vector3.forward,
                        Rotation = Quaternion.Euler(0f, 90f, 0f)
                    }
                };

                // Generate walls for the current grid cell
                foreach (var config in wallConfigs)
                {
                    for (int k = 0; k < config.Count; k++)
                    {
                        Vector3 wallTransform = config.StartPosition + config.Direction * (wallSpacing * k);
                        Matrix4x4 mat = Matrix4x4.TRS(wallTransform, config.Rotation, Vector3.one);
                        wallMatrices.Add(mat);
                    }
                }
            }
        }

        // Convert to array and add to list
        wallMatrixArray = wallMatrices.ToArray();
        wallMatrixArrayList.Add(wallMatrixArray);
    }

    void RenderWalls()
    {
        foreach(Matrix4x4[] wallMatrixArray_ in wallMatrixArrayList)
        {
            if (wallMatrixArray_.Length > 0)
            {
                Graphics.DrawMeshInstanced(wallMesh, 0, texture, wallMatrixArray_, wallMatrixArray_.Length);
            }
        }
    }
    #endregion
}
