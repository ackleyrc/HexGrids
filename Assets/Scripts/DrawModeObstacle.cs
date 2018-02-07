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
        // TODO: Remove obstacles when right-clicking (or toggle existing obstacles under the current line)
        // TODO: Only draw obstacles when first click starts within the grid

        // Determine which hex the mouse is currently hovering over
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Cube cubeUnderMouse = hexManager.grid.PixelToCube(worldPoint.x, worldPoint.y, 1f);

        // When left mouse button first clicked, store off hex location under mouse
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            firstCube = cubeUnderMouse;
            hexManager.highlights[0].SetActive(false);
            tentativeLocations = new List<GameObject>();
        }
        // Otherwise, if left mouse button is being held down (dragging state)...
        else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        { 
            GameObject hexPrefab = Input.GetMouseButton(0) ? hexManager.obstaclePrefab : hexManager.warningPrefab;
            float zOffset = Input.GetMouseButton(0) ? 0f : 0.25f;

            // ... and the mouse is under a new hex location...
            if (cubeUnderMouse == firstCube || cubeUnderMouse != previousCube)
            {
                List<Cube> newLocations = new List<Cube>();
                // Get the hex locations that form a line between where the mouse was first clicked and where it is now
                newLocations = Cube.Line(firstCube, cubeUnderMouse);

                // Relocate or create prospective obstacles under each hex location
                for (int i = 0; i < newLocations.Count; i++)
                {
                    if (i >= tentativeLocations.Count)
                    {
                        GameObject hex = GameObject.Instantiate(hexPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        tentativeLocations.Add(hex);
                    }
                    tentativeLocations[i].transform.position = hexManager.grid.CubeToPixel(newLocations[i], 1f) + Vector3.forward - Vector3.forward * zOffset;
                    tentativeLocations[i].SetActive(hexManager.grid.GetHexes().Contains(newLocations[i]));
                }
                // Deactivate any additional prospective obstalces beyond the number needed to highlight the line
                for (int j = newLocations.Count; j < tentativeLocations.Count; j++)
                {
                    tentativeLocations[j].SetActive(false);
                }
            }
        }
        // Otherwise, if left mouse button has been released or is not held and a new hex has been moused over...
        else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            if (Input.GetMouseButtonUp(0))
            {
                // Lock in any active prospective obstacles and destroy the rest
                foreach (GameObject obstacle in tentativeLocations)
                {
                    Vector3 position = obstacle.transform.position;
                    Cube cubePosition = hexManager.grid.PixelToCube(position.x, position.y, 1f);
                    if (obstacle.activeSelf && !hexManager.obstacles.ContainsKey(cubePosition))
                    {
                        hexManager.obstacles.Add(cubePosition, obstacle);
                    }
                    else
                    {
                        Destroy(obstacle);
                    }
                }
            }
            else
            {
                // Destroy any obstacles under the red warning hexes
                foreach (GameObject hex in tentativeLocations)
                {
                    Vector3 position = hex.transform.position;
                    Cube cubePosition = hexManager.grid.PixelToCube(position.x, position.y, 1f);
                    if (hex.activeSelf)
                    {
                        if (hexManager.obstacles.ContainsKey(cubePosition))
                        {
                            GameObject go = hexManager.obstacles[cubePosition];
                            hexManager.obstacles.Remove(cubePosition);
                            Destroy(go);
                        }
                    }
                    Destroy(hex);
                }
            }
        }
        // Otherwise, if mouse is only hovering, highlight hex under mouse (where there is not an obstacle underneath)
        else if (cubeUnderMouse != previousCube)
        {
            hexManager.highlights[0].transform.position = hexManager.grid.CubeToPixel(cubeUnderMouse, 1f) + Vector3.forward;
            if (hexManager.grid.GetHexes().Contains(cubeUnderMouse) && !hexManager.obstacles.ContainsKey(cubeUnderMouse)) {
                hexManager.highlights[0].SetActive(true);
            }
            else
            {
                hexManager.highlights[0].SetActive(false);
            }
        }

        // Make note of the hex currently under the mouse this frame for comparison next frame
        previousCube = cubeUnderMouse;
    }

    protected override void DoEnter() { }

    protected override void DoExit() { }
}
