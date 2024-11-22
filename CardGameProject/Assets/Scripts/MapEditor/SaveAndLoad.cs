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
        Link[] linkObjArr = FindObjectsByType<Link>(FindObjectsSortMode.None);

        Debug.Log(pointObjArr.Length);

        // Check for starting point


        // Loop through

    }
}
