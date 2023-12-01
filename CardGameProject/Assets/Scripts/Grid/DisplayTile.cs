using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTile : MonoBehaviour
{
    public enum Type
    {
        Event,
        Missing,
        Availiable
    }

    public int dx;
    public int dz;
    public Event eventObj;

    public void SetupDisplayTile(Tile tile)
    {
        dx = tile.x;
        dz = tile.z;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
