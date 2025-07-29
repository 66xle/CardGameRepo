using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class SaveAndLoad : MonoBehaviour
{
    public Transform pointHolder;
    public Transform linkHolder;

    public GameObject pointPrefab;
    public GameObject linkPrefab;

    public List<GameObject> createdPointObj = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveMap()
    {

        Point[] pointObjArr = FindObjectsByType<Point>(FindObjectsSortMode.None);
        

        Debug.Log(pointObjArr.Length);


        List<PointData> pointDataList = new List<PointData>();

        // Check for starting point



        // Loop through
        foreach (Point point in pointObjArr)
        {
            // Get all target guids
            List<Connection> connectionList = new List<Connection>();

            // Get all target guids that are linked to current point
            foreach (LinkData data in point.links)
            {
                Point targetPoint = data.targetObj.GetComponent<Point>();

                Connection newConnection = new Connection();
                newConnection.targetGUID = data.targetObj.GetComponent<Point>().guid;
                newConnection.linkGUID = data.linkObj.GetComponent<Link>().guid;

                connectionList.Add(newConnection);
            }

            PointData pointData = new PointData
            {
                Guid = point.guid,
                Position = point.gameObject.transform.position,
                IsStart = false,
                Connections = connectionList
            };


            // Add points to list
            pointDataList.Add(pointData);
        }

        // Save as an scriptable object
        //MapData loadedAsset = AssetDatabase.LoadAssetAtPath($"Assets/ScriptableObjects/Map/test.asset", typeof(MapData)) as MapData;  
        MapData mapData = new MapData
        {
            pointDataList = pointDataList,
        };

        Debug.Log(mapData.pointDataList.Count);

        AssetDatabase.CreateAsset(mapData, $"Assets/ScriptableObjects/Map/test.asset");

    }

    public void LoadMap()
    {
        string path = EditorUtility.OpenFilePanel("Load", "Assets/ScriptableObjects/Map", "asset");

        // Split because it doesn't like the path before Assets
        string[] split = Regex.Split(path, @"(?=Assets)");
        MapData loadedAsset = AssetDatabase.LoadAssetAtPath<MapData>(split[1]);

        if (loadedAsset == null)
        {
            Debug.Log("Is null");
            return;
        }

        Debug.Log("clear");

        

        ClearMap();
        LoadPoints(loadedAsset);
    }

    void ClearMap()
    {
        Point[] pointObjArr = FindObjectsByType<Point>(FindObjectsSortMode.None);
        
        foreach (Point point in pointObjArr)
        {
            point.DestoryPoint();
        }
    }

    void LoadPoints(MapData loadedAsset)
    {
        createdPointObj.Clear();

        List<string> pointCreatedGUID = new List<string>();
        List<string> linkCreatedGUID = new List<string>();

        // Loop through points
        foreach (PointData data in loadedAsset.pointDataList)
        {
            GameObject baseObj;
            GameObject targetObj;

            if (!pointCreatedGUID.Contains(data.Guid))
            {
                baseObj = CreatePoint(data);
                pointCreatedGUID.Add(data.Guid);
            }
            else
            {
                baseObj = createdPointObj.First(x => x.GetComponent<Point>().guid == data.Guid);
            }
            
            
            // Loop through links
            foreach(Connection connection in data.Connections)
            {
                // Check if point isn't/is created
                if (!pointCreatedGUID.Contains(connection.targetGUID))
                {
                    // Grab pointdata
                    PointData pointData = loadedAsset.pointDataList.First(x => x.Guid == connection.targetGUID);

                    targetObj = CreatePoint(pointData);
                    pointCreatedGUID.Add(connection.targetGUID);
                }
                else
                {
                    targetObj = createdPointObj.First(x => x.GetComponent<Point>().guid == connection.targetGUID);
                }

                // Check if link isn't/is created
                if (!linkCreatedGUID.Contains(connection.linkGUID))
                {
                    Point point = baseObj.GetComponent<Point>();
                    point.CreateLink(linkPrefab, linkHolder);
                    point.AddLink(targetObj);


                    point.currentLinkObj.GetComponent<Link>().guid = connection.linkGUID;

                    linkCreatedGUID.Add(connection.linkGUID);
                }
            }
        }
    }

    GameObject CreatePoint(PointData data)
    {
        GameObject newPoint = Instantiate(pointPrefab, pointHolder);
        newPoint.transform.position = data.Position;
        newPoint.GetComponent<Point>().guid = data.Guid;

        createdPointObj.Add(newPoint);

        return newPoint;
    }
}

#endif