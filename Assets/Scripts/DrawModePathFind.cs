using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawModePathFind : State
{
    HexManager hexManager;

    Cube firstCube;
    Cube previousCube;

    public delegate bool CubeFilterCriteria(Cube cube);
    CubeFilterCriteria criteria;

    protected override void DoAwake()
    {
        hexManager = GetComponent<HexManager>();
        criteria = (x) => hexManager.grid.GetHexes().Contains(x) && !hexManager.obstacles.ContainsKey(x);
    }

    protected override void DoStart() { }

    public override void DoUpdate()
    {
        // Determine which hex the mouse is currently hovering over
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Cube cubeUnderMouse = hexManager.grid.PixelToCube(worldPoint.x, worldPoint.y, 1f);

        // When left mouse button first clicked, store off hex location under mouse
        if (Input.GetMouseButtonDown(0))
        {
            firstCube = cubeUnderMouse;
        }
        // Otherwise, if left mouse button is being held down (dragging state)...
        else if (Input.GetMouseButton(0))
        {
            // ... and the mouse is under a new hex location over the grid...
            if (cubeUnderMouse != previousCube && hexManager.grid.GetHexes().Contains(firstCube))
            {
                List<Cube> path = new List<Cube>();
                List<Cube> firstAndLast = new List<Cube> { firstCube, cubeUnderMouse };

                if (criteria(firstCube))
                {
                    // Get the hex locations that form a line between where the mouse was first clicked and where it is now
                    path = hexManager.grid.GetShortestPath(firstCube, cubeUnderMouse, (x) => criteria(x));
                }

                if (path.Count > 0)
                {
                    // Replace the existing highlights to cover the determined line, and hide any existing warning tiles
                    hexManager.ReplaceHighlights(path);
                    hexManager.ReplaceWarningTiles(new List<Cube>());
                }
                else
                {
                    // If path is not available, show warningPrefab hex under start and finish and hide any existing highlights
                    hexManager.ReplaceWarningTiles(firstAndLast.Where((x) => hexManager.grid.GetHexes().Contains(x)).ToList());
                    hexManager.ReplaceHighlights(new List<Cube>());
                }
            }
        }
        // Otherwise, if left mouse button has been released or is not held and a new hex has been moused over...
        else if (Input.GetMouseButtonUp(0) || cubeUnderMouse != previousCube)
        {
            // Highlight just the hex currently under the mouse
            hexManager.ReplaceHighlights(cubeUnderMouse);
            hexManager.ReplaceWarningTiles(new List<Cube>());
        }

        // Make note of the hex currently under the mouse this frame for comparison next frame
        previousCube = cubeUnderMouse;
    }

    protected override void DoEnter() { }

    protected override void DoExit() { }
}
