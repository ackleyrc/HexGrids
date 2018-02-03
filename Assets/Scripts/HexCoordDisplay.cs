using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCoordDisplay : MonoBehaviour {

    Cube cube;
    public TextMesh text;

    public void SetCube(Cube cube)
    {
        this.cube = cube;
    }

    public void DisplayCube()
    {
        text.text = cube.q + ", " + cube.r + ", " + cube.s;
        text.fontSize = 220;
    }

    public void DisplayAxial()
    {
        text.text = cube.q + ", " + cube.r;
        text.fontSize = 260;
    }

    public void DisplayNone()
    {
        text.text = "";
    }
}
