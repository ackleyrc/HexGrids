using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour
{
    public int width;
    public int height;

    public Text widthText;
    public Text heightText;

    public GameObject hexPrefab;
    public GameObject highlightPrefab;

    List<GameObject> highlights = new List<GameObject>();

    HexGrid grid;
    Dictionary<Cube, GameObject> hexes = new Dictionary<Cube, GameObject>();

    Cube firstCube;
    Cube previousCube = new Cube(0, 0 ,0);

    CoordDisplay currentCoordDisplay = CoordDisplay.Cube;
    DrawMode currentDrawMode = DrawMode.Line;

    public enum DrawMode
    {
        Line,
        Range
    }

	void Start ()
    {
        SetGrid();

        GameObject highlight = GameObject.Instantiate(highlightPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        highlight.SetActive(false);
        highlights.Add(highlight);
    }

    private void Update()
    {
        // Determine which hex the mouse is currently hovering over
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Cube cubeUnderMouse = grid.PixelToCube(worldPoint.x, worldPoint.y, 1f);

        // When left mouse button first clicked, store off hex location under mouse
        if (Input.GetMouseButtonDown(0))
        {
            firstCube = cubeUnderMouse;
        }
        // Otherwise, if left mouse button is being held down (dragging state)...
        else if (Input.GetMouseButton(0))
        {
            // ... and the mouse is under a new hex location...
            if (cubeUnderMouse != previousCube)
            {
                List<Cube> cubesToHighlight = new List<Cube>();
                if (currentDrawMode == DrawMode.Line)
                {
                    // Get the hex locations that form a line between where the mouse was first clicked and where it is now
                    cubesToHighlight = Cube.LineDraw(firstCube, cubeUnderMouse);
                }
                else if (currentDrawMode == DrawMode.Range)
                {
                    // Get the hex locations within the distance from the first hex clicked and where the mouse is now
                    cubesToHighlight = Cube.Range(firstCube, Cube.Distance(firstCube, cubeUnderMouse));
                }

                // Relocate or create highlights under each hex to highlight
                for (int i = 0; i < cubesToHighlight.Count; i++)
                {
                    if (i >= highlights.Count)
                    {
                        GameObject highlight = GameObject.Instantiate(highlightPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        highlights.Add(highlight);
                    }
                    // Debug.Log("cubesToHighlight[" + i + "]: " + cubesToHighlight[i]);
                    highlights[i].transform.position = grid.CubeToPixel(cubesToHighlight[i], 1f) + Vector3.forward;
                    highlights[i].SetActive(grid.GetHexes().Contains(cubesToHighlight[i]));
                }
                // Deactivate any additional highlights beyond the number needed to highlight the line
                for (int j = cubesToHighlight.Count; j < highlights.Count; j++)
                {
                    highlights[j].SetActive(false);
                }
            }
        }
        // Otherwise, if left mouse button has been released or is not held and a new hex has been moused over...
        else if (Input.GetMouseButtonUp(0) || cubeUnderMouse != previousCube)
        {
            // Set the first highlight location to the hex currently under the mouse
            highlights[0].transform.position = grid.CubeToPixel(cubeUnderMouse, 1f) + Vector3.forward;
            highlights[0].SetActive(grid.GetHexes().Contains(cubeUnderMouse));

            // Deactivate any additional highlights beyond the number needed to highlight the current hex
            for (int i = 1; i < highlights.Count; i++)
            {
                highlights[i].SetActive(false);
            }
        }

        // Make note of the hex currently under the mouse this frame for comparison next frame
        previousCube = cubeUnderMouse;
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

    public void SetDrawModeLine()
    {
        if (currentDrawMode != DrawMode.Line)
        {
            currentDrawMode = DrawMode.Line;
        }
    }

    public void SetDrawModeRange()
    {
        if (currentDrawMode != DrawMode.Range)
        {
            currentDrawMode = DrawMode.Range;
        }
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
