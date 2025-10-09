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
        // --- PC Input ---
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.OSXPlayer ||
            Application.platform == RuntimePlatform.LinuxPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXEditor)
        {
            LeftClickInputDown = Input.GetKeyDown(KeyCode.Mouse0);
            HoldLeftClickInput = Input.GetKey(KeyCode.Mouse0);
            LeftClickInputUp = Input.GetKeyUp(KeyCode.Mouse0);
        }
        // --- Mobile Input ---
        else if (Application.platform == RuntimePlatform.Android ||
                 Application.platform == RuntimePlatform.IPhonePlayer)
        {
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
        }
    }
}
