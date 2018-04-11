using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GISparser {
    public static GISdata LoadOSM(string path)
    {
        //coś
        return null;
    }

    public static bool lineChecker(Vector2d l1, Vector2d l2, Vector2d p, float thickness)
    { 
        //cos
        return false;
    }

    public static bool fieldChecker(List<Vector2d> points, Vector2d p)
    {
        //cos
        return false;
    }

    //generator płaskiej ziemi
    public static Vector2d LatlonToXY(Vector2d latlon)
    {
        Vector2d result;
        result.y = System.Math.Log(System.Math.Tan((latlon.y + 90d) / 360d * System.Math.PI)) / System.Math.PI * 180d;
        result.x = latlon.x;
        result -= constantOffset;
        return result;
    }
    public static Vector2d XYtoLatlon(Vector2d XY)
    {
        Vector2d result;
        XY += constantOffset;
        result.y = System.Math.Atan(System.Math.Exp(XY.y / 180d * System.Math.PI)) / System.Math.PI * 360d - 90d;
        result.x = XY.x;
        return result;
    }
    public static double latlonToMeters(Vector2d p1, Vector2d p2)
    {  // generally used geo measurement function
        var R = 6378.137; // Radius of earth in KM
        var dLat = p2.y * Mathd.PI / 180 - p1.y * Mathd.PI / 180;
        var dLon = p2.x * Mathd.PI / 180 - p1.x * Mathd.PI / 180;
        var a = Mathd.Sin(dLat / 2) * Mathd.Sin(dLat / 2) +
        Mathd.Cos(p1.y * Mathd.PI / 180) * Mathd.Cos(p2.y * Mathd.PI / 180) *
        Mathd.Sin(dLon / 2) * Mathd.Sin(dLon / 2);
        var c = 2 * Mathd.Atan2(Mathd.Sqrt(a), Mathd.Sqrt(1 - a));
        var d = R * c;
        return d * 1000; // meters
    }

    private static Vector2d constantOffset = new Vector2d(0,0);

    public static void setOffset(double x, double y)
    {
        constantOffset.x = x;
        constantOffset.y = y;
    }


}
