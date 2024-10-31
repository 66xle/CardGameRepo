using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject contextMenu;

    [Separator]

    public float camSpeed = 0.5f;
    public float zoomSpeed = 0.5f;


    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        float xAxis = Input.GetAxisRaw("Horizontal");
        float zAxis = Input.GetAxisRaw("Vertical");
        float yAxis = Input.GetAxisRaw("Mouse ScrollWheel");

        if (xAxis != 0 || zAxis != 0 || yAxis != 0)
        {
            contextMenu.SetActive(false);
        }


        transform.position = new Vector3(xAxis * camSpeed + transform.position.x, yAxis * zoomSpeed + transform.position.y, zAxis * camSpeed + transform.position.z); 
    }
}
