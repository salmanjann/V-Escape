using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground
{
    public enum Zone { Inside, Outside, Forbidden, Hallway }
    public Vector3 position;
    public Zone zoneTag;
    public Quaternion rotation;
    int type = -1;
    int i, j;
    public Ground(int _floorNumber, Vector3 _center, int _width, int _length, int _i, int _j, bool _isBase = false)
    {
        i = _i;
        j = _j;
        int new_width = _width * 4 / 2;
        int new_length = _length * 4 / 2;
        Vector3 topLeftCorner = new Vector3(_center.x - 1 - new_width,  _floorNumber * 4, _center.z + new_length + 1);
        position = topLeftCorner + new Vector3(_j * 2,0f , -(_i * 2));

        if (_isBase)
        {
            zoneTag = Zone.Hallway;
        }
        else
        {
            // Assigning zone
            if (_i == 0 || _j == 0 || _i == _length * 2 + 1 || _j == _width * 2 + 1)
            {
                zoneTag = Zone.Outside;
            }
            else
            {
                zoneTag = Zone.Inside;
            }

            // Rotation for outside tiles
            if (_j == 0)
            {
                type = 1;
            }
            else if (_i == 0)
            {
                type = 2;
            }
            else if (_j == _width * 2 + 1)
            {
                type = 3;
            }
            else if (_i == _length * 2 + 1)
            {
                type = 0;
            }
            if (type != -1)
                rotation = Quaternion.Euler(0f, 90.0f * type, 0f);
            else
                rotation = Quaternion.Euler(0f, 0f, 0f);
        }

    }

    public void MarkForbidden()
    {
        zoneTag = Zone.Forbidden;
    }

    public void DebugTile()
    {
        Debug.Log($"Tile at {position} is {zoneTag}");
    }

    public void DrawGizmo()
    {
        Color color = Color.green;
        if (zoneTag == Zone.Outside) color = Color.blue;
        if (zoneTag == Zone.Forbidden) color = Color.red;
        if (zoneTag == Zone.Hallway) color = Color.yellow;

        Gizmos.color = color;
        Gizmos.DrawCube(position + Vector3.up * 0.05f, new Vector3(1.8f, 0.1f, 1.8f));
    }

    public int getI()
    {
        return i;
    }

    public int getJ()
    {
        return j;
    }
    public int getType()
    {
        return type;
    }

    public void SetRotation(Quaternion newRotation)
    {
        rotation = newRotation;
    }


}
