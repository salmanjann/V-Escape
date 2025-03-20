using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleRoomPos
{
    public Ground center;
    public int width,length, posNumber,centerRow,centerCol;

    public PossibleRoomPos(Ground _center, int _posNumber, int _width, int _length,int _cetnerRow, int _centerCol){
        this.center = _center;
        this.posNumber = _posNumber;
        this.width = _width;
        this.length = _length;
        this.centerRow = _cetnerRow;
        this.centerCol = _centerCol;
    }

}
