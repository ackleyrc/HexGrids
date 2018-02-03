using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenToHexTest : MonoBehaviour {

    public GameObject hexPrefab;
    public GameObject highlightPrefab;

    GameObject highlight;

    public int width;
    public int height;

    HexGrid grid;
    Dictionary<Cube, GameObject> hexes = new Dictionary<Cube, GameObject>();

    Cube previousCube = new Cube(0, 0 ,0);

	void Start () {

        grid = new HexGrid();

        HashSet<Cube> cubeCoords = grid.GenerateRectangularGrid(HexGrid.Alignment.Horizontal, width, height);

        foreach (Cube hex in cubeCoords)
        {
            GameObject go = GameObject.Instantiate(hexPrefab, grid.CubeToPixel(hex, 1f), Quaternion.identity);
            hexes.Add(hex, go);
        }

        highlight = GameObject.Instantiate(highlightPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        highlight.SetActive(false);
    }

    private void Update()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Cube nearestCube = grid.PixelToCube(worldPoint.x, worldPoint.y, 1f);

        if (nearestCube != previousCube)
        {
            highlight.transform.position = grid.CubeToPixel(nearestCube, 1f) + Vector3.forward;
            previousCube = nearestCube;
            highlight.SetActive(grid.GetHexes().Contains(nearestCube));
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Cube: " + nearestCube);
        }
    }

}
