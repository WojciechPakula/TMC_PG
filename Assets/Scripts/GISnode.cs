using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISnode
{
    public long id { get; set; }
    public Vector2d latlon { get; set; }
    public Vector2d XY {
        get
        {
            Vector2d result;
            result.y = System.Math.Log(System.Math.Tan((latlon.y + 90d) / 360d * System.Math.PI)) / System.Math.PI * 180d;
            result.x = latlon.x;
            return result;
        }
        set
        {
            XY = value;
            Vector2d result;
            result.y = System.Math.Atan(System.Math.Exp(XY.y / 180d * System.Math.PI)) / System.Math.PI * 360d - 90d;
            result.x = XY.x;
            latlon = result;
        }
    }
    public Dictionary<string, string> tags { get; set; }
    public bool visible { get; set; }

    
    public GISnode(long id = 0, double latitude = 0.0, double longitude = 0.0)
    {
        this.id = id;
        latlon = new Vector2d(longitude, latitude);
        tags = new Dictionary<string, string>();
    }

    public GISnode()
    {

    }
}
