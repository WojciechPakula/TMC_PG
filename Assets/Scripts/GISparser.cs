using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GISparser {
    public static GISdata LoadOSM(string path)
    {
        //coś
        return null;
    }

    //generator płaskiej ziemi
    public static Vector2 LatlonToXY(Vector2 latlon)
    {
        Vector2 result;
        result.y = (float)(System.Math.Log(System.Math.Tan((latlon.y + 90) / 360 * System.Math.PI)) / System.Math.PI * 180);
        result.x = latlon.x;
        return result;
    }
    public static Vector2 XYtoLatlon(Vector2 XY)
    {
        Vector2 result;       
        result.y = (float)(System.Math.Atan(System.Math.Exp((double)XY.y / 180 * System.Math.PI)) / System.Math.PI * 360 - 90);
        result.x = XY.x;
        return result;
    }
}
