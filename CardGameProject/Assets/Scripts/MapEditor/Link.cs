using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = pointB.transform.position - pointA.transform.position;

        Vector3 newPosition = pointA.transform.position + (dir / 2);

        transform.position = newPosition;
        transform.rotation = Quaternion.LookRotation(dir.normalized);
        transform.localScale = new Vector3(0.2f, 0.2f, dir.magnitude);
    }
}
