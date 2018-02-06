using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawModeObstacle : State
{
    HexManager hexManager;

    Cube firstCube;
    Cube previousCube;

    List<GameObject> prospectiveObstacles = new List<GameObject>(); 

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
        if (Input.GetMouseButtonDown(0))
        {
            firstCube = cubeUnderMouse;
            hexManager.highlights[0].SetActive(false);
            prospectiveObstacles = new List<GameObject>();
        }
        // Otherwise, if left mouse button is being held down (dragging state)...
        else if (Input.GetMouseButton(0))
        {
            // ... and the mouse is under a new hex location...
            if (cubeUnderMouse == firstCube || cubeUnderMouse != previousCube)
            {
                List<Cube> obstacleLocations = new List<Cube>();
                // Get the hex locations that form a line between where the mouse was first clicked and where it is now
                obstacleLocations = Cube.LineDraw(firstCube, cubeUnderMouse);

                // Relocate or create prospective obstacles under each hex location
                for (int i = 0; i < obstacleLocations.Count; i++)
                {
                    if (i >= prospectiveObstacles.Count)
                    {
                        GameObject obstacle = GameObject.Instantiate(hexManager.obstaclePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        prospectiveObstacles.Add(obstacle);
                    }
                    prospectiveObstacles[i].transform.position = hexManager.grid.CubeToPixel(obstacleLocations[i], 1f) + Vector3.forward;
                    prospectiveObstacles[i].SetActive(hexManager.grid.GetHexes().Contains(obstacleLocations[i]));
                }
                // Deactivate any additional prospective obstalces beyond the number needed to highlight the line
                for (int j = obstacleLocations.Count; j < prospectiveObstacles.Count; j++)
                {
                    prospectiveObstacles[j].SetActive(false);
                }
            }
        }
        // Otherwise, if left mouse button has been released or is not held and a new hex has been moused over...
        else if (Input.GetMouseButtonUp(0))
        {
            // Lock in any active prospective obstacles and destroy the rest
            foreach (GameObject obstacle in prospectiveObstacles)
            {
                Vector3 position = obstacle.transform.position;
                Cube cubePosition = hexManager.grid.PixelToCube(position.x, position.y, 1f);
                if (obstacle.activeSelf)
                {
                    if (!hexManager.obstacles.ContainsKey(cubePosition))
                    {
                        hexManager.obstacles.Add(cubePosition, obstacle);
                    }
                }
                else
                {
                    Destroy(obstacle);
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
