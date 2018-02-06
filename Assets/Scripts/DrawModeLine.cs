using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawModeLine : State
{
    DemoManager demoManager;

    Cube firstCube;
    Cube previousCube;

    protected override void DoAwake()
    {
        demoManager = GetComponent<DemoManager>();
    }

    protected override void DoStart() { }

    public override void DoUpdate()
    {
        // Determine which hex the mouse is currently hovering over
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Cube cubeUnderMouse = demoManager.grid.PixelToCube(worldPoint.x, worldPoint.y, 1f);

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
                // Get the hex locations that form a line between where the mouse was first clicked and where it is now
                cubesToHighlight = Cube.LineDraw(firstCube, cubeUnderMouse);

                // Relocate or create highlights under each hex to highlight
                for (int i = 0; i < cubesToHighlight.Count; i++)
                {
                    if (i >= demoManager.highlights.Count)
                    {
                        GameObject highlight = GameObject.Instantiate(demoManager.highlightPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        demoManager.highlights.Add(highlight);
                    }
                    // Debug.Log("cubesToHighlight[" + i + "]: " + cubesToHighlight[i]);
                    demoManager.highlights[i].transform.position = demoManager.grid.CubeToPixel(cubesToHighlight[i], 1f) + Vector3.forward;
                    demoManager.highlights[i].SetActive(demoManager.grid.GetHexes().Contains(cubesToHighlight[i]));
                }
                // Deactivate any additional highlights beyond the number needed to highlight the line
                for (int j = cubesToHighlight.Count; j < demoManager.highlights.Count; j++)
                {
                    demoManager.highlights[j].SetActive(false);
                }
            }
        }
        // Otherwise, if left mouse button has been released or is not held and a new hex has been moused over...
        else if (Input.GetMouseButtonUp(0) || cubeUnderMouse != previousCube)
        {
            // Set the first highlight location to the hex currently under the mouse
            demoManager.highlights[0].transform.position = demoManager.grid.CubeToPixel(cubeUnderMouse, 1f) + Vector3.forward;
            demoManager.highlights[0].SetActive(demoManager.grid.GetHexes().Contains(cubeUnderMouse));

            // Deactivate any additional highlights beyond the number needed to highlight the current hex
            for (int i = 1; i < demoManager.highlights.Count; i++)
            {
                demoManager.highlights[i].SetActive(false);
            }
        }

        // Make note of the hex currently under the mouse this frame for comparison next frame
        previousCube = cubeUnderMouse;
    }

    protected override void DoEnter() { }

    protected override void DoExit() { }
}
