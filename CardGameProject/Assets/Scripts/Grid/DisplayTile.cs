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
    public Type dType;

    public void SetupDisplayTile(Tile tile)
    {
        dx = tile.x;
        dz = tile.z;
        dType = (Type)tile.type;
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
