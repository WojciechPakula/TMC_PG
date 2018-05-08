using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISnode
{
    public long id { get; set; }
    private Vector2d _latlon;
    private Vector2d _XY;

    public Vector2d latlon {
        get
        {
            return _latlon;
        }
        set
        {
            _latlon = value;
            //Vector2d result;
            //result.y = System.Math.Log(System.Math.Tan((_latlon.y + 90d) / 360d * System.Math.PI)) / System.Math.PI * 180d;
            //result.x = _latlon.x;

            _XY = GISparser.LatLonToWeb(_latlon,0);
        }
    }
    public Vector2d XY {
        get
        {
            return _XY;
        }
        set
        {
            _XY = value;
            //Vector2d result;
            //result.y = System.Math.Atan(System.Math.Exp(_XY.y / 180d * System.Math.PI)) / System.Math.PI * 360d - 90d;
            //result.x = _XY.x;

            _latlon = GISparser.WebToLatLon(_XY, 0);
        }
    }
    public Dictionary<string, string> tags { get; set; }
    public bool visible { get; set; }

    
    public GISnode(long id = 0, double latitude = 0.0, double longitude = 0.0)
    {
        this.id = id;
        latlon = new Vector2d(latitude, longitude);
        tags = new Dictionary<string, string>();
    }


}
