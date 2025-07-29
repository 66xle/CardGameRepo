using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public bool LeftClickInputDown { get; private set; } = false;

    public bool HoldLeftClickInput { get; private set; } = false;

    public bool LeftClickInputUp { get; private set; } = false;

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        LeftClickInputDown = Input.GetKeyDown(KeyCode.Mouse0) ? true : false;
        HoldLeftClickInput = Input.GetKey(KeyCode.Mouse0) ? true : false;
        LeftClickInputUp = Input.GetKeyUp(KeyCode.Mouse0) ? true : false;
#else
        // Use touch input on mobile
        LeftClickInputDown = false;
        HoldLeftClickInput = false;
        LeftClickInputUp = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                LeftClickInputDown = true;
            else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                HoldLeftClickInput = true;
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                LeftClickInputUp = true;
        }       
#endif
    }
}
