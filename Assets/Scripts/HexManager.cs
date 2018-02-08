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

    List<GameObject> tentativeObstacleAdditions = new List<GameObject>();
    List<GameObject> tentativeObstacleRemovals = new List<GameObject>();
    [HideInInspector]
    public Dictionary<Cube, GameObject> obstacles = new Dictionary<Cube, GameObject>();

    List<GameObject> warningTiles = new List<GameObject>();

    public HexGrid grid = new HexGrid();
    public Dictionary<Cube, GameObject> hexes = new Dictionary<Cube, GameObject>();

    DemoManager demoManager;

    private void Awake()
    {
        demoManager = GetComponent<DemoManager>();
    }

    void Start ()
    {
        SetWidth(width);
        SetHeight(height);
        SetGrid();
        GameObject highlight = GameObject.Instantiate(highlightPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        highlight.SetActive(false);
        highlights.Add(highlight);
    }

    public void DeactivateHighlights()
    {
        ReplaceHighlights(new List<Cube>());
    }

    public void ReplaceHighlights(Cube location)
    {
        ReplaceHighlights(new List<Cube> { location });
    }

    public void ReplaceHighlights(List<Cube> newLocations)
    {
        ReplaceHexTiles(newLocations, highlights, highlightPrefab);
    }

    public void PreviewObstacleAdditions(List<Cube> newLocations)
    {
        ReplaceHexTiles(newLocations, tentativeObstacleAdditions, obstaclePrefab);
    }

    public void ConfirmObstacleAdditions()
    {
        // Lock in any active prospective obstacles and destroy the rest
        foreach (GameObject obstacleAdd in tentativeObstacleAdditions)
        {
            Vector3 position = obstacleAdd.transform.position;
            Cube cubePosition = grid.PixelToCube(position.x, position.y, 1f);
            if (obstacleAdd.activeSelf && !obstacles.ContainsKey(cubePosition))
            {
                obstacles.Add(cubePosition, obstacleAdd);
            }
            else
            {
                Destroy(obstacleAdd);
            }
        }
        // Reset tentative list for next time
        tentativeObstacleAdditions = new List<GameObject>();
    }

    public void PreviewObstacleRemovals(List<Cube> newLocations)
    {
        ReplaceHexTiles(newLocations, tentativeObstacleRemovals, warningPrefab);
    }

    public void ConfirmObstacleRemovals()
    {
        // Destroy any obstacles under the hex tiles marking removal
        foreach (GameObject obstacleRemove in tentativeObstacleRemovals)
        {
            Vector3 position = obstacleRemove.transform.position;
            Cube cubePosition = grid.PixelToCube(position.x, position.y, 1f);
            if (obstacleRemove.activeSelf)
            {
                if (obstacles.ContainsKey(cubePosition))
                {
                    GameObject go = obstacles[cubePosition];
                    obstacles.Remove(cubePosition);
                    Destroy(go);
                }
            }
            Destroy(obstacleRemove);
        }
        // Reset tentative list for next time
        tentativeObstacleRemovals = new List<GameObject>();
    }

    public void ReplaceWarningTiles(List<Cube> newLocations)
    {
        ReplaceHexTiles(newLocations, warningTiles, warningPrefab);
    }

    private void ReplaceHexTiles(List<Cube> newLocations, List<GameObject> hexTileContainer, GameObject hexTilePrefab)
    {
        if (newLocations != null)
        {
            // Relocate or create hex tiles under each provided hex location
            for (int i = 0; i < newLocations.Count; i++)
            {
                if (i >= hexTileContainer.Count)
                {
                    GameObject hexTile = GameObject.Instantiate(hexTilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    hexTileContainer.Add(hexTile);
                }
                hexTileContainer[i].transform.position = grid.CubeToPixel(newLocations[i], 1f) + Vector3.forward;
                hexTileContainer[i].SetActive(grid.GetHexes().Contains(newLocations[i]));
            }
            // Deactivate any remaining existing hex tiles beyond the number of those currently needed
            for (int j = newLocations.Count; j < hexTileContainer.Count; j++)
            {
                hexTileContainer[j].SetActive(false);
            }
        }
    }

    private void RemoveOffGridObstacles()
    {
        Dictionary<Cube, GameObject> obstaclesToRemove = new Dictionary<Cube, GameObject>();
        foreach (KeyValuePair<Cube, GameObject> pair in obstacles)
        {
            if (!grid.GetHexes().Contains(pair.Key))
            {
                obstaclesToRemove.Add(pair.Key, pair.Value);
            }
        }
        foreach (KeyValuePair<Cube, GameObject> pair in obstaclesToRemove)
        {
            obstacles.Remove(pair.Key);
            Destroy(pair.Value);
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

    private void SetWidth(int val)
    {
        width = (int)Mathf.Clamp(val, 1, Mathf.Infinity);
        widthText.text = "Width: " + width;
    }

    private void SetHeight(int val)
    {
        height = (int)Mathf.Clamp(val, 1, Mathf.Infinity);
        heightText.text = "Height: " + height;
    }

    public void UpdateWidth(int val)
    {
        SetWidth(width + val);
        SetGrid();
        RemoveOffGridObstacles();
    }

    public void UpdateHeight(int val)
    {
        SetHeight(height + val);
        SetGrid();
        RemoveOffGridObstacles();
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
