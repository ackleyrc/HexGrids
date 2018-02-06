using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawModeRange : State
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
        // TODO: Only draw range when first click starts within the grid

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
            if (cubeUnderMouse != previousCube)
            {
                List<Cube> cubesToHighlight = new List<Cube>();
                // Get the hex locations within the distance from the first hex clicked and where the mouse is now
                cubesToHighlight = Cube.Range(firstCube, Cube.Distance(firstCube, cubeUnderMouse));

                // Relocate or create highlights under each hex to highlight
                for (int i = 0; i < cubesToHighlight.Count; i++)
                {
                    if (i >= hexManager.highlights.Count)
                    {
                        GameObject highlight = GameObject.Instantiate(hexManager.highlightPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        hexManager.highlights.Add(highlight);
                    }
                    // Debug.Log("cubesToHighlight[" + i + "]: " + cubesToHighlight[i]);
                    hexManager.highlights[i].transform.position = hexManager.grid.CubeToPixel(cubesToHighlight[i], 1f) + Vector3.forward;
                    hexManager.highlights[i].SetActive(hexManager.grid.GetHexes().Contains(cubesToHighlight[i]));
                }
                // Deactivate any additional highlights beyond the number needed to highlight the line
                for (int j = cubesToHighlight.Count; j < hexManager.highlights.Count; j++)
                {
                    hexManager.highlights[j].SetActive(false);
                }
            }
        }
        // Otherwise, if left mouse button has been released or is not held and a new hex has been moused over...
        else if (Input.GetMouseButtonUp(0) || cubeUnderMouse != previousCube)
        {
            // Set the first highlight location to the hex currently under the mouse
            hexManager.highlights[0].transform.position = hexManager.grid.CubeToPixel(cubeUnderMouse, 1f) + Vector3.forward;
            hexManager.highlights[0].SetActive(hexManager.grid.GetHexes().Contains(cubeUnderMouse));

            // Deactivate any additional highlights beyond the number needed to highlight the current hex
            for (int i = 1; i < hexManager.highlights.Count; i++)
            {
                hexManager.highlights[i].SetActive(false);
            }
        }

        // Make note of the hex currently under the mouse this frame for comparison next frame
        previousCube = cubeUnderMouse;
    }

    protected override void DoEnter() { }

    protected override void DoExit() { }
}
