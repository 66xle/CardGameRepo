using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public List<LinkData> links = new List<LinkData>();

    public GameObject currentLinkObj;

    public LayerMask mapLayerMask;


    private bool isLinkActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLinkActive)
        {
            UpdateLinkPosition();
        }
    }

    public void UpdateLinkPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, mapLayerMask))
        {
            SetLinkTransform(currentLinkObj, hit.point);
        }
    }

    public void SetLinkTransform(GameObject linkObj, Vector3 endPos)
    {
        Vector3 dir = endPos - transform.position;

        Vector3 newPosition = transform.position + (dir / 2);

        linkObj.transform.position = newPosition;
        linkObj.transform.rotation = Quaternion.LookRotation(dir.normalized);
        linkObj.transform.localScale = new Vector3(0.2f, 0.2f, dir.magnitude);
    }

    public void CreateLink(GameObject linkPrefab, Transform parent)
    {
        currentLinkObj = Instantiate(linkPrefab, transform.position, Quaternion.identity, parent);

        isLinkActive = true;
    }

    public void AddLink(GameObject pointB)
    {
        GameObject pointA = transform.gameObject;

        LinkData pointAData = new LinkData();
        pointAData.linkObj = currentLinkObj;
        pointAData.targetObj = pointB;

        LinkData pointBData = new LinkData();
        pointBData.linkObj = currentLinkObj;
        pointBData.targetObj = pointA;

        links.Add(pointAData);
        pointB.GetComponent<Point>().links.Add(pointBData);


        Link link = currentLinkObj.GetComponent<Link>();
        link.pointA = pointA;
        link.pointB = pointB;


        SetLinkTransform(currentLinkObj, pointB.transform.position);

        isLinkActive = false;
    }

    public void SetConnectedLinkPosition()
    {
        if (links.Count == 0) return;

        foreach (LinkData data in links)
        {
            GameObject linkObj = data.linkObj;
            GameObject targetObj = data.targetObj;

            SetLinkTransform(linkObj, targetObj.transform.position);
        }
    }

    public void DestroyLink()
    {
        Destroy(currentLinkObj);

        isLinkActive = false;
    }
}
