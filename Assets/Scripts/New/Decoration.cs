using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoration
{
    public Vector3 position;
    public Quaternion rotation;
    public DecorationAsset asset;

    public Decoration(Vector3 position, Quaternion rotation, DecorationAsset asset,int _type, Vector3 _offset)
    {
        this.position = position;
        this.rotation = rotation;
        this.asset = asset;

        if(_offset.x >= 1 || _offset.z >= 1){
            if(_type ==0)
                this.position += new Vector3(0f,0f,1f);
            else if(_type ==1)
                this.position += new Vector3(1f,0f,0f);
            else if(_type ==2)
                this.position += new Vector3(0f,0f,-1f);
            else if(_type ==3)
                this.position += new Vector3(-1f,0f,0f);
        }
    }
}

