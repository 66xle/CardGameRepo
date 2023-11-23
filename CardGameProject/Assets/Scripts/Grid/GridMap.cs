using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;
using System.Linq;
using TreeEditor;

public class GridMap : MonoBehaviour
{
    [SerializeField] float gridXSize;
    [SerializeField] float gridZSize;
    [SerializeField] float hexSize = 0.577f;

    [SerializeField] GameObject greenPrefab;
    [SerializeField] GameObject bluePrefab;
    [SerializeField] GameObject yellowPrefab;

    [Header("Settings")]
    [SerializeField] float numOfEvents = 5;
    [SerializeField] float distanceBetweenEvents = 2;
    [SerializeField] float minPlaceEvent = 3;

    private List<List<Tile>> tileMap;
    private List<Tile> eventList;

    // Start is called before the first frame update
    void Start()
    {
        tileMap = new List<List<Tile>>();
        eventList = new List<Tile>();

        GenerateGrid();
        PlaceEventTiles();
    }

    void GenerateGrid()
    {
        // Math stuff for missing tiles
        int index = -1;
        int middleNum = Mathf.RoundToInt(gridZSize / 2) + 1;

        GameObject tilePrefab = greenPrefab;

        for (int x = 0; x < gridXSize; x++)
        {
            index += 2;
            List<Tile> row = new List<Tile>();

            for (int z = 0; z < gridZSize; z++)
            {
                #region Determine Missing tiles

                float num = Mathf.Abs(z + 1 - middleNum);

                // Bad
                if (num > index)
                {
                    row.Add(new Tile(x, z, Tile.Type.Missing));
                    continue;
                }
                else
                {
                    row.Add(new Tile(x, z, Tile.Type.Availiable));
                }

                #endregion


                //Vector3 worldPosition = CalculateWorldPostion(x, z);
                //Instantiate(tilePrefab, worldPosition, Quaternion.identity, transform);

                // Move left
                float moveLeft = 3f / 2f * hexSize * z;

                float height = Mathf.Sqrt(3f) * hexSize;

                // Move forwards
                float moveForward = height * x;

                // If z is odd, offset forward a bit
                if (z % 2 != 0)
                {
                    moveForward += height / 2f;
                }

                Vector3 worldPosition =  new Vector3(transform.position.x + moveForward, transform.position.y - 1f, transform.position.z + moveLeft);
                Instantiate(tilePrefab, worldPosition, Quaternion.identity, transform);

            }

            // Prefab rows
            if (tilePrefab == greenPrefab)
                tilePrefab = bluePrefab;
            else
                tilePrefab = greenPrefab;


            tileMap.Add(row);
        }
    }

    Vector3 CalculateWorldPostion(float x, float z)
    {
        // Move left
        float moveLeft = 3f / 2f * hexSize * z;

        float height = Mathf.Sqrt(3f) * hexSize;

        // Move forwards
        float moveForward = height * x;

        // If z is odd, offset forward a bit
        if (z % 2 != 0)
        {
            moveForward += height / 2f;
        }

        return new Vector3(transform.position.x + moveForward, transform.position.y, transform.position.z + moveLeft);
    }

    void PlaceEventTiles()
    {
        List<List<Tile>> tempList = new List<List<Tile>>();

        // Clone list
        foreach (List<Tile> tileList in tileMap)
        {
            List<Tile> row = new List<Tile>();

            foreach (Tile tile in tileList)
            {
                Tile newTile = new Tile(tile);

                row.Add(newTile);
            }

            tempList.Add(row);
        }

        tempList.RemoveRange(0, (int)minPlaceEvent);
        tempList.RemoveAt(tempList.Count - 1);


        while (eventList.Count < numOfEvents && tempList.Count > 0)
        {
            int randomX = (int)Random.Range(0, tempList.Count - 1);
            int randomZ = (int)Random.Range(0, tempList[randomX].Count - 1);

            List<Tile> tileRow = tempList[randomX];
            Tile randomTile = tileRow[randomZ];

            if (randomTile.type != Tile.Type.Availiable || IsNearEventTiles(randomTile))
            {
                RemoveTile(tempList, randomX, randomZ);

                continue;
            }

            eventList.Add(randomTile);
            tileMap[randomTile.x][randomTile.z].type = Tile.Type.Event;

            RemoveTile(tempList, randomX, randomZ);

            Vector3 worldPosition = CalculateWorldPostion(randomTile.x, randomTile.z);
            Instantiate(yellowPrefab, worldPosition, Quaternion.identity, transform);
        }
    }

    void RemoveTile(List<List<Tile>> tempList, int x, int z)
    {
        List<Tile> tileRow = tempList[x];
        Tile randomTile = tileRow[z];

        tileRow.Remove(randomTile);

        if (tileRow.Count <= 0)
            tempList.Remove(tileRow);
    }

    bool IsNearEventTiles (Tile selectedTile)
    {
        foreach (Tile tile in eventList)
        {
            float disX = Mathf.Abs(tile.x - selectedTile.x);
            float disZ = Mathf.Abs(tile.z - selectedTile.z);

             if (disX <= distanceBetweenEvents)
            {
                if (disZ <= distanceBetweenEvents)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
