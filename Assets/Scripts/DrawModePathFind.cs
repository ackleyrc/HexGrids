using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawModePathFind : State
{
    HexManager hexManager;

    Cube firstCube;
    Cube previousCube;

    protected override void DoAwake()
    {
        hexManager = GetComponent<HexManager>();
    }

    protected override void DoStart() { }

    public override void DoUpdate()
    {
        // TODO: Only draw line when first click starts within the grid

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
            // ... and the mouse is under a new hex location...
            if (hexManager.grid.GetHexes().Contains(firstCube) && cubeUnderMouse != previousCube)
            {
                // Get the hex locations that form a line between where the mouse was first clicked and where it is now
                List<Cube> cubesToHighlight = hexManager.grid.PathFindBFS(firstCube,
                                                                          cubeUnderMouse, 
                                                                          (x) => hexManager.grid.GetHexes().Contains(x) 
                                                                          && !hexManager.obstacles.ContainsKey(x));
                Debug.Log("cubesToHighlight.Count: " + cubesToHighlight.Count);

                // TODO: If path is not available, show warningPrefab hex under start and finish

                // Replace the existing highlights to cover the determined line
                hexManager.ReplaceHighlights(cubesToHighlight);
            }
        }
        // Otherwise, if left mouse button has been released or is not held and a new hex has been moused over...
        else if (Input.GetMouseButtonUp(0) || cubeUnderMouse != previousCube)
        {
            // Highlight just the hex currently under the mouse
            hexManager.ReplaceHighlights(cubeUnderMouse);
        }

        // Make note of the hex currently under the mouse this frame for comparison next frame
        previousCube = cubeUnderMouse;
    }

    protected override void DoEnter() { }

    protected override void DoExit() { }
}
