using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MapEditor : MonoBehaviour
{
    public GameObject contextMenu;
    public GameObject pointContextMenu;

    [Separator]

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

    [Separator]

    public GameObject seletedObject;
    public GameObject seletedLink;
    private Vector3 offSetDir;

    public bool isMenuOpen;
    private bool holdingDownLeftClick;
    private bool isLinkActive;

    // Start is called before the first frame update
    void Start()
    {
        holdingDownLeftClick = false;
        isLinkActive = false;
        isMenuOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        DetectInput();
    }

    void DetectInput()
    {
        RightClick();
        LeftClick();

        DeleteObj();
    }

    void LeftClick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (!isMenuOpen)
                SelectPoint();

            if (isLinkActive)
                DestroyLink();
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

                // set links position
                seletedObject.GetComponent<Point>().SetConnectedLinkPosition();
            }
        }
    }

    void RightClick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (isLinkActive)
                DestroyLink();

            if (seletedObject != null)
            {
                seletedObject.transform.GetComponent<MeshRenderer>().material = defaultMat;
                seletedObject = null;
            }

            if (seletedLink != null)
            {
                seletedLink.transform.GetComponent<MeshRenderer>().material = defaultMat;
                seletedLink = null;
            }

            if (isMenuOpen)
            {
                contextMenu.SetActive(false);
                pointContextMenu.SetActive(false);
                isMenuOpen = false;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, ~linkLayerMask))
            {
                if (hit.transform.CompareTag("Point"))
                {
                    pointContextMenu.transform.position = Input.mousePosition;
                    pointContextMenu.SetActive(true);
                    isMenuOpen = true;

                    seletedObject = hit.transform.gameObject;
                    seletedObject.transform.GetComponent<MeshRenderer>().material = pointSeletedMat;

                    return;
                }
            }

            contextMenu.transform.position = Input.mousePosition;
            contextMenu.SetActive(true);
            isMenuOpen = true;
        }
    }

    void SelectPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (seletedObject != null && !isLinkActive)
            {
                seletedObject.transform.GetComponent<MeshRenderer>().material = defaultMat;
                seletedObject = null;
            }

            if (seletedLink != null)
            {
                seletedLink.transform.GetComponent<MeshRenderer>().material = defaultMat;
                seletedLink = null;
            }

            if (hit.transform.CompareTag("Point"))
            {
                if (isLinkActive)
                {
                    seletedObject.GetComponent<Point>().AddLink(hit.transform.gameObject);
                    seletedObject.transform.GetComponent<MeshRenderer>().material = defaultMat;
                    seletedObject = null;

                    isLinkActive = false;

                    return;
                }

                seletedObject = hit.transform.gameObject;
                seletedObject.transform.GetComponent<MeshRenderer>().material = seletedMat;

                offSetDir = seletedObject.transform.position - hit.point;

                holdingDownLeftClick = true;
            }
            else if (hit.transform.CompareTag("Link"))
            {
                if (!isLinkActive)
                {
                    seletedLink = hit.transform.gameObject;
                    seletedLink.transform.GetComponent<MeshRenderer>().material = seletedMat;
                }
            }
        }
    }

    void DeleteObj()
    {
        if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
        {
            if (seletedObject != null)
            {
                seletedObject.GetComponent<Point>().DestoryPoint();
                seletedLink = null;
            }
            else if (seletedLink != null)
            {
                seletedLink.GetComponent<Link>().DeleteLink();
                seletedLink = null;
            }
        }
    }

    public void CreatePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(contextMenu.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            GameObject point = Instantiate(pointPrefab, hit.point, Quaternion.identity, pointHolder);
            point.GetComponent<Point>().mapLayerMask = mapLayerMask;

            contextMenu.SetActive(false);
            isMenuOpen = false;
        }
    }

    private void DestroyLink()
    {
        seletedObject.GetComponent<Point>().DestroyTempLink();
        seletedObject.transform.GetComponent<MeshRenderer>().material = defaultMat;
        seletedObject = null;

        isLinkActive = false;
    }

    public void CreateLink()
    {
        seletedObject.GetComponent<Point>().CreateLinkActive(linkPrefab, linkHolder);

        isLinkActive = true;

        pointContextMenu.SetActive(false);
        isMenuOpen = false;
    }

    public void CloseContextMenu()
    {
        contextMenu.SetActive(false);
        isMenuOpen = false;

        SelectPoint();
    }

    public void ClosePointContextMenu()
    {
        pointContextMenu.SetActive(false);
        isMenuOpen = false;

        SelectPoint();
    }

}
