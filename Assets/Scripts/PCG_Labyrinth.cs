using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PCG_Labyrinth : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField]
    Vector2Int Grid = new Vector2Int(10, 10); // Size of whole map
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
    // List<Matrix4x4[]> wallMatrixArrayList;

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

    private class parentchildNode
    {
        public Vector2Int parent;
        public Vector2Int child;
        public parentchildNode(Vector2Int parent_, Vector2Int child_)
        {
            parent = parent_;
            child = child_;
        }
    }

    private List<parentchildNode> MakePath(Vector2Int parent, Vector2Int current, List<Vector2Int> possible)
    {
        List<parentchildNode> list = new List<parentchildNode>();
        if(possible.Count == 0)
        {
            parentchildNode node = new parentchildNode(parent,current);
            list.Add(node);
            return list;
        }
        List<Vector2Int> Moves = new List<Vector2Int>();
        for(int i = -1; i <= 1; i += 2)
        {
            for(int j = 0; j <= 1; j ++)
            {
                int x_ = 0, y_ = 0;
                if(j == 0)
                {
                    x_ = i;
                }
                else
                {
                    y_ = i;
                }
                Vector2Int newMove = current + new Vector2Int(x_,y_);
                if(newMove.x < 0 || newMove.x >= Grid.x)
                {
                    continue;
                }
                if(newMove.y < 0 || newMove.y >= Grid.y)
                {
                    continue;
                }
                Moves.Add(newMove);
            }
        }
        while(Moves.Count > 0)
        {
            int index = Random.Range(0,Moves.Count);
            Vector2Int move_made = Moves[index];
            Moves.RemoveAt(index);
            if(possible.Contains(move_made))
            {
                possible.Remove(move_made);
                List<parentchildNode> path = MakePath(current, move_made,possible);
                list.AddRange(path);
            }
            else if(!possible.Contains(move_made) && Random.Range(0,100) < 10)
            {
                list.Add(new parentchildNode(current,move_made));
            }
        }
        list.Add(new parentchildNode(parent,current));
        return list;
    }

    private List<parentchildNode> Get_Paths()
    {
        List<Vector2Int> possible = new List<Vector2Int>();

        for(int i = 0; i < Grid.x; i++)
        {
            for(int j = 0; j < Grid.y; j++)
            {
                possible.Add(new Vector2Int(i,j));
            }   
        }
        List<parentchildNode> result = MakePath(new Vector2Int(0,0),new Vector2Int(0,0),possible);
        // if(possible.Count>0)
        // {
        //     Debug.Log("Not all possibilities covered... " + possible.Count.ToString() + "remain");
        // }
        // else
        // {
        //     Debug.Log("All possibilities covered");
        // }
        return result;
    }

    void CreateWalls()
    {
        // Initial Seed
        Random.InitState(seed);
        Get_Paths();

        wallMatrices = new List<Matrix4x4>();
        // wallMatrixArrayList = new List<Matrix4x4[]>();

        int labyrinthSizeNewX = ((int)labyrinthSize.x) / 2;
        int labyrinthSizeNewY = ((int)labyrinthSize.y) / 2;

        wallCountX = labyrinthSizeNewX >= 2 ? labyrinthSizeNewX / 2 : 1;
        wallCountY = labyrinthSizeNewY >= 2 ? labyrinthSizeNewY / 2 : 1;

        for (int i = 0; i < Grid.x; i++)
        {
            for (int j = 0; j < Grid.y; j++)
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
        // wallMatrixArrayList.Add(wallMatrixArray);
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
