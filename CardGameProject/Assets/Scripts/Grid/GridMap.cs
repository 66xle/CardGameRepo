using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    [SerializeField] float gridXSize;
    [SerializeField] float gridZSize;
    [SerializeField] float hexSize = 0.577f;

    [SerializeField] GameObject greenPrefab;
    [SerializeField] GameObject bluePrefab;

    private List<List<string>> gridMap;

    // Start is called before the first frame update
    void Start()
    {
        gridMap = new List<List<string>>();

        GenerateGrid();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateGrid()
    {
        int index = -1;
        int middleNum = Mathf.RoundToInt(gridZSize / 2) + 1;

        for (int x = 0; x < gridXSize; x++)
        {
            index += 2;

            List<string> row = new List<string>();

            for (int z = 0; z < gridZSize; z++)
            {
                float num = Mathf.Abs(z + 1 - middleNum);

                // Bad
                if (num > index)
                {
                    row.Add("x");
                    continue;
                }
                else
                {
                    row.Add("y");
                }


                GameObject tilePrefab = greenPrefab;

                float moveLeft = 3f / 2f * hexSize * z;

                float height = Mathf.Sqrt(3f) * hexSize;

                // height
                float moveForward = height * x;

                // If z is odd
                if (z % 2 != 0)
                {
                    moveForward += height / 2f;
                    tilePrefab = bluePrefab;
                }

                Vector3 worldPosition = new Vector3(transform.position.x + moveForward, transform.position.y, transform.position.z + moveLeft);
                Instantiate(tilePrefab, worldPosition, Quaternion.identity, transform);
                
            }

            gridMap.Add(row);
        }
    }
}
