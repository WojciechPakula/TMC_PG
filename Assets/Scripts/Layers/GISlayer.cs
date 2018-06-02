using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GISlayer {
    private Dictionary<Vector3Int, Texture2D> segmentCache = new Dictionary<Vector3Int, Texture2D>();
    public abstract Texture2D renderSegment(int x, int y, int z);

    public Texture2D renderSegment(Vector3Int coords)
    {
        return renderSegment(coords.x, coords.y, coords.z);
    }
    public Texture2D getCacheSegment(int x, int y, int z)
    {
        Texture2D result = null;
        bool val = segmentCache.TryGetValue(new Vector3Int(x,y,z), out result);
        if (val == false) return null;
        return result;
    }
    public void setCacheSegment(int x, int y, int z, Texture2D tex)
    {
        segmentCache.Add(new Vector3Int(x,y,z),tex);
    }
    public void setCacheSegment(Vector3Int coords, Texture2D tex)
    {
        setCacheSegment(coords.x, coords.y, coords.z,tex);
    }
    public void clearCache()
    {
        segmentCache.Clear();
    }
    /*{        
        Texture2D result = new Texture2D(256,256);
        Color32 resetColor = new Color32(0, 0, 0, 0);
        Color32[] resetColorArray = result.GetPixels32();
        for (int i = 0; i < resetColorArray.Length; i++)       
            resetColorArray[i] = resetColor;       
        result.SetPixels32(resetColorArray);


        result.Apply();
        return result;
    }*/

    //web
    public static Vector2d LatLonToWeb(Vector2d latlon, int z = 0)
    {
        Vector2d result;
        latlon *= Mathd.PI / 180.0;
        result.y = -System.Math.Log(System.Math.Tan(Mathd.PI / 4.0 + latlon.x / 2.0)) + Mathd.PI;
        result.x = latlon.y + Mathd.PI;

        result = result * ((256 / (2 * Mathd.PI)) * Mathd.Pow(2, z));        
        return result;
    }
    public static Vector2d WebToLatLon(Vector2d web, int z = 0)
    {
        Vector2d result;
        web = web / ((256 / (2 * Mathd.PI)) * Mathd.Pow(2, z));
        result.y = 2 * System.Math.Atan(System.Math.Exp(-web.x + Mathd.PI)) - Mathd.PI / 2.0;
        result.x = web.y - Mathd.PI;

        result *= 180.0 / Mathd.PI;
        return result;
    }
    //Unitary
    public static Vector2d LatLonToUnitary(Vector2d latlon)
    {
        Vector2d result= LatLonToWeb(latlon, 0);
        result.x /= 256.0;
        result.y /= 256.0;
        return result;
    }
    public static Vector2d UnitaryToLatLon(Vector2d unitary)
    {
        Vector2d result = WebToLatLon(new Vector2d(unitary.x*256, unitary.y*256), 0);
        return result;
    }
    //segment
    public static Vector3Int unitaryToSegment(Vector2d unitary, int zoom)
    {
        Vector3Int result = new Vector3Int((int)(unitary.x*(double)(1 << zoom)), (int)(unitary.y* (double)(1 << zoom)), zoom);
        return result;
    }
    public static Vector2d segmentToUnitary(Vector3Int segment)
    {
        Vector2d result = new Vector2d((double)segment.x/(double)(1<<segment.z), (double)segment.y / (double)(1 << segment.z));
        return result;
    }
}
