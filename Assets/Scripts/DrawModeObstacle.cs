using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawModeObstacle : State
{
    HexManager hexManager;

    Cube firstCube;
    Cube previousCube;

    List<GameObject> tentativeLocations = new List<GameObject>(); 

    protected override void DoAwake()
    {
        hexManager = GetComponent<HexManager>();
    }

    protected override void DoStart() { }

    public override void DoUpdate()
    {
        // TODO: Only draw obstacles when first click starts within the grid

        // Determine which hex the mouse is currently hovering over
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Cube cubeUnderMouse = hexManager.grid.PixelToCube(worldPoint.x, worldPoint.y, 1f);

        // When left mouse button first clicked, store off hex location under mouse
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            firstCube = cubeUnderMouse;
            hexManager.DeactivateHighlights();
        }
        // Otherwise, if left mouse button is being held down (dragging state)...
        else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        { 
            // ... and the mouse is under a new hex location...
            if (cubeUnderMouse == firstCube || cubeUnderMouse != previousCube)
            {
                // Get the hex locations that form a line between where the mouse was first clicked and where it is now
                List<Cube> newLocations = Cube.Line(firstCube, cubeUnderMouse);

                // Replace the existing hex tiles to cover the determined line
                if (Input.GetMouseButton(0))
                {
                    hexManager.PreviewObstacleAdditions(newLocations);
                }
                else if (Input.GetMouseButton(1))
                {
                    hexManager.PreviewObstacleRemovals(newLocations);
                }
            }
        }
        // Otherwise, if left mouse button has been released or is not held and a new hex has been moused over...
        else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            if (Input.GetMouseButtonUp(0))
            {
                // Lock in any active prospective obstacles
                hexManager.ConfirmObstacleAdditions();
            }
            else
            {
                // Destroy any obstacles under the red warning hexes
                hexManager.ConfirmObstacleRemovals();
            }
        }
        // Otherwise, if mouse is only hovering, highlight hex under mouse (where there is not an obstacle underneath)
        else if (cubeUnderMouse != previousCube)
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
