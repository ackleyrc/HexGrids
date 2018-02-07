using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(DrawModeLine))]
[RequireComponent(typeof(DrawModeRange))]
[RequireComponent(typeof(DrawModeObstacle))]
[RequireComponent(typeof(DrawModePathFind))]
[RequireComponent(typeof(DrawModeReachable))]
public class DemoManager : MonoBehaviour
{
    [HideInInspector]
    public StateMachine stateMachine;

    private List<State> drawModes;

    public DrawModeLine drawModeLine;
    public DrawModeRange drawModeRange;
    public DrawModeObstacle drawModeObstacle;
    public DrawModePathFind drawModePathFind;
    public DrawModeReachable drawModeReachable;

    HexManager hexManager;

    public enum CoordDisplay
    {
        Cube,
        Axial,
        None
    }

    [HideInInspector]
    public CoordDisplay currentCoordDisplay = CoordDisplay.Cube;

    private void Awake()
    {
        stateMachine = GetComponent<StateMachine>();
        stateMachine.Init(drawModeLine);
        drawModes = new List<State>
        {
            drawModeLine,
            drawModeRange,
            drawModeObstacle,
            drawModePathFind,
            drawModeReachable
        };
        hexManager = GetComponent<HexManager>();
    }

    public void SetDrawMode(int drawModeIndex)
    {
        if (stateMachine.currentState != drawModes[drawModeIndex])
        {
            stateMachine.ChangeState(drawModes[drawModeIndex]);
        }
    }

    public void DisplayCube()
    {
        hexManager.DisplayCube();
        currentCoordDisplay = CoordDisplay.Cube;
    }

    public void DisplayAxial()
    {
        hexManager.DisplayAxial();
        currentCoordDisplay = CoordDisplay.Axial;
    }

    public void DisplayNone()
    {
        hexManager.DisplayNone();
        currentCoordDisplay = CoordDisplay.None;
    }
}
