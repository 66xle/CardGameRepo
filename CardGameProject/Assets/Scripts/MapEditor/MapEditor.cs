using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapEditor : MonoBehaviour
{
    public GameObject contextMenu;
    public GameObject pointContextMenu;
    public LayerMask mapLayerMask;
    public LayerMask linkLayerMask;

    [Separator]

    public Transform pointHolder;
    public Transform linkHolder;
    public GameObject pointPrefab;
    public GameObject linkPrefab;

    [Separator]

    public Material seletedMat;
    public Material pointSeletedMat;
    public Material defaultMat;

    private GameObject seletedObject;
    private GameObject currentActiveLink;
    private Vector3 offSetDir;

    private bool holdingDownLeftClick;
    private bool isLinkActive;

    // Start is called before the first frame update
    void Start()
    {
        holdingDownLeftClick = false;
        isLinkActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        DetectInput();

        if (isLinkActive)
        {
            UpdateLinkPosition();
        }
    }

    void DetectInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (isLinkActive)
            {
                Destroy(currentActiveLink);
                isLinkActive = false;

                seletedObject.transform.GetComponent<MeshRenderer>().material = defaultMat;
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, ~linkLayerMask))
            {
                if (hit.transform.CompareTag("Point"))
                {
                    pointContextMenu.transform.position = Input.mousePosition;
                    pointContextMenu.SetActive(true);

                    seletedObject = hit.transform.gameObject;
                    seletedObject.transform.GetComponent<MeshRenderer>().material = pointSeletedMat;

                    return;
                }
            }

            contextMenu.transform.position = Input.mousePosition;
            contextMenu.SetActive(true);

        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (seletedObject != null)
                    seletedObject.transform.GetComponent<MeshRenderer>().material = defaultMat;

                if (hit.transform.CompareTag("Point"))
                {
                    if (isLinkActive)
                    {
                        GameObject pointA = seletedObject.transform.gameObject;
                        GameObject pointB = hit.transform.gameObject;

                        pointA.GetComponent<Point>().links.Add(pointB);
                        pointB.GetComponent<Point>().links.Add(pointA);

                        SetLinkTransform(hit.transform.position);

                        isLinkActive = false;

                        return;
                    }

                    seletedObject = hit.transform.gameObject;
                    seletedObject.transform.GetComponent<MeshRenderer>().material = seletedMat;

                    offSetDir = seletedObject.transform.position - hit.point;

                    holdingDownLeftClick = true;
                }
            }

            if (isLinkActive)
            {
                Destroy(currentActiveLink);
                isLinkActive = false;

                seletedObject.transform.GetComponent<MeshRenderer>().material = defaultMat;
                return;
            }

        }
        
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            holdingDownLeftClick = false;
        }

        if (holdingDownLeftClick)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                Vector3 newPosition = hit.point + offSetDir;
                newPosition.y = seletedObject.transform.position.y;

                // set position
                seletedObject.transform.position = newPosition; 
            }
        }
    }

    

    public void CreatePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(contextMenu.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            Instantiate(pointPrefab, hit.point, Quaternion.identity, pointHolder);
            CloseContextMenu();
        }
    }

    public void CreateLink()
    {
        currentActiveLink = Instantiate(linkPrefab, seletedObject.transform.position, Quaternion.identity, linkHolder);

        isLinkActive = true;

        ClosePointContextMenu();
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


    public void CloseContextMenu()
    {
        contextMenu.SetActive(false);
    }

    public void ClosePointContextMenu()
    {
        pointContextMenu.SetActive(false);
    }


    private void SetLinkTransform(Vector3 endPos)
    {
        Vector3 dir = endPos - seletedObject.transform.position;

        Vector3 newPosition = seletedObject.transform.position + (dir / 2);

        currentActiveLink.transform.position = newPosition;
        currentActiveLink.transform.rotation = Quaternion.LookRotation(dir.normalized);
        currentActiveLink.transform.localScale = new Vector3(0.2f, 0.2f, dir.magnitude);
    }
}
