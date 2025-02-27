

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PointData
{
    public string Guid;
    public Vector3 Position;
    public bool IsStart;

    public List<Connection> Connections = new List<Connection>();
    
}
