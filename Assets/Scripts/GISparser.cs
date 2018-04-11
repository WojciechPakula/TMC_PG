﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GISparser {
    public static GISdata LoadOSM(string path)
    {
        //coś
        return null;
    }

    

    //generator płaskiej ziemi
    public static Vector2d LatlonToXY(Vector2d latlon)
    {
        Vector2d result;
        result.y = System.Math.Log(System.Math.Tan((latlon.y + 90d) / 360d * System.Math.PI)) / System.Math.PI * 180d;
        result.x = latlon.x;
        return result;
    }
    public static Vector2d XYtoLatlon(Vector2d XY)
    {
        Vector2d result;       
        result.y = System.Math.Atan(System.Math.Exp(XY.y / 180d * System.Math.PI)) / System.Math.PI * 360d - 90d;
        result.x = XY.x;
        return result;
    }
}
