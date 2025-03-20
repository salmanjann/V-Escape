using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;
using System.Linq;
public class Room
{
    public int width, length, roomNumber, floorNumber;
    int[] doorWalls;
    public GameObject room;
    public Transform parent;
    public Vector3 center;
    public Corner[] corners;
    public Ground[,] ground;
    public Wall[] walls;
    public DecorationAsset[] decorationAssets;
    public List<Decoration> props = new List<Decoration>();

    public Transform cornerParent, floorParent, wallParent, decorationParent;

    public bool isBase = false;

    public Room() { room = null; }
    public Room(int _floorNumber, int _roomNumber, int[] _doorWalls, int _width, int _length, Vector3 _center, Transform _parent, DecorationAsset[] _decorationAssets, bool _isBase = false)
    {
        floorNumber = _floorNumber;
        roomNumber = _roomNumber;
        width = _width;
        length = _length;
        center = _center;
        doorWalls = _doorWalls;

        parent = _parent;
        if (_roomNumber != -1)
            room = new GameObject($"Room_{roomNumber}");
        else
            room = new GameObject($"Floor_{_floorNumber + 1}");
        room.transform.SetParent(parent);

        corners = new Corner[4];
        walls = new Wall[4];
        ground = new Ground[length * 2 + 2, width * 2 + 2];

        decorationAssets = _decorationAssets;
        isBase = _isBase;

        CreateRoom();
    }

