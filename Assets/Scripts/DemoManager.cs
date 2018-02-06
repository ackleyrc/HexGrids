using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(DrawModeLine))]
[RequireComponent(typeof(DrawModeRange))]
public class DemoManager : MonoBehaviour
{
    public int width;
    public int height;

    public Text widthText;
    public Text heightText;

    public GameObject hexPrefab;
    public GameObject highlightPrefab;

    [HideInInspector]
    public StateMachine stateMachine;
    private List<State> drawModes;
    public DrawModeLine drawModeLine;
    public DrawModeRange drawModeRange;

    public List<GameObject> highlights = new List<GameObject>();

    public HexGrid grid;
    Dictionary<Cube, GameObject> hexes = new Dictionary<Cube, GameObject>();

    Cube firstCube;
    Cube previousCube = new Cube(0, 0 ,0);

    CoordDisplay currentCoordDisplay = CoordDisplay.Cube;

    private void Awake()
    {
        stateMachine = GetComponent<StateMachine>();
        stateMachine.Init(drawModeLine);
        drawModes = new List<State>
        {
            drawModeLine,
            drawModeRange
        };
    }

    void Start ()
    {
        SetGrid();

        GameObject highlight = GameObject.Instantiate(highlightPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        highlight.SetActive(false);
        highlights.Add(highlight);
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

    public void SetDrawMode(int drawModeIndex)
    {
        if (stateMachine.currentState != drawModes[drawModeIndex])
        {
            stateMachine.ChangeState(drawModes[drawModeIndex]);
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
