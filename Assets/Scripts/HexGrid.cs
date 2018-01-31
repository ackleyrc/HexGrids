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
        if (q + r + s != 0)
        {
            Debug.LogError("The provided coordinates (" + q + ", " + r + ", " + s") do not form a canonical coordinate set. Coordinate values must satisfy the criteria q + r + s == 0.");
        }
        
        this.vec3 = new Vector3Int(q, s, r);
    }

    public int q { get { return vec3.x; } }

    public int r { get { return vec3.z; } }

    public int s { get { return vec3.y; } }

    public static Cube operator +(Cube a, Cube b)
    {
        return new Cube(a.q + b.q, a.r + b.r, a.s + b.s);
    }
    
    public static Cube operator -(Cube a, Cube b)
    {
        return new Cube(a.q - b.q, a.r - b.r, a.s - b.s);
    }
    
    public bool Equals(Cube other)
    {
        return other.q == this.q && other.r == this.r && other.s == this.s;
    }
    
    public static Cube operator ==(Cube a, Cube b)
    {
        return a.Equals(b);
    }

    public static Cube operator !=(Cube a, Cube b)
    {
        return !a.Equals(b);
    }
    
    public override string ToString()
    {
        return string.Format ("Cube(" + q + ", " + r + ", " + s + ")");
    }
    
    public override int GetHashCode()
    {
        return new Vector2Int(q, r).GetHashCode();   
    }
}
