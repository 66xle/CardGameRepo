using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public List<GameObject> links;

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
            SetLinkTransform(hit.point);
        }
    }

    public void SetLinkTransform(Vector3 endPos)
    {
        Vector3 dir = endPos - transform.position;

        Vector3 newPosition = transform.position + (dir / 2);

        currentLinkObj.transform.position = newPosition;
        currentLinkObj.transform.rotation = Quaternion.LookRotation(dir.normalized);
        currentLinkObj.transform.localScale = new Vector3(0.2f, 0.2f, dir.magnitude);
    }

    public void CreateLink(GameObject linkPrefab, Transform parent)
    {
        currentLinkObj = Instantiate(linkPrefab, transform.position, Quaternion.identity, parent);

        isLinkActive = true;
    }

    public void AddLink(GameObject pointB)
    {
        GameObject pointA = transform.gameObject;

        links.Add(pointB);
        pointB.GetComponent<Point>().links.Add(pointA);

        SetLinkTransform(pointB.transform.position);

        isLinkActive = false;
    }

    public void DestroyLink()
    {
        Destroy(currentLinkObj);

        isLinkActive = false;
    }
}
