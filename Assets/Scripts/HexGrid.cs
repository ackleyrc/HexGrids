using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid {

}

public class Cube
{
    Vector3Int vec3;

    public Cube(int q, int r, int s)
    {
        this.vec3 = new Vector3Int(q, s, r);
    }

    public int q { get { return vec3.x; } }

    public int r { get { return vec3.z; } }

    public int s { get { return vec3.y; } }

    // TODO: Implement functions for IEquatable 
    //      See https://answers.unity.com/questions/950867/multidimensional-array-vs-dictionary-performance.html

    // operator +

    // bool Equals(other)

    // operator ==

    // operator !=

    // GetHashCode

    // ToString()
}
