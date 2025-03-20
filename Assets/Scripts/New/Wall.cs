using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall
{
    public Vector3[] positions;
    public Quaternion rotation;
    public int[] row, col;
    public Wall(int _floorNumber, Vector3 _startingPoint, int _nWalls, int _type, bool _hasDoor = false, bool _isBase = false)
    {
        // Enforce rule: If it's a base room but not the ground floor, disable doors
        if (_isBase && _floorNumber > 0)
        {
            _hasDoor = false; // Ensure no doors on non-ground base floors
        }

        // Number of wall prefabs in a wall
        positions = new Vector3[_nWalls];
        row = new int[_nWalls];
        col = new int[_nWalls];

        // Rotation of wall
        rotation = Quaternion.Euler(0f, _type * 90f, 0f);
        bool doorDone = false;

        int doorWallIndex = -1;
        int windowCount = 0; // Track the number of windows
        int abNormalWalls = 0;

        if (_hasDoor && _isBase)
        {
            // Select a random door position between the first and last wall (not at index 0 or _nWalls - 1)
            doorWallIndex = Random.Range(2, _nWalls - 2);
        }

        for (int wallNumber = 0; wallNumber < _nWalls; wallNumber++)
        {
            // **Force first and last two walls to be simple walls if it's a base room**
            if (_isBase && (wallNumber < 2 || wallNumber >= _nWalls - 2))
            {
                row[wallNumber] = 1; // Simple wall
            }
            else
            {
                // Ensure no doors are placed before `doorWallIndex` if `_isBase == true`
                if (_hasDoor && _isBase)
                {
                    if (wallNumber < doorWallIndex)
                    {
                        row[wallNumber] = Random.Range(1, 3); // Only Walls or Windows
                    }
                    else if (wallNumber == doorWallIndex)
                    {
                        row[wallNumber] = 0; // Place Door
                        doorDone = true;
                    }
                    else
                    {
                        row[wallNumber] = Random.Range(1, 3); // Continue placing Walls or Windows
                    }
                }
                else
                {
                    // Non-base floors: Apply standard logic
                    if (_hasDoor && !doorDone && wallNumber == _nWalls - 1)
                    {
                        row[wallNumber] = 0; // Force Door on Last Wall
                        doorDone = true;
                    }
                    else if (_hasDoor && !doorDone)
                    {
                        row[wallNumber] = Random.Range(0, 3);
                        if (row[wallNumber] == 0)
                        {
                            doorDone = true; // Mark Door as Done
                        }
                    }
                    else
                    {
                        row[wallNumber] = Random.Range(1, 3); // No Door, Only Walls or Windows
                    }
                }
            }

            // Limit window placement based on `isBase`
            if (row[wallNumber] == 2)
            {
                if ((_isBase && windowCount >= 5) || (!_isBase && windowCount >= 1))
                {
                    row[wallNumber] = 1; // Convert to a normal wall
                }
                else
                {
                    windowCount++; // Increase window count
                }
            }

            // Further wall type
            if (row[wallNumber] == 0)
            {
                if (_isBase)
                    col[wallNumber] = Random.Range(0, 2);
                else
                    col[wallNumber] = Random.Range(2, 4);
            }
            else if (row[wallNumber] == 1)
            {
                if (_isBase)
                    col[wallNumber] = Random.Range(0, 3);
                else
                {
                    col[wallNumber] = Random.Range(0, 7);
                    if (col[wallNumber] > 2)
                    {
                        if (abNormalWalls >= 1)
                        {
                            col[wallNumber] = Random.Range(0, 3);
                        }
                        else
                        {
                            abNormalWalls++;
                        }
                    }
                }
            }
            else if (row[wallNumber] == 2)
            {
                col[wallNumber] = Random.Range(0, 9);
            }

            positions[wallNumber] = _startingPoint;

            // Displacement
            int xSign = _type == 0 ? 1 : -1;
            int zSign = _type == 1 ? -1 : 1;

            int x = _type == 0 || _type == 2 ? xSign * (wallNumber * 4 + 4) : 0;
            int z = _type == 1 || _type == 3 ? zSign * (wallNumber * 4 + 4) : 0;

            positions[wallNumber].x += x;
            positions[wallNumber].z += z;
        }
    }
}
