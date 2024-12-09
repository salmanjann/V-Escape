using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCG_Labyrinth : MonoBehaviour
{
    [Header("Labyrinth Configuration")]
    [SerializeField]
    Vector2 labyrinthLengthSize = new Vector2(4.0f, 4.0f); // Size of whole map
    Vector2 prevLabSize;

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

    private float lengthMultiples;
    private Vector2 labyrinthSize;

    // Start is called before the first frame update
    void Start()
    {
        lengthMultiples = 4f;
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
        if (labyrinthSize != prevLabSize || seed != prevSeed)
        {
            prevLabSize = labyrinthSize;
            prevSeed = seed;
            CreateWalls();
        }
        RenderWalls();
    }

    // Convert the user given size to a size that is compatible with the meshes
    private void AssignLabyrinthSize()
    {
        labyrinthSize = labyrinthLengthSize * lengthMultiples;
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

        int labyrinthSizeNewX = ((int)labyrinthSize.x) / 2;
        int labyrinthSizeNewY = ((int)labyrinthSize.y) / 2;

        wallCountX = labyrinthSizeNewX >= 2 ? labyrinthSizeNewX / 2 : 1;
        wallCountY = labyrinthSizeNewY >= 2 ? labyrinthSizeNewY / 2 : 1;

        // Define wall axes configurations
        var wallConfigs = new List<WallConfig>
        {
            new WallConfig
            {
                Count = wallCountX,
                StartPosition = transform.position + new Vector3(-(labyrinthSizeNewX - 2), 0, 0),
                Direction = Vector3.right,
                Rotation = Quaternion.identity
            },
            new WallConfig
            {
                Count = wallCountY,
                StartPosition = transform.position + new Vector3(-labyrinthSizeNewX - 0.5f, 0, 2.5f),
                Direction = Vector3.forward,
                Rotation = Quaternion.Euler(0f, 90f, 0f)
            },
            new WallConfig
            {
                Count = wallCountX,
                StartPosition = transform.position + new Vector3(-(labyrinthSizeNewX - 2), 0, wallCountY * wallSpacing + 1f),
                Direction = Vector3.right,
                Rotation = Quaternion.identity
            },
            new WallConfig
            {
                Count = wallCountY,
                StartPosition = transform.position + new Vector3(labyrinthSizeNewX + 0.5f, 0, 2.5f),
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
                wallMatrices.Add(mat);
            }
        }

        wallMatrixArray = wallMatrices.ToArray();
    }

    void RenderWalls()
    {
        if (wallMatrixArray.Length > 0)
        {
            Graphics.DrawMeshInstanced(wallMesh, 0, texture, wallMatrixArray, wallMatrixArray.Length);
        }
    }
    #endregion
}
