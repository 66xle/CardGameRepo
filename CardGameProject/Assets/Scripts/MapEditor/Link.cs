using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Link : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;


    public void DeleteLink()
    {
        RemoveLink(pointA);
        RemoveLink(pointB);

        Destroy(gameObject);
    }

    void RemoveLink(GameObject obj)
    {
        Point point = obj.GetComponent<Point>();

        LinkData removeLink = point.links.First(data => data.linkObj == gameObject);

        point.links.Remove(removeLink);
    }
}
