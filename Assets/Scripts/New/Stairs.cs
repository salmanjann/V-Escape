using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Stairs 
{
    public Vector3 position;
    public Quaternion rotation;

    public Stairs(int _floorNumber, int _type,int _width, int _length){
        rotation = _type == 0? Quaternion.Euler(0f,90f,0f) : Quaternion.Euler(0f,270f,0f);

        _width = _width *2;
        _length = _length * 2;

        int xSign = _type == 0 ? -1 : 1;
        int zSign = -1;

        position = new Vector3(_width * xSign, _floorNumber * 4, _length * zSign);
    }
}
