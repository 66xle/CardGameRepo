using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapEditor : MonoBehaviour
{
    public GameObject contextMenu;
    public LayerMask layerMask;

    [Separator]

    public Transform pointHolder;
    public GameObject pointPrefab;

    [Separator]

    public Material seletedMat;
    public Material defaultMat;

    private GameObject seletedObject;
    private Vector3 offSetDir;

    private bool holdingDownLeftClick;

    // Start is called before the first frame update
    void Start()
    {
        holdingDownLeftClick = false;
    }

    // Update is called once per frame
    void Update()
    {
        DetectInput();
    }

    void DetectInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
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
                    seletedObject = hit.transform.gameObject;
                    seletedObject.transform.GetComponent<MeshRenderer>().material = seletedMat;

                    offSetDir = seletedObject.transform.position - hit.point;

                    Debug.Log("Offset Direction Calculated: " + offSetDir); // Debug output


                    holdingDownLeftClick = true;
                }
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
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            Instantiate(pointPrefab, hit.point, Quaternion.identity, pointHolder);
            CloseContextMenu();
        }
    }

    public void CloseContextMenu()
    {
        contextMenu.SetActive(false);
    }

}
