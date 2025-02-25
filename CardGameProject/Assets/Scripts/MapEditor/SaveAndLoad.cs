using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
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
            List<string> guidList = new List<string>();

            // Get all target guids that are linked to current point
            foreach (LinkData data in point.links)
            {
                Point targetPoint = data.targetObj.GetComponent<Point>();
                guidList.Add(targetPoint.guid);
            }

            PointData pointData = new PointData
            {
                Guid = point.guid,
                Position = point.gameObject.transform.position,
                IsStart = false,
                Connections = guidList
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
        Debug.Log(path);


        MapData loadedAsset = AssetDatabase.LoadAssetAtPath(path, typeof(MapData)) as MapData;

        if (loadedAsset == null)
        {
            Debug.Log("Is null");
            return;
        }

        Debug.Log("clear");

        ClearMap();

    }

    void ClearMap()
    {
        Point[] pointObjArr = FindObjectsByType<Point>(FindObjectsSortMode.None);
        
        foreach (Point point in pointObjArr)
        {
            point.DestoryPoint();
        }
    }
}
