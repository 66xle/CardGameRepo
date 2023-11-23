using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Tile
{
    public enum Type
    {
        Event,
        Missing,
        Availiable
    }

    public int x;
    public int z;
    public Type type;

    public Tile(Tile tile)
    {
        x = tile.x;
        z = tile.z;
        type = tile.type;
    }

    public Tile(int x, int z, Type type)
    {
        this.x = x;
        this.z = z;
        this.type = type;
    }
}
