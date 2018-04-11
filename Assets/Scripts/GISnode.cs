using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISnode
{
    public long id { get; set; }
    public Vector2d latlon { get; set; }
    public Dictionary<string, string> tags { get; set; }

    public int uid { get; set; }
    public string user { get; set; }

    public DateTime timestamp { get; set; }
    public long changeset { get; set; }
    public int version { get; set; }
    public bool visible { get; set; }

    
    public GISnode(long id = 0, double latitude = 0.0, double longitude = 0.0)
    {
        this.id = id;
        latlon = new Vector2d(latitude, longitude);
        tags = new Dictionary<string, string>();
    }
}
