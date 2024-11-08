using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


    private GameObject seletedObject;
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
    }

    void DetectInput()
    {
        RightClick();
        LeftClick();
    }

    void LeftClick()
    {
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
                        seletedObject.GetComponent<Point>().AddLink(hit.transform.gameObject);
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
                seletedObject.transform.GetComponent<MeshRenderer>().material = defaultMat;

            CloseContextMenu();
            ClosePointContextMenu();

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
    }

    public void CreatePoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(contextMenu.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            GameObject point = Instantiate(pointPrefab, hit.point, Quaternion.identity, pointHolder);
            point.GetComponent<Point>().mapLayerMask = mapLayerMask;

            CloseContextMenu();
        }
    }

    private void DestroyLink()
    {
        seletedObject.GetComponent<Point>().DestroyLink();

        isLinkActive = false;
    }

    public void CreateLink()
    {
        seletedObject.GetComponent<Point>().CreateLink(linkPrefab, linkHolder);

        isLinkActive = true;

        ClosePointContextMenu();
    }

    public void CloseContextMenu()
    {
        contextMenu.SetActive(false);
    }

    public void ClosePointContextMenu()
    {
        pointContextMenu.SetActive(false);
    }

}
