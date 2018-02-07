using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexManager : MonoBehaviour {

    public int width;
    public int height;

    public Text widthText;
    public Text heightText;

    public GameObject hexPrefab;
    public GameObject highlightPrefab;
    public GameObject obstaclePrefab;
    public GameObject warningPrefab;

    [HideInInspector]
    public List<GameObject> highlights = new List<GameObject>();

    [HideInInspector]
    public Dictionary<Cube, GameObject> obstacles = new Dictionary<Cube, GameObject>();

    public HexGrid grid = new HexGrid();
    public Dictionary<Cube, GameObject> hexes = new Dictionary<Cube, GameObject>();

    DemoManager demoManager;

    private void Awake()
    {
        demoManager = GetComponent<DemoManager>();
    }

    void Start ()
    {
        SetGrid();
        GameObject highlight = GameObject.Instantiate(highlightPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        highlight.SetActive(false);
        highlights.Add(highlight);
    }

    public void ReplaceHighlights(List<Cube> newLocations)
    {
        if (newLocations != null)
        {
            // Relocate or create highlights under each hex to highlight
            for (int i = 0; i < newLocations.Count; i++)
            {
                if (i >= highlights.Count)
                {
                    GameObject highlight = GameObject.Instantiate(highlightPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    highlights.Add(highlight);
                }
                highlights[i].transform.position = grid.CubeToPixel(newLocations[i], 1f) + Vector3.forward;
                highlights[i].SetActive(grid.GetHexes().Contains(newLocations[i]));
            }
            // Deactivate any additional highlights beyond the number those needed
            for (int j = newLocations.Count; j < highlights.Count; j++)
            {
                highlights[j].SetActive(false);
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

        foreach (Cube hex in grid.GetHexes())
        {
            GameObject go = GameObject.Instantiate(hexPrefab, grid.CubeToPixel(hex, 1f), Quaternion.identity);

            HexCoordDisplay hexCoordDisplay = go.GetComponent<HexCoordDisplay>();
            hexCoordDisplay.SetCube(hex);
            if (demoManager.currentCoordDisplay == DemoManager.CoordDisplay.Cube)
            {
                hexCoordDisplay.DisplayCube();
            }
            else if (demoManager.currentCoordDisplay == DemoManager.CoordDisplay.Axial)
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
        Camera.main.transform.position = new Vector3(center.x, center.y, -10f) + 0.5f * Vector3.up;
        Camera.main.orthographicSize = Mathf.Max(width, height);
    }

    // TODO: Remove or deactivate obstacles outside grid after resizing

    public void UpdateWidth(int val)
    {
        width = (int)Mathf.Clamp(width + val, 1, Mathf.Infinity);
        widthText.text = "Width: " + width;
        SetGrid();
    }

    public void UpdateHeight(int val)
    {
        height = (int)Mathf.Clamp(height + val, 1, Mathf.Infinity);
        heightText.text = "Height: " + height;
        SetGrid();
    }

    public void DisplayCube()
    {
        foreach (GameObject go in hexes.Values)
        {
            go.GetComponent<HexCoordDisplay>().DisplayCube();
        }
    }

    public void DisplayAxial()
    {
        foreach (GameObject go in hexes.Values)
        {
            go.GetComponent<HexCoordDisplay>().DisplayAxial();
        }
    }

    public void DisplayNone()
    {
        foreach (GameObject go in hexes.Values)
        {
            go.GetComponent<HexCoordDisplay>().DisplayNone();
        }
    }
}
