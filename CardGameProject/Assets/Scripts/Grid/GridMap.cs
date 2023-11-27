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
    [SerializeField] Material redMat;
    [SerializeField] EventManager eventManager;
    [SerializeField] EventDisplay eventDisplay;

    [Header("Settings")]
    [SerializeField] float numOfEvents = 5;
    [SerializeField] float distanceBetweenEvents = 2;
    [SerializeField] float minRowEvent = 3;

    private List<List<Tile>> gripMap;
    private List<Tile> eventList;
    private Tile currentTile;
    private List<DisplayTile> nextTiles;

    [HideInInspector] public bool disableTileInteract = false;

    // Start is called before the first frame update
    void Start()
    {
        gripMap = new List<List<Tile>>();
        eventList = new List<Tile>();
        nextTiles = new List<DisplayTile>();

        disableTileInteract = false;

        GenerateGrid();
        GenerateEventTiles();
        GenerateStartTile();
    }

    void Update()
    {
        DetectTileHit();

        

    }

    #region Generate on Start

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


                // Debugging
                Vector3 worldPosition = CalculateWorldPostion(x, z);
                worldPosition.y -= 1f;

                Instantiate(tilePrefab, worldPosition, Quaternion.identity, transform);
            }

            // Prefab rows
            if (tilePrefab == greenPrefab)
                tilePrefab = bluePrefab;
            else
                tilePrefab = greenPrefab;


            gripMap.Add(row);
        }
    }

    void GenerateEventTiles()
    {
        List<List<Tile>> tempList = new List<List<Tile>>();

        #region Clone List
        foreach (List<Tile> tileList in gripMap)
        {
            List<Tile> row = new List<Tile>();

            foreach (Tile tile in tileList)
            {
                Tile newTile = new Tile(tile);

                row.Add(newTile);
            }

            tempList.Add(row);
        }
        #endregion

        // Determine which rows events can be placed in
        tempList.RemoveRange(0, (int)minRowEvent);
        tempList.RemoveAt(tempList.Count - 1);

        // Place all events or when grid has no more space
        while (eventList.Count < numOfEvents && tempList.Count > 0)
        {
            // Get random tile position
            int randomX = (int)Random.Range(0, tempList.Count - 1);
            int randomZ = (int)Random.Range(0, tempList[randomX].Count - 1);

            List<Tile> tileRow = tempList[randomX];
            Tile randomTile = tileRow[randomZ];

            // If tile is avaliable or Is near a event tile
            if (randomTile.type != Tile.Type.Availiable || IsNearEventTiles(randomTile))
            {
                RemoveTile(tempList, randomX, randomZ);

                continue;
            }

            // Store event tile information
            eventList.Add(randomTile);
            gripMap[randomTile.x][randomTile.z].type = Tile.Type.Event;

            RemoveTile(tempList, randomX, randomZ);

            // Spawn tile
            Vector3 worldPosition = CalculateWorldPostion(randomTile.x, randomTile.z);
            DisplayTile spawnedTile = Instantiate(yellowPrefab, worldPosition, Quaternion.identity, transform).AddComponent<DisplayTile>();
            spawnedTile.SetupDisplayTile(randomTile);
            spawnedTile.eventObj = eventManager.GetEventFromQueue();
        }
    }

    void GenerateStartTile()
    {
        // Get middle of grid
        int middleNum = Mathf.RoundToInt(gridZSize / 2);
        currentTile = gripMap[0][middleNum];

        Vector3 worldPosition = CalculateWorldPostion(currentTile.x, currentTile.z);

        // Spawn Tile
        DisplayTile selectedTile = Instantiate(blackPrefab, worldPosition, Quaternion.identity, transform).AddComponent<DisplayTile>();
        selectedTile.SetupDisplayTile(currentTile);
        selectedTile.eventObj = eventManager.GetEventFromQueue();

        nextTiles.Add(selectedTile);
    }



    #endregion

    #region Selecting Event Tile

    void GenerateNextTiles()
    {
        // Check if tile even/odd then offset tile
        Tile leftTile = (currentTile.z % 2 == 0) ? OffsetTile(currentTile.x, currentTile.z + 1) : OffsetTile(currentTile.x + 1, currentTile.z + 1);
        Tile forwardTile = OffsetTile(currentTile.x + 1, currentTile.z);
        Tile rightTile = (currentTile.z % 2 == 0) ? OffsetTile(currentTile.x, currentTile.z - 1) : OffsetTile(currentTile.x + 1, currentTile.z - 1);

        List<Tile> tileDirection = new List<Tile>(new List<Tile> { leftTile, forwardTile, rightTile });

        // Check if tiles are avaliable
        for (int i = 0; i < tileDirection.Count; i++)
        {
            Tile tile = tileDirection[i];

            if (gripMap[tile.x][tile.z].type != Tile.Type.Availiable)
            {
                tileDirection.Remove(tile);
            }
        }

        // Get random number of tiles to spawn
        int numOfTileToSpawn = Random.Range(1, tileDirection.Count + 1);

        for (int i = 0; i < numOfTileToSpawn; i++)
        {
            // Choose a random tile
            Tile rdmTile = tileDirection[Random.Range(0, tileDirection.Count)];
            tileDirection.Remove(rdmTile);

            // Spawn tile
            Vector3 worldPostion = CalculateWorldPostion(rdmTile.x, rdmTile.z);
            DisplayTile spawnedTile = Instantiate(blackPrefab, worldPostion, Quaternion.identity, transform).AddComponent<DisplayTile>();
            spawnedTile.SetupDisplayTile(rdmTile);
            spawnedTile.eventObj = eventManager.GetEventFromQueue();
            nextTiles.Add(spawnedTile);
        }
    }

    void DetectTileHit()
    {
        // Detect correct input / disable input
        if (!Input.GetKeyDown(KeyCode.Mouse0) || disableTileInteract)
            return;

        // Raycast from screen to tile
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            // Get selected tile
            DisplayTile selectedTile = hit.transform.GetComponent<DisplayTile>();

            if (nextTiles.Contains(selectedTile))
            {
                // Remove other tiles, not selected tile
                nextTiles.Remove(selectedTile);
                foreach (DisplayTile tile in nextTiles)
                {
                    Destroy(tile.gameObject);
                }
                nextTiles.Clear();

                // Debug
                selectedTile.gameObject.GetComponent<MeshRenderer>().material = redMat;

                // Load Event UI
                eventManager.LoadEvent(selectedTile.eventObj, eventDisplay);

                // Store event tile
                currentTile = new Tile(selectedTile);
                disableTileInteract = true;


                // Here temporary (run this when event ends)
                if (selectedTile.dx < gridXSize - 1)
                    GenerateNextTiles();
            }
        }
    }

    #endregion


    #region Other Functions

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

    Tile OffsetTile(int x, int z)
    {
        int clampX = (int)Mathf.Clamp(x, 0, gridXSize);
        int clampZ = (int)Mathf.Clamp(z, 0, gridZSize - 1);

        return new Tile(clampX, clampZ);
    }

    #endregion

}
