using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool leftClickInputDown { get; private set; }


    // Update is called once per frame
    void Update()
    {
        leftClickInputDown = Input.GetKeyDown(KeyCode.Mouse0) ? true : false;
    }
}
