using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour {

    public GameObject hexPrefab;
    public GameObject highlightPrefab;

    GameObject highlight;

    public int width;
    public int height;

    public Text widthText;
    public Text heightText;

    HexGrid grid;
    Dictionary<Cube, GameObject> hexes = new Dictionary<Cube, GameObject>();

    Cube previousCube = new Cube(0, 0 ,0);

    CoordDisplay currentCoordDisplay = CoordDisplay.Cube;

	void Start ()
    {
        SetGrid();

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
            if (hexes.ContainsKey(nearestCube))
            {
                Debug.Log("Cube: " + nearestCube);
            }
        }
    }

    public void SetGrid()
    {
        foreach (GameObject go in hexes.Values)
        {
            Destroy(go);
        }

        hexes = new Dictionary<Cube, GameObject>();

        grid = new HexGrid();
        grid.GenerateRectangularGrid(HexGrid.Alignment.Horizontal, width, height);

        widthText.text = "Width: " + width;
        heightText.text = "Height: " + height;

        foreach (Cube hex in grid.GetHexes())
        {
            GameObject go = GameObject.Instantiate(hexPrefab, grid.CubeToPixel(hex, 1f), Quaternion.identity);

            HexCoordDisplay hexCoordDisplay = go.GetComponent<HexCoordDisplay>();
            hexCoordDisplay.SetCube(hex);
            if (currentCoordDisplay == CoordDisplay.Cube)
            {
                hexCoordDisplay.DisplayCube();
            }
            else if (currentCoordDisplay == CoordDisplay.Axial)
            {
                hexCoordDisplay.DisplayAxial();
            }
            else
            {
                hexCoordDisplay.DisplayNone();
            }

            hexes.Add(hex, go);
        }

        ResetCamera();
    }

    public void ResetCamera()
    {
        int rTopRight = height - 1;
        int qTopRight = (width - 1) - Mathf.CeilToInt(rTopRight / 2f);

        Cube bottomLeft = new Cube(0, 0);
        Cube topRight = new Cube(qTopRight, rTopRight);

        Vector3 center = Vector3.Lerp(grid.CubeToPixel(bottomLeft, 1f), grid.CubeToPixel(topRight, 1f), 0.5f);
        Camera.main.transform.position = new Vector3(center.x, center.y, -10f);
        Camera.main.orthographicSize = Mathf.Max(width, height);
    }

    public enum CoordDisplay
    {
        Cube,
        Axial,
        None
    }

    public void DisplayCube()
    {
        if (currentCoordDisplay != CoordDisplay.Cube)
        {
            foreach (GameObject go in hexes.Values)
            {
                go.GetComponent<HexCoordDisplay>().DisplayCube();
            }
            currentCoordDisplay = CoordDisplay.Cube;
        }
    }

    public void DisplayAxial()
    {
        if (currentCoordDisplay != CoordDisplay.Axial)
        {
            foreach (GameObject go in hexes.Values)
            {
                go.GetComponent<HexCoordDisplay>().DisplayAxial();
            }
            currentCoordDisplay = CoordDisplay.Axial;
        }
    }

    public void DisplayNone()
    {
        if (currentCoordDisplay != CoordDisplay.None)
        {
            foreach (GameObject go in hexes.Values)
            {
                go.GetComponent<HexCoordDisplay>().DisplayNone();
            }
            currentCoordDisplay = CoordDisplay.None;
        }
    }

    public void UpdateWidth(int val)
    {
        width = (int) Mathf.Clamp(width + val, 1, Mathf.Infinity);
        SetGrid();
    }

    public void UpdateHeight(int val)
    {
        height = (int) Mathf.Clamp(height + val, 1, Mathf.Infinity);
        SetGrid();
    }
}