    void CreateRoom()
    {
        CreateGround();
        CreateCorners();
        CreateWalls();
        if (!isBase)
            PlaceDecorations();
    }
    void CreateCorners()
    {
        cornerParent = CreateParent("Corners", room.transform);
        // if (isInsideRoom){
            // Debug.Log("Floor Number " + floorNumber.ToString());
            // Debug.Log("Room Center" + center.ToString());
        // }
        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = new Corner(floorNumber, center, width, length, i);
            // if (isInsideRoom)
                // Debug.Log("Corner " + corners[i].position.ToString());
        }
    }

    void CreateGround()
    {
        floorParent = CreateParent("Ground", room.transform);
        for (int i = 0; i < ground.GetLength(0); i++)
        {
            for (int j = 0; j < ground.GetLength(1); j++)
            {
                ground[i, j] = isBase == true ? new Ground(floorNumber, center, width, length, i, j, true) : new Ground(floorNumber, center, width, length, i, j);
            }
        }
    }

    void CreateWalls()
    {
        int maxDoors = doorWalls.Length; // Max available doors
        int n = Random.Range(1, maxDoors);

        List<int> selectedDoors = GetRandomDoors(doorWalls, n);
        wallParent = CreateParent("Walls", room.transform);
        for (int i = 0; i < 4; i++)
        {
            int nWalls = (i == 0 || i == 2) ? width : length;
            if (selectedDoors.Contains(i) && isBase)
            {
                walls[i] = new Wall(floorNumber, corners[i].position, nWalls, i, true, true);
            }
            else if (selectedDoors.Contains(i) && !isBase)
            {
                walls[i] = new Wall(floorNumber, corners[i].position, nWalls, i, true);
            }
            else if (isBase)
            {
                walls[i] = new Wall(floorNumber, corners[i].position, nWalls, i, false, true);
            }
            else if (!isBase)
            {
                walls[i] = new Wall(floorNumber, corners[i].position, nWalls, i);
            }

            for (int j = 0; j < nWalls; j++)
            {
                if (walls[i].row[j] == 0 || walls[i].row[j] == 2 || (walls[i].row[j] == 1 && walls[i].col[j] > 2))
                {
                    int forbiddenTileI = 0, forbiddenTileJ = 0;
                    switch (i)
                    {
                        case 0:
                            forbiddenTileI = length * 2 + 1;
                            forbiddenTileJ = j * 2 + 1;
                            ground[forbiddenTileI, forbiddenTileJ].MarkForbidden();
                            ground[forbiddenTileI, forbiddenTileJ + 1].MarkForbidden();
                            break;
                        case 1:
                            forbiddenTileJ = 0;
                            forbiddenTileI = j * 2 + 1;
                            ground[forbiddenTileI, forbiddenTileJ].MarkForbidden();
                            ground[forbiddenTileI + 1, forbiddenTileJ].MarkForbidden();
                            break;
                        case 2:
                            forbiddenTileI = 0;
                            forbiddenTileJ = width * 2 + 1 - (j * 2 + 1);
                            ground[forbiddenTileI, forbiddenTileJ].MarkForbidden();
                            ground[forbiddenTileI, forbiddenTileJ - 1].MarkForbidden();
                            break;
                        case 3:
                            forbiddenTileJ = width * 2 + 1;
                            forbiddenTileI = length * 2 + 1 - (j * 2 + 1);
                            ground[forbiddenTileI, forbiddenTileJ].MarkForbidden();
                            ground[forbiddenTileI - 1, forbiddenTileJ].MarkForbidden();
                            break;
                    }
                }

            }

        }
    }
    Transform CreateParent(string name, Transform parent = null)
    {
        GameObject obj = new GameObject(name);
        if (parent) obj.transform.SetParent(parent);
        return obj.transform;
    }

    List<int> GetRandomDoors(int[] availableDoors, int n)
    {
        List<int> shuffledDoors = new List<int>(availableDoors);
        List<int> selectedDoors = new List<int>();

        // Fisher-Yates shuffle to randomize door selection
        for (int i = shuffledDoors.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffledDoors[i], shuffledDoors[j]) = (shuffledDoors[j], shuffledDoors[i]);
        }

        // Take the first 'n' doors from the shuffled list
        for (int i = 0; i < n; i++)
        {
            selectedDoors.Add(shuffledDoors[i]);
        }

        return selectedDoors;
    }


    public void PlaceDecorations()
    {
        decorationParent = CreateParent("Decorations", room.transform);
        Dictionary<Ground.Zone, float> zoneChances = new Dictionary<Ground.Zone, float>
        {
            { Ground.Zone.Inside, 0.7f },
            { Ground.Zone.Outside, 0.9f },
            { Ground.Zone.Forbidden, -1f },
            { Ground.Zone.Hallway, 0.5f }
        };
        List<Ground> availablefloor = new List<Ground>();
        for (int x = 0; x < ground.GetLength(0); x++)
        {
            for (int y = 0; y < ground.GetLength(1); y++)
            {
                if (ground[x, y].zoneTag != Ground.Zone.Forbidden)
                    availablefloor.Add(ground[x, y]);
            }
        }

        foreach (var cell in availablefloor)
        {
            float zoneChance = zoneChances[cell.zoneTag];
            if (Random.Range(0f, 1f) <= zoneChance)
            {
                var possibleElements = decorationAssets.Where(x => x.zone == cell.zoneTag).ToList();
                if (possibleElements.Count > 0)
                {
                    DecorationAsset decoration = PickOneAsset(possibleElements);
                    if (!IsOverlap(cell, decoration.area))
                    {
                        RemoveArea(cell, decoration.area);
                        props.Add(new Decoration(cell.position, cell.rotation, decoration, cell.getType(), decoration.offset));
                    }
                }
            }
        }
    }

    DecorationAsset PickOneAsset(List<DecorationAsset> possibleElements)
    {
        float totalWeight = possibleElements.Sum(asset => asset.chances);
        float randomValue = Random.Range(0, totalWeight);
        float cumulative = 0;

        foreach (var asset in possibleElements)
        {
            cumulative += asset.chances;
            if (randomValue <= cumulative)
                return asset;
        }

        return possibleElements[0];
    }

    bool IsOverlap(Ground tile, Vector2 area)
    {
        int i = tile.getI(), j = tile.getJ(), type = tile.getType();

        if (area.x == 1 && area.y == 0)
        {
            tile.MarkForbidden();
            return false;
        }

        if (type == -1)
        {
            int[] rotations = { 0, 1, 2, 3 };
            type = rotations[Random.Range(0, rotations.Length)];
            tile.SetRotation(Quaternion.Euler(0f, 90f * type, 0f));
        }

        int[,] offsets = (area.x, area.y) switch
        {
            (2, 2) => GetOffsetsForType(type, 2),
            (3, 3) => GetOffsetsForType(type, 3),
            _ => null
        };

        if (offsets == null)
            return true;

        for (int index = 0; index < offsets.GetLength(0); index++)
        {
            int tileX = i + offsets[index, 0],
                tileZ = j + offsets[index, 1];

            if (tileX < 0 || tileX >= ground.GetLength(0) || tileZ < 0 || tileZ >= ground.GetLength(1) ||
                ground[tileX, tileZ].zoneTag == Ground.Zone.Forbidden)
                return true;
        }
        return false;
    }

    void RemoveArea(Ground tile, Vector2 area)
    {
        tile.MarkForbidden();
        int i = tile.getI(), j = tile.getJ(), type = tile.getType();
        int[,] offsets = (area.x, area.y) switch
        {
            (2, 2) => GetOffsetsForType(type, 2),
            (3, 3) => GetOffsetsForType(type, 3),
            _ => null
        };

        if (offsets == null)
            return;

        for (int index = 0; index < offsets.GetLength(0); index++)
        {
            int tileX = i + offsets[index, 0],
                tileZ = j + offsets[index, 1];
            if (tileX >= 0 && tileX < ground.GetLength(0) && tileZ >= 0 && tileZ < ground.GetLength(1))
                ground[tileX, tileZ].MarkForbidden();
        }
    }


    void DestroyRoom()
    {
        // if(room!=null)
        //     Destroy()
    }

    int[,] GetOffsetsForType(int type, int size)
    {
        if (size == 2)
        {
            return type switch
            {
                0 => new int[,] { { 0, 0 }, { 1, 0 }, { 0, 1 }, { 1, 1 } },
                1 => new int[,] { { 0, 0 }, { 0, 1 }, { -1, 0 }, { -1, 1 } },
                2 => new int[,] { { 0, 0 }, { -1, 0 }, { 0, -1 }, { -1, -1 } },
                3 => new int[,] { { 0, 0 }, { 0, -1 }, { 1, 0 }, { 1, -1 } },
                _ => new int[,] { { 0, 0 }, { 1, 0 }, { 0, 1 }, { 1, 1 } }
            };
        }
        else if (size == 3)
        {
            return type switch
            {
                0 => new int[,] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } },
                1 => new int[,] { { -1, -1 }, { 0, -1 }, { 1, -1 }, { -1, 0 }, { 1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 } },
                2 => new int[,] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } },
                3 => new int[,] { { -1, -1 }, { 0, -1 }, { 1, -1 }, { -1, 0 }, { 1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 } },
                _ => new int[,] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } }
            };
        }
        return new int[0, 0];
    }
}
