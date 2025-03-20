using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Floor
{
    public int nRooms;

    public Room baseRoom;
    public Transform parent;
    public Room[] rooms;
    int floorNumber;
    public Ceiling[,] ceilings;
    public GameObject ceilingParent;
    // Define the 2D array for possible door positions for each room
    int[][] roomDoors = new int[9][]
    {
        new int[] { 0, 3 },        // Room 0
        new int[] { 0, 1, 3 },     // Room 1
        new int[] { 0, 1 },        // Room 2
        new int[] { 0, 2, 3 },     // Room 3
        new int[] { 0, 1, 2, 3 },  // Room 4
        new int[] { 0, 1, 2 },     // Room 5
        new int[] { 0, 2, 3 },     // Room 6
        new int[] { 0, 1, 2, 3, 4 }, // Room 7
        new int[] { 0, 1, 2 }      // Room 8
    };

    public Floor(int _floorNumber, Transform _parent, DecorationAsset[] _decorationAssets, int width, int length)
    {
        floorNumber = _floorNumber;
        nRooms = Random.Range(4, 8);
        rooms = new Room[nRooms];
        parent = _parent;
        int[] _doorWall = new int[1];
        _doorWall[0] = 0;
        baseRoom = new Room(floorNumber, -1, _doorWall, width, length, new Vector3(0f,4*_floorNumber,0f), parent, _decorationAssets, true);

        ceilings = new Ceiling[length + 1, width + 1];
        ceilingParent = new GameObject("Ceilings");
        ceilingParent.transform.SetParent(baseRoom.room.transform);

        CreateCeilings(width,length);
        CreateRooms(_decorationAssets);
        MarkLastFourRowsForbidden();
        baseRoom.PlaceDecorations();
    }
    int[] PickUniqueNumbers(int n, int min, int max)
    {
        List<int> numbers = new List<int>();
        for (int i = min; i <= max; i++)
        {
            numbers.Add(i);
        }

        // Fisher-Yates shuffle to randomize
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
        }

        // Take the first `n` unique numbers
        return numbers.GetRange(0, n).ToArray();
    }

    void CreateCeilings(int width,int length)
    {
        for (int i = 0; i < ceilings.GetLength(0); i++)
        {
            for (int j = 0; j < ceilings.GetLength(1); j++)
            {
                ceilings[i, j] = new Ceiling(floorNumber,Vector3.zero, width, length, i, j);
            }
        }
    }
    void CreateRooms(DecorationAsset[] _decorationAssets)
    {
        int twoWall = 6, threeWall = 8, fourWall = 10;
        int[] rowStarts = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] colStarts = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        PossibleRoomPos[] possibleRooms = new PossibleRoomPos[9];

        for (int i = 0; i < 9; i++)
        {
            // Debug.Log("Room_" + i.ToString());
            int startRow = rowStarts[i];
            int startCol = colStarts[i];

            // Debug.Log("Start Row " + startRow.ToString());
            // Debug.Log("Start Col " + startCol.ToString());

            int _width, _length;
            int centerRow;
            int centerCol;

            _width = (Random.Range(0, 2) == 0) ? threeWall : fourWall;
            if (startCol + _width - 1 > baseRoom.ground.GetLength(1) - 1)
            {
                _width = (Random.Range(0, 2) == 0) ? threeWall : twoWall;
                if (startCol + _width - 1 > baseRoom.ground.GetLength(1) - 1)
                {
                    _width = twoWall;
                }
            }

            _length = (Random.Range(0, 2) == 0) ? threeWall : fourWall;
            if (startRow + _length - 1 > baseRoom.ground.GetLength(0) - 4)
            {
                _length = (Random.Range(0, 2) == 0) ? threeWall : twoWall;
                if (startRow + _length - 1 > baseRoom.ground.GetLength(0) - 4)
                {
                    _length = twoWall;
                }
            }

            // Debug.Log("Width " + _width.ToString());
            // Debug.Log("Length " + _length.ToString());

            centerRow = startRow + _length / 2;
            centerCol = startCol + _width / 2;
            if (i % 3 == 2)
                centerCol = baseRoom.ground.GetLength(1) - 1 - _width / 2 + 1;

            // Debug.Log("Center Row " + centerRow.ToString());
            // Debug.Log("Center Col " + centerCol.ToString());
            // Debug.Log(baseRoom.ground[centerRow, centerCol].position);

            possibleRooms[i] = new PossibleRoomPos(
                                baseRoom.ground[centerRow, centerCol], i + 1, (_width - 2) / 2, (_length - 2) / 2, centerRow, centerCol);

            if(floorNumber > 0){
                // Debug.Log(possibleRooms[i].center.position);
            }
            if (i + 1 < 9)
            {
                if (rowStarts[i + 1] == 0)
                    rowStarts[i + 1] = startRow;
                if (i + 3 < 9 && rowStarts[i + 3] == 0)
                    rowStarts[i + 3] = startRow + _length + 3;
                colStarts[i + 1] = startCol + _width + 3;
                if (i % 3 == 2)
                    colStarts[i + 1] = 0;
            }
        }
        int[] randomNumbers = PickUniqueNumbers(nRooms, 0, 8);
        int i_real = 0;
        int hallwayWidth = 2;
        // Mark Hallway Around center
        void MarkHallway(int centerRow, int centerCol, int w, int h)
        {
            // Get grid dimensions
            int rows = baseRoom.ground.GetLength(0);
            int cols = baseRoom.ground.GetLength(1);

            // 1. Mark the inside of the room
            for (int row = centerRow - (h / 2); row <= centerRow + (h / 2); row++)
            {
                if (row >= 0 && row < rows) // Check bounds
                {
                    for (int col = centerCol - (w / 2); col <= centerCol + (w / 2); col++)
                    {
                        if (col >= 0 && col < cols) // Check bounds
                        {
                            baseRoom.ground[row, col].MarkForbidden();
                        }
                    }
                }
            }

            // Top hallway
            for (int row = 1; row <= hallwayWidth; row++)
            {
                int hallwayRow = centerRow - (h / 2) - (row - 1);
                if (hallwayRow >= 0) // Check bounds
                {
                    for (int col = centerCol - (w / 2); col <= centerCol + (w / 2); col++)
                    {
                        if (col >= 0 && col < cols) // Check bounds
                        {
                            baseRoom.ground[hallwayRow, col].MarkForbidden();
                        }
                    }
                }
            }

            // Bottom hallway
            for (int row = 1; row <= hallwayWidth; row++)
            {
                int hallwayRow = centerRow + (h / 2) + (row - 1);
                if (hallwayRow < rows) // Check bounds
                {
                    for (int col = centerCol - (w / 2); col <= centerCol + (w / 2); col++)
                    {
                        if (col >= 0 && col < cols) // Check bounds
                        {
                            baseRoom.ground[hallwayRow, col].MarkForbidden();
                        }
                    }
                }
            }

            // Left hallway
            for (int col = 1; col <= hallwayWidth; col++)
            {
                int hallwayCol = centerCol - (w / 2) - (col - 1);
                if (hallwayCol >= 0) // Check bounds
                {
                    for (int row = centerRow - (h / 2); row <= centerRow + (h / 2); row++)
                    {
                        if (row >= 0 && row < rows) // Check bounds
                        {
                            baseRoom.ground[row, hallwayCol].MarkForbidden();
                        }
                    }
                }
            }

            // Right hallway
            for (int col = 1; col <= hallwayWidth; col++)
            {
                int hallwayCol = centerCol + (w / 2) + (col - 1);
                if (hallwayCol < cols) // Check bounds
                {
                    for (int row = centerRow - (h / 2); row <= centerRow + (h / 2); row++)
                    {
                        if (row >= 0 && row < rows) // Check bounds
                        {
                            baseRoom.ground[row, hallwayCol].MarkForbidden();
                        }
                    }
                }
            }

        }

        foreach (int i in randomNumbers)
        {
            if (possibleRooms[i] == null) continue;

            MarkHallway(possibleRooms[i].centerRow, possibleRooms[i].centerCol, possibleRooms[i].width * 2 + 2, possibleRooms[i].length * 2 + 2);

            // Create a new room instance
            rooms[i_real] = new Room(
                floorNumber,
                possibleRooms[i].posNumber,
                roomDoors[i], // Default doorWall (can be updated later if needed)
                possibleRooms[i].width,
                possibleRooms[i].length,
                possibleRooms[i].center.position + new Vector3(-1f, 0f, 1f),
                baseRoom.room.transform,
                _decorationAssets,false
            );
            if(floorNumber > 0){
                // Debug.Log(rooms[i_real].center);
            }
            i_real++;
        }
    }
    void MarkLastFourRowsForbidden()
    {
        int rows = baseRoom.ground.GetLength(0); // Get total rows
        int cols = baseRoom.ground.GetLength(1); // Get total columns

        // Ensure we don't go out of bounds
        int startRow = Mathf.Max(0, rows - 4);

        for (int row = startRow; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                baseRoom.ground[row, col].MarkForbidden();
            }
        }
    }

}
