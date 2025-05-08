using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool LeftClickInputDown { get; private set; } = false;

    // Update is called once per frame
    void Update()
    {
        LeftClickInputDown = Input.GetKeyDown(KeyCode.Mouse0) ? true : false;
    }
}
