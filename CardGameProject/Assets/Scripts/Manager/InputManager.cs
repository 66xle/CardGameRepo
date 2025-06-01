using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public bool LeftClickInputDown { get; private set; } = false;

    public bool HoldLeftClickInput { get; private set; } = false;

    public bool LeftClickInputUp { get; private set; } = false;

    // Update is called once per frame
    void Update()
    {
        LeftClickInputDown = Input.GetKeyDown(KeyCode.Mouse0) ? true : false;

        HoldLeftClickInput = Input.GetKey(KeyCode.Mouse0) ? true : false;

        LeftClickInputUp = Input.GetKeyUp(KeyCode.Mouse0) ? true : false;
    }
}
