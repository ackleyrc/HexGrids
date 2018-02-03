using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenToHexTest : MonoBehaviour {

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

    bool displayingCube = true;

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

        // float xOffset = width % 2 == 0 ? -0.35f : 0.35f;
        // float yOffset = height % 2 == 0 ? -0.35f : 0.35f;
        int rCenter = Mathf.FloorToInt((height) / 2f);
        int qCenter = Mathf.FloorToInt((width) / 2f) - Mathf.FloorToInt(rCenter / 2f);
        Vector3 worldPoint = grid.CubeToPixel(new Cube(qCenter, rCenter), 1f);
        Camera.main.transform.position = new Vector3(worldPoint.x, worldPoint.y, -10f);
        Camera.main.orthographicSize = Mathf.Max(width, height);

        widthText.text = "Width: " + width;
        heightText.text = "Height: " + height;

        foreach (Cube hex in grid.GetHexes())
        {
            GameObject go = GameObject.Instantiate(hexPrefab, grid.CubeToPixel(hex, 1f), Quaternion.identity);

            HexCoordDisplay hexCoordDisplay = go.GetComponent<HexCoordDisplay>();
            hexCoordDisplay.SetCube(hex);
            if (displayingCube)
            {
                hexCoordDisplay.DisplayCube();
            }
            else
            {
                hexCoordDisplay.DisplayAxial();
            }

            hexes.Add(hex, go);
        }
    }

    public void DisplayCube()
    {
        if (!displayingCube)
        {
            foreach (GameObject go in hexes.Values)
            {
                go.GetComponent<HexCoordDisplay>().DisplayCube();
            }
            displayingCube = true;
        }
    }

    public void DisplayAxial()
    {
        if (displayingCube)
        {
            foreach (GameObject go in hexes.Values)
            {
                go.GetComponent<HexCoordDisplay>().DisplayAxial();
            }
            displayingCube = false;
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
