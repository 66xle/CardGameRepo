using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Link : MonoBehaviour
{
    public string guid;
    public GameObject pointA;
    public GameObject pointB;


    public void Start()
    {
#if UNITY_EDITOR
        guid = GUID.Generate().ToString();
#endif
    }

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
