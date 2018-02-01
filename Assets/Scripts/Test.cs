using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    
    Dictionary<Cube, int> hashes = new Dictionary<Cube, int>();

	void Start () {
        
        for (int y = -1; y < 1; y++)
        {
            for (int x = -3; x < 2; x++)
            {
                Cube testCube = new Cube(x, y);
                Debug.Log(testCube + " Hash: " + testCube.GetHashCode());
                hashes.Add(testCube, testCube.GetHashCode());
            }
        }
        
        Debug.Log("Total Keys: " + hashes.Keys.Count);
        Cube anotherCube = new Cube(-1, 0);
        hashes.Add(anotherCube, -1);
        Debug.Log("Total Keys: " + hashes.Keys.Count);
    }

}
