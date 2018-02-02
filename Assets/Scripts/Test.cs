using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public GameObject hexPrefab;
    public int width;
    public int height;

    Dictionary<Cube, int> hashes = new Dictionary<Cube, int>();

	void Start () {

        HexGrid grid = new HexGrid();

        HashSet<Cube> hexes = grid.GenerateRectangularGrid(HexGrid.Alignment.Horizontal, width, height);

        foreach (Cube hex in hexes)
        {
            GameObject.Instantiate(hexPrefab, grid.CubeToPixel(hex, 1f), Quaternion.identity);
        }
        
    }

}
