using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

public class GridMap : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] float gridXSize = 13;
    [SerializeField] float gridZSize= 13;
    [SerializeField] float hexSize = 0.577f;

    [Header("Path Settings")]
    [SerializeField][Range(1, 3)] int minPath = 1;

    [Header("Event Tile Settings")]
    [SerializeField] float numOfEvents = 5;
    [SerializeField] float distanceBetweenEvents = 2;
    [SerializeField] float minRowEvent = 3;

    [Header("Hex Prefabs")]
    [SerializeField] GameObject greenPrefab;
    [SerializeField] GameObject bluePrefab;
    [SerializeField] GameObject yellowPrefab;
    [SerializeField] GameObject blackPrefab;

    [Header("References")]
    [SerializeField] Material redMat;
    [SerializeField] Material blueMat;
    [SerializeField] Material purpleMat;
    [SerializeField] EventManager eventManager;
    [SerializeField] EventDisplay eventDisplay;
    [SerializeField] InputManager inputManager;

    #region Internal Variables

    private List<List<Tile>> gripMap;
    private List<DisplayTile> eventList;
    public List<DisplayTile> nextTileList;
    private Tile currentTile;
    private DisplayTile prevDisplayTile;

    private bool canGenerateTile;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        gripMap = new List<List<Tile>>();
        eventList = new List<DisplayTile>();
        nextTileList = new List<DisplayTile>();
        currentTile = null;
        prevDisplayTile = null;
        canGenerateTile = true;

        GenerateGrid();
        GenerateEventTiles();
    }

    void Update()
    {
        if (canGenerateTile && !eventDisplay.disableTileInteract)
        {
            canGenerateTile = false;

            GenerateTile();
        }
        else if (inputManager.leftClickInputDown && !eventDisplay.disableTileInteract)
        { // Detect correct input / disable input

            DetectTileHit();
        }
    }

    #region Generate Tile

    void GenerateGrid()
    {
        // Math stuff for missing tiles
        int index = -1;
        int middleNum = Mathf.RoundToInt(gridZSize / 2) + 1;

        GameObject tilePrefab = greenPrefab;

        #region Loop Grid

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

        #endregion
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
            #region Determine if tile can be placed

            // Get random tile position
            int randomX = (int)Random.Range(0, tempList.Count - 1);
            int randomZ = (int)Random.Range(0, tempList[randomX].Count - 1);

            List<Tile> tileRow = tempList[randomX];
            Tile randomTile = tileRow[randomZ];

            // If tile is avaliable or Is near a event tile
            if (randomTile.type != Tile.Type.Availiable || IsNearEventTiles(randomTile, distanceBetweenEvents))
            {
                RemoveTile(tempList, randomX, randomZ);

                continue;
            }

            #endregion

            // Store event tile information
            gripMap[randomTile.x][randomTile.z].type = Tile.Type.Event;

            RemoveTile(tempList, randomX, randomZ);

            #region Spawn Tile

            // Spawn tile
            Vector3 worldPosition = CalculateWorldPostion(randomTile.x, randomTile.z);
            DisplayTile spawnedTile = Instantiate(yellowPrefab, worldPosition, Quaternion.identity, transform).AddComponent<DisplayTile>();
            spawnedTile.SetupDisplayTile(randomTile);
            spawnedTile.eventObj = eventManager.GetEventFromQueue();

            #endregion

            eventList.Add(spawnedTile);
        }
    }

    void GenerateTile() // Generate tile when event has ended
    {
        if (currentTile == null)
        {
            GenerateStartTile();
        }
        else if (currentTile.x < gridXSize - 1)
        {
            GenerateNextTiles();
        }
        else
        {
            // Cycle finished
            NewCycle();
            return;
        }

        CheckForEventTiles();
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

        nextTileList.Add(selectedTile);
    }

    void NewCycle()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Init();
    }

    void GenerateNextTiles()
    {
        #region Determine which direction tiles can be placed

        // Check if tile even/odd then offset tile
        Tile leftTile = (currentTile.z % 2 == 0) ? OffsetTile(currentTile.x, currentTile.z + 1) : OffsetTile(currentTile.x + 1, currentTile.z + 1);
        Tile forwardTile = OffsetTile(currentTile.x + 1, currentTile.z);
        Tile rightTile = (currentTile.z % 2 == 0) ? OffsetTile(currentTile.x, currentTile.z - 1) : OffsetTile(currentTile.x + 1, currentTile.z - 1);

        

        List<Tile> temp = new List<Tile>(new List<Tile> { leftTile, forwardTile, rightTile });

        List<Tile> tileDirection = new List<Tile>();

        // Check if tiles are avaliable
        for (int i = 0; i < 3; i++)
        {
            Tile tile = temp[i];

            if (tile == null)
                continue;

            if (gripMap[tile.x][tile.z].type != Tile.Type.Availiable)
                continue;

            tileDirection.Add(tile);
        }


        #endregion

        // Get random number of tiles to spawn
        int numOfTileToSpawn = Random.Range(minPath, tileDirection.Count + 1);

        for (int i = 0; i < numOfTileToSpawn; i++)
        {
            if (tileDirection.Count == 0)
                break;

            // Choose a random tile
            Tile rdmTile = tileDirection[Random.Range(0, tileDirection.Count)];

            if (rdmTile == null)
            {
                Debug.Log("test");
            }

            tileDirection.Remove(rdmTile);

            #region Spawn Tile

            // Spawn tile
            Vector3 worldPostion = CalculateWorldPostion(rdmTile.x, rdmTile.z);
            DisplayTile spawnedTile = Instantiate(blackPrefab, worldPostion, Quaternion.identity, transform).AddComponent<DisplayTile>();
            spawnedTile.SetupDisplayTile(rdmTile);
            spawnedTile.eventObj = eventManager.GetEventFromQueue();
            nextTileList.Add(spawnedTile);

            #endregion
        }
    }

    #endregion

    #region Selecting Event Tile

    void CheckForEventTiles()
    {
        DisplayTile eventTile = GetNearEventTile(currentTile, 1);

        if (eventTile != null)
        {
            nextTileList.Add(eventTile);
        }
    }

    void DetectTileHit()
    {
        // Raycast from screen to tile
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            // Get selected tile
            DisplayTile selectedTile = hit.transform.GetComponent<DisplayTile>();

            if (nextTileList.Contains(selectedTile))
            {
                if (eventList.Contains(selectedTile))
                    eventList.Remove(selectedTile);

                LoadEventTile(selectedTile);
            }
        }
    }

    void LoadEventTile(DisplayTile selectedTile)
    {
        // Remove other tiles, not selected tile
        nextTileList.Remove(selectedTile);

        List<DisplayTile> tempList = Extensions.Clone(nextTileList);
        foreach (DisplayTile nextTile in tempList)
        {
            // If skip event tile, disable it
            if (eventList.Contains(nextTile))
            {
                nextTile.GetComponent<MeshRenderer>().material = purpleMat;
                nextTileList.Remove(nextTile);
                eventList.Remove(nextTile);
                continue;
            }

            Destroy(nextTile.gameObject);
        }
        nextTileList.Clear();

        // Debug
        selectedTile.gameObject.GetComponent<MeshRenderer>().material = blueMat;

        // Load Event UI
        eventDisplay.Display(selectedTile.eventObj);

        if (prevDisplayTile != null)
            prevDisplayTile.gameObject.GetComponent<MeshRenderer>().material = redMat;

        // Store event tile
        prevDisplayTile = selectedTile;
        currentTile = new Tile(selectedTile);
        eventDisplay.disableTileInteract = true;

        canGenerateTile = true;
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

    bool IsNearEventTiles (Tile selectedTile, float distance)
    {
        foreach (DisplayTile tile in eventList)
        {
            float disX = Mathf.Abs(tile.dx - selectedTile.x);
            float disZ = Mathf.Abs(tile.dz - selectedTile.z);

             if (disX <= distance)
            {
                if (disZ <= distance)
                {
                    return true;
                }
            }
        }

        return false;
    }

    DisplayTile GetNearEventTile(Tile selectedTile, float distance)
    {
        foreach (DisplayTile tile in eventList)
        {
            float disX = Mathf.Abs(tile.dx - selectedTile.x);
            float disZ = Mathf.Abs(tile.dz - selectedTile.z);

            if (disX <= distance)
            {
                if (disZ <= distance)
                {
                    return tile;
                }
            }
        }

        return null;
    }

    Tile OffsetTile(int x, int z)
    {
        if (x < 0 || x > gridXSize - 1 || z < 0 || z > gridZSize - 1)
        {
            return null;
        }

        return new Tile(x, z);
    }

    #endregion

}
