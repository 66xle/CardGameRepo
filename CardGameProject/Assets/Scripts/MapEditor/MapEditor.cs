using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapEditor : MonoBehaviour
{
    public GameObject contextMenu;
    public Transform pointHolder;
    public GameObject pointPrefab;
    public LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        
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
