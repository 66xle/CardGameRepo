using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float camSpeed = 0.5f;
    public float zoomSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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



        transform.position = new Vector3(xAxis * camSpeed + transform.position.x, yAxis * zoomSpeed + transform.position.y, zAxis * camSpeed + transform.position.z); 
    }
}
