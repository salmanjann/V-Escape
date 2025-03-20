using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ceiling 
{
    public Vector3 position;
    
    int i, j;
    public Ceiling(int _floorNumber, Vector3 _center, int _width, int _length, int _i, int _j)
    {
        i = _i;
        j = _j;
        int new_width = _width * 4 / 2;
        int new_length = _length * 4 / 2;
        Vector3 topLeftCorner = new Vector3(_center.x - 1 - new_width, 0, _center.z + new_length + 1);
        position = topLeftCorner + new Vector3(_j * 4,(_floorNumber +1) * 4 -0.2f, -(_i * 4)) + new Vector3(1f,0f,-1f);
    }
}
