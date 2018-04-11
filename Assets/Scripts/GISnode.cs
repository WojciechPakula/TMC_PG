using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISnode {
    public ulong id { get; set; }
    public Vector2d latlon { get; set; }
    public Dictionary<string, string> tags { get; set; }
    
    public GISnode(ulong id = 0, double latitude = 0.0, double longitude = 0.0)
    {
        this.id = id;
        latlon = new Vector2d(latitude, longitude);
        tags = new Dictionary<string, string>();
    }

    public GISnode(ulong id, Vector2d latlon, Dictionary<string, string> tags)
        : this(id, latlon.x, latlon.y)
    {
        this.tags = tags;

    }
}
