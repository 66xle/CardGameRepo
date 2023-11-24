using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

public class GridMap : MonoBehaviour
{
    [SerializeField] float gridXSize;
    [SerializeField] float gridZSize;
    [SerializeField] float hexSize = 0.577f;

    [SerializeField] GameObject greenPrefab;
    [SerializeField] GameObject bluePrefab;
    [SerializeField] GameObject yellowPrefab;
    [SerializeField] GameObject blackPrefab;

    [Header("Settings")]
    [SerializeField] float numOfEvents = 5;
    [SerializeField] float distanceBetweenEvents = 2;
    [SerializeField] float minPlaceEvent = 3;

    private List<List<Tile>> tileMap;
    private List<Tile> eventList;
    private Tile currentTile;
    private List<DisplayTile> currentNextTiles;

    // Start is called before the first frame update
    void Start()
    {
        tileMap = new List<List<Tile>>();
        eventList = new List<Tile>();
        currentNextTiles = new List<DisplayTile>();

        GenerateGrid();
        PlaceEventTiles();
        GenerateStartTile();
    }

    void Update()
    {
        DetectTileHit();
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


                Vector3 worldPosition = CalculateWorldPostion(x, z);
                worldPosition.y -= 1f;

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
            Instantiate(yellowPrefab, worldPosition, Quaternion.identity, transform).AddComponent<DisplayTile>().SetupDisplayTile(randomTile);
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

    void GenerateStartTile()
    {
        int middleNum = Mathf.RoundToInt(gridZSize / 2);

        currentTile = tileMap[0][middleNum];

        Vector3 worldPosition = CalculateWorldPostion(currentTile.x, currentTile.z);

        DisplayTile selectedTile = Instantiate(blackPrefab, worldPosition, Quaternion.identity, transform).AddComponent<DisplayTile>();
        selectedTile.SetupDisplayTile(currentTile);
        currentNextTiles.Add(selectedTile);
    }

    void GenerateNextTiles()
    {
        // Check if tile is even num
        Tile leftTile = (currentTile.z % 2 == 0) ? new Tile(currentTile.x, (int)Mathf.Clamp(currentTile.z + 1, 0, gridZSize - 1)) : 
                                                   new Tile((int)Mathf.Clamp(currentTile.x + 1, 0, gridXSize), (int)Mathf.Clamp(currentTile.z + 1, 0, gridZSize - 1));

        Tile forwardTile = new Tile((int)Mathf.Clamp(currentTile.x + 1, 0, gridXSize), currentTile.z);

        Tile rightTile = (currentTile.z % 2 == 0) ? new Tile(currentTile.x, (int)Mathf.Clamp(currentTile.z - 1, 0, gridZSize - 1)) : 
                                                    new Tile((int)Mathf.Clamp(currentTile.x + 1, 0, gridXSize), (int)Mathf.Clamp(currentTile.z - 1, 0, gridZSize - 1));

        List<Tile> tileDirection = new List<Tile>(new List<Tile> { leftTile, forwardTile, rightTile });

        for (int i = 0; i < tileDirection.Count; i++)
        {
            Tile tile = tileDirection[i];

            if (tileMap[tile.x][tile.z].type != Tile.Type.Availiable)
            {
                tileDirection.Remove(tile);
            }
        }



        int numOfTileToSpawn = Random.Range(1, tileDirection.Count + 1);
        Debug.Log(tileDirection.Count);

        for (int i = 0; i < numOfTileToSpawn; i++)
        {
            Tile rdmTile = tileDirection[Random.Range(0, tileDirection.Count)];
            tileDirection.Remove(rdmTile);

            Vector3 worldPostion = CalculateWorldPostion(rdmTile.x, rdmTile.z);
            DisplayTile spawnedTile = Instantiate(blackPrefab, worldPostion, Quaternion.identity, transform).AddComponent<DisplayTile>();
            spawnedTile.SetupDisplayTile(rdmTile);
            currentNextTiles.Add(spawnedTile);
        }
    }

    void DetectTileHit()
    {
        if (!Input.GetKeyUp(KeyCode.Mouse0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            DisplayTile selectedTile = hit.transform.GetComponent<DisplayTile>();

            if (currentNextTiles.Contains(selectedTile))
            {

                currentTile = new Tile(selectedTile);
                currentNextTiles.Clear();
                GenerateNextTiles();
            }
        }
    }
}
