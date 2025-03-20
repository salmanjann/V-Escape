using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corner 
{
    public Vector3 position;
    public Quaternion rotation;
    public int cornerPrefabNumber;

    public Corner(int _floorNumber, Vector3 _center,int _width, int _length, int _type) {
        // considering width and length represents the number of walls
        _width = _width *4 /2;
        _length = _length *4 /2;

        rotation = Quaternion.Euler(0f,_type * 90f, 0f);

        // Setting position based on wall direction
        if(_type == 0){
            position = _center + new Vector3(-_width -2 ,0, -_length-2);
        }
        else if(_type == 1){
            position = _center + new Vector3(-_width -2 ,0, _length+2);
        }
        else if(_type == 2){
            position = _center + new Vector3(_width +2 , 0, _length+2 );
        }
        else {
            position = _center + new Vector3(_width +2 , 0, -_length -2);
        }

        cornerPrefabNumber = Random.Range(0, 2);

    }

}
