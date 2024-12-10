using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

// using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;

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
    // [SerializeField]
    public Mesh wallMesh;
    public Mesh floorMesh;

    public Material stone;

    [SerializeField]
    Material texture;

    [Header("Randomization")]
    [SerializeField]
    public bool random_seed = false;
    public int seed = 1234;
    int prevSeed;

    // Other Vars
    int wallCountX;
    int wallCountY;
    float wallSpacing = 4.0f;
    List<Matrix4x4> wallMatrices;
    List<Matrix4x4> floor;

    Matrix4x4[] wallMatrixArray;
    // List<Matrix4x4[]> wallMatrixArrayList;

    private Vector2 labyrinthSize;

    // Start is called before the first frame update
    void Start()
    {
        EnableInstancingForMaterial(stone);
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
            if(random_seed)
            {
                seed = UnityEngine.Random.Range(0,(int)Math.Pow(2,29));
            }
            prevGrid = Grid;
            prevLabSize = labyrinthSize;
            prevSeed = seed;
            CreateWalls();
            CreateFloor();
        }
        RenderWalls();
        RenderFloor();
    }
    // add GPU instancing to material
    void EnableInstancingForMaterial(Material material)
    {
        if (material != null && !material.enableInstancing)
        {
            material.enableInstancing = true;
            Debug.Log("GPU Instancing enabled for material: " + material.name);
        }
        else if (material == null)
        {
            Debug.LogError("Material is null. Cannot enable GPU Instancing.");
        }
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
    private List<Vector2Int> getChildren(Vector2Int parent,List<parentchildNode> path)
    {
        List<Vector2Int> children = new List<Vector2Int>();
        foreach(parentchildNode node in path)
        {
            if(node.parent == parent && node.child != node.parent)
            {
                children.Add(node.child);
            }
        }
        return children;
    }
    private List<Vector2Int> getParent(Vector2Int child,List<parentchildNode> path)
    {
        List<Vector2Int> parents = new List<Vector2Int>();
        foreach(parentchildNode node in path)
        {
            if(node.child == child && node.child != node.parent)
            {
                parents.Add(node.parent);
            }
        }
        return parents;
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
            int index = UnityEngine.Random.Range(0,Moves.Count);
            Vector2Int move_made = Moves[index];
            Moves.RemoveAt(index);
            if(possible.Contains(move_made))
            {
                possible.Remove(move_made);
                List<parentchildNode> path = MakePath(current, move_made,possible);
                list.AddRange(path);
            }
            else if(!possible.Contains(move_made) && UnityEngine.Random.Range(0,100) < 10)
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
        UnityEngine.Random.InitState(seed);
        List<parentchildNode> paths = Get_Paths();

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
                Vector2Int current = new Vector2Int(i,j);
                List<Vector2Int> parents = getParent(current, paths);
                List<Vector2Int> children = getChildren(current, paths);
                List<Vector2Int> destinations = new List<Vector2Int>();
                destinations.AddRange(parents);
                parents = null;
                destinations.AddRange(children);
                children = null;
                List<Vector2Int> removal = new List<Vector2Int>();

                foreach(Vector2Int destination in destinations)
                {
                    var pos = destination - current;
                    if(!removal.Contains(pos))
                    {
                        removal.Add(pos);
                    }
                }

                if(removal.Contains(new Vector2Int(1,0)))
                {
                    wallConfigs.RemoveAt(3);
                }
                if(removal.Contains(new Vector2Int(0,1)))
                {
                    wallConfigs.RemoveAt(2);
                }
                if(removal.Contains(new Vector2Int(-1,0)))
                {
                    wallConfigs.RemoveAt(1);
                }
                if(removal.Contains(new Vector2Int(0,-1)))
                {
                    wallConfigs.RemoveAt(0);
                }

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
        Add_WallColliders();
        // wallMatrixArrayList.Add(wallMatrixArray);
    }

    private void Add_WallColliders()
    {
        // Simple Wall Collider
        // Parent Wall Collider object
        GameObject wallColliders = GameObject.Find("Wall Matrix Colliders");
        if(wallColliders != null)
        {
            int i = 0;
            GameObject wallColliders_child;
            while(true)
            {
                wallColliders_child = GameObject.Find($"Wall Matrix Collider_{i}");
                if(wallColliders_child != null)
                {
                    Destroy(wallColliders_child);
                    i++;
                    continue;
                }
                break;
            }
            Destroy(wallColliders);
        }
        wallColliders = new GameObject("Wall Matrix Colliders");
        wallColliders.transform.parent = this.transform;
        for (int i = 0; i < wallMatrixArray.Length; i++)
        {
            Vector3 position = wallMatrixArray[i].GetColumn(3);
            Debug.Log("Position " + position.ToString());
            Quaternion rotation = Quaternion.LookRotation(wallMatrixArray[i].GetColumn(2), wallMatrixArray[i].GetColumn(1));

            // Create a child GameObject for each wall collider
            GameObject wallColliderObj = new GameObject($"Wall Matrix Collider_{i}");
            wallColliderObj.tag = "Wall";
            wallColliderObj.transform.parent = wallColliders.transform;
            wallColliderObj.transform.position = position;
            wallColliderObj.transform.rotation = rotation;

            BoxCollider boxColliderWall = wallColliderObj.AddComponent<BoxCollider>();
            boxColliderWall.size = new Vector3(4.0f, 4.0f, 1.0f);
            boxColliderWall.center = new Vector3(0.0f, 2.0f, 0.0f);
        }
    }

    void RenderWalls()
    {
        if (wallMatrixArray.Length > 0)
        {
            Graphics.DrawMeshInstanced(wallMesh, 0, texture, wallMatrixArray, wallMatrixArray.Length);
        }
    }
    #endregion

    #region Floor Generation
    void CreateFloor()
    {
        int gridSizeX = wallCountX * 2 + 1;
        int gridSizeY = wallCountY * 2 + 1;

        floor = new List<Matrix4x4>();
        for(int a = 0; a < Grid.x; a++)
        {
            for(int b = 0; b < Grid.y; b++)
            {
                for (int i = 0; i < gridSizeY; i++)
                {
                    for (int j = 0; j < gridSizeX; j++)
                    {
                        Vector3 gridOffset = new Vector3(a * (labyrinthSize.x - 2 + wallSpacing), 0, b * (labyrinthSize.y - 2 + wallSpacing));

                        Vector3 tileTransform = gridOffset + new Vector3(-wallSpacing*labyrinthLengthSize.x / 2 + 2 * j, -1, 0.5f + 2 * i);

                        Matrix4x4 mat = Matrix4x4.TRS(tileTransform, transform.rotation, new Vector3(1, 1, 1));

                        floor.Add(mat);
                    }
                }
            }
        }
        Create_Floor_Collider();
    }

    private void Create_Floor_Collider_()
    {
        GameObject floor_Collider = GameObject.Find("Floor Collider");
        if (floor_Collider != null)
        {
            Destroy(floor_Collider);
        }
        floor_Collider = new GameObject("Floor Collider");

        // Calculate grid size based on wallCountX and wallCountY
        int gridSizeX = wallCountX * 2 + 1;
        int gridSizeY = wallCountY * 2 + 1;

        // Calculate the total floor dimensions
        float totalWidth = gridSizeX * Grid.x * 2 * labyrinthLengthSize.x;
        float totalHeight = gridSizeY * Grid.y * 2 * labyrinthLengthSize.y;

        // Position the floor collider to cover the entire floor area
        floor_Collider.transform.position = new Vector3(
            this.transform.position.x + (totalWidth - wallSpacing) / 2,
            this.transform.position.y - 1,
            this.transform.position.z + (totalHeight - wallSpacing) / 2
        );

        // Add a BoxCollider and set its size
        BoxCollider boxCollider = floor_Collider.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(totalWidth, 1, totalHeight); // Adjust height if needed
    }
    private void Create_Floor_Collider()
    {
        GameObject floor_Collider = GameObject.Find("Floor Collider");
        if(floor_Collider != null)
        {
            Destroy(floor_Collider);
        }
        floor_Collider = new GameObject("Floor Collider");
        floor_Collider.tag = "Floor";
        floor_Collider.transform.parent = this.transform;
        floor_Collider.transform.position = new Vector3(this.transform.position.x + ((Grid.x - 1) * wallSpacing * labyrinthLengthSize.x)/2 + (Grid.x - 1), this.transform.position.y-1, this.transform.position.z + labyrinthLengthSize.y * Grid.y * wallSpacing /2 + (Grid.y-1));
        // floorcollider = floor_Collider.AddComponent<BoxCollider>();
        floor_Collider.AddComponent<BoxCollider>();
        var scale = floor_Collider.transform.localScale;
        scale.x *= Grid.x * wallSpacing * labyrinthLengthSize.x + (Grid.x - 1) * wallSpacing / 2;
        scale.z *= Grid.y * wallSpacing * labyrinthLengthSize.y + (Grid.y - 1) * wallSpacing / 2;
        floor_Collider.transform.localScale = scale;
    }
    // private BoxCollider floorcollider;
    // private void OnDrawGizmos()
    // {
    //     if (floorcollider != null)
    //     {
    //         // Set Gizmos color
    //         Gizmos.color = new Color(0, 1, 0, 0.3f); // Semi-transparent green

    //         // Calculate the floorcollider's world position and size
    //         Vector3 center = floorcollider.transform.TransformPoint(floorcollider.center);
    //         Vector3 size = Vector3.Scale(floorcollider.size, floorcollider.transform.lossyScale);

    //         // Draw a cube at the collider's position with its size
    //         Gizmos.DrawCube(center, size);

    //         // Optional: Draw the outline
    //         Gizmos.color = Color.green; // Solid green
    //         Gizmos.DrawWireCube(center, size);
    //     }
    // }

    void RenderFloor()
    {
        if (floor.Count > 0)
        {
            Matrix4x4[] floorArray = floor.ToArray();
            Graphics.DrawMeshInstanced(floorMesh, 0, stone, floorArray, floorArray.Length);
        }
    }
    #endregion

}
