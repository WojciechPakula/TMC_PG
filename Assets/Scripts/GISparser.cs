using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

public static class GISparser {
    public static GISdata LoadOSM(string path)
    {        
        GISdata loadedData = new GISdata();
        osm Osm = new osm();
        //najpierw ładujemy ten osm który zawiera wszystkie dane
        XmlSerializer serializer = new XmlSerializer(typeof(osm));
        StreamReader reader = new StreamReader(path);
        Osm = (osm)serializer.Deserialize(reader);
        reader.Close();

        osmNode[] osmNodes = Osm.node;
        osmWay[] osmWays = Osm.way;
        GISnode currentNode = null;
        GISway currentWay = null;
        double minLat = double.MaxValue;
        double maxLat = double.MinValue;
        double minLon = double.MaxValue;
        double maxLon = double.MinValue;

        //wczytujemy najpierw nody bo są potrzebne przy drogach
        foreach (osmNode node in osmNodes)
        {
            double currentLat = double.Parse(node.lat, CultureInfo.InvariantCulture);
            double currentLon = double.Parse(node.lon, CultureInfo.InvariantCulture);
            //aktualny node tworzony - id, współrzędne
            currentNode = new GISnode(Convert.ToInt64(node.id), currentLat, currentLon);
            //sprawdzamy czy ma tagi
            if (node.tag != null)
            {
                foreach (tag nodeTag in node.tag)
                {
                    //dodajemy każdy tag do słownika
                    currentNode.tags.Add(nodeTag.k, nodeTag.v);
                }
            }
            //ustawiamy visibility i dodajemy
            currentNode.visible = Convert.ToBoolean(node.visible);
            loadedData.nodeContainer.Add(currentNode);
            if (currentLat > maxLat) maxLat = currentLat;
            if (currentLon > maxLon) maxLon = currentLon;
            if (currentLat < minLat) minLat = currentLat;
            if (currentLon < minLon) minLon = currentLon;
        }

        foreach (osmWay way in osmWays)
        {
            //aktualny way tworzony - tylko id
            currentWay = new GISway(Convert.ToInt64(way.id));
            //sprawdzamy czy ma tagi
            if (way.tag != null)
            {
                foreach (tag wayTag in way.tag)
                {
                    //dodajemy tagi
                    currentWay.tags.Add(wayTag.k, wayTag.v);
                }
            }
            //przechodzimy się po wszystkich nodach waywa
            foreach (osmWayND wayNode in way.nd)
            {
                //szukamy w nodach już dodanych tego aktualnego (żeby była referencja) i dodajemy
                GISnode node = loadedData.nodeContainer.Find(i => i.id == Convert.ToInt64(wayNode.@ref));
                currentWay.localNodeContainer.Add(node);
            }
            //ustawiamy visibility i dodajemy
            currentWay.visible = Convert.ToBoolean(way.visible);
            loadedData.wayContainer.Add(currentWay);
        }
        loadedData.maxLat = maxLat;
        loadedData.maxLon = maxLon;
        loadedData.minLat = minLat;
        loadedData.minLon = minLon;
        return loadedData;
    }

    public static bool lineChecker(Vector2d l1, Vector2d l2, Vector2d p, float thickness)
    {
        double maxDistance = thickness / 2;

        // P jest w okręgu o promieniu maxDistance od l1
        double l1_distance = (p - l1).magnitude;
        if (l1_distance <= maxDistance)
        {
            return true;
        }

        // P jest w okręgu o promieniu maxDistance od l2
        double l2_distance = (p - l2).magnitude;
        if (l2_distance <= maxDistance)
        {
            return true;
        }

        // P znajduje się w pozostałym obszarze
        Vector2d xDirection = (l2 - l1).normalized;
        Vector2d yDirection = new Vector2d(-xDirection.y, xDirection.x);
        Vector2d point = p - l1;

        double pointProjectedX = Vector2d.Dot(point, xDirection);
        double pointProjectedY = Vector2d.Dot(point, yDirection);

        return pointProjectedX >= 0 && pointProjectedX <= (l2 - l1).magnitude &&
            pointProjectedY <= maxDistance && pointProjectedY >= -maxDistance;
    }

    public static bool fieldChecker(List<Vector2d> points, Vector2d p)
    {
        bool result = false;
        int j = points.Count - 1;
        for (int i = 0; i < points.Count(); i++)
        {
            //bardzo ciekawe sprawdzanie pozycji y
            if (points[i].y < p.y && points[j].y >= p.y || points[j].y < p.y && points[i].y >= p.y)
            {
                //rownie ciekawe sprawdzanie pozycji x
                if (points[i].x + (p.y - points[i].y) / (points[j].y - points[i].y) * (points[j].x - points[i].x) < p.x)
                {
                    result = !result;
                }
            }
            j = i;
        }
        return result;
    }

    //generator płaskiej ziemi
    /*
    //stare
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
    }*/
    //nowsze/poprawione zamienienie x i y
    public static Vector2d LatlonToXY(Vector2d latlon)
    {
        Vector2d result;
        result.x = System.Math.Log(System.Math.Tan((latlon.x + 90d) / 360d * System.Math.PI)) / System.Math.PI * 180d;
        result.y = latlon.y;
        result -= constantOffset;
        return result;
    }
    public static Vector2d XYtoLatlon(Vector2d XY)
    {
        Vector2d result;
        XY += constantOffset;
        result.x = System.Math.Atan(System.Math.Exp(XY.x / 180d * System.Math.PI)) / System.Math.PI * 360d - 90d;
        result.y = XY.y;
        return result;
    }
    //web
    public static Vector2d LatLonToWeb(Vector2d latlon,int z=0)
    {
        Vector2d result;
        latlon *= Mathd.PI/180.0;
        result.y = -System.Math.Log(System.Math.Tan(Mathd.PI/4.0+ latlon.x/2.0)) + Mathd.PI;        
        result.x = latlon.y + Mathd.PI;

        result = result * ((256 / (2 * Mathd.PI)) * Mathd.Pow(2,z));

        //result *= 180.0 / Mathd.PI;
        return result;
    }
    public static Vector2d WebToLatLon(Vector2d web,int z=0)
    {
        Vector2d result;
        //web *= Mathd.PI / 180.0;
        web = web / ((256 / (2 * Mathd.PI)) * Mathd.Pow(2, z));
        result.y = 2*System.Math.Atan(System.Math.Exp(-web.x + Mathd.PI)) - Mathd.PI/2.0;
        result.x = web.y - Mathd.PI;

        result *= 180.0 / Mathd.PI;
        return result;
    }

    public static List<byte> getQuadPath(Vector2Int coord, int z)
    {
        List<byte> list = new List<byte>();
        int x = coord.x;
        int y = coord.y;
        for (int i = 0; i < z; ++i)
        {
            bool bx = GetBit(x,i);
            bool by = GetBit(y,i);
            
            if (bx == false && by == false) { list.Add(0); continue; }
            if (bx == true && by == false) { list.Add(1); continue; }
            if (bx == false && by == true) { list.Add(2); continue; }
            if (bx == true && by == true) { list.Add(3); continue; }
        }
        list.Reverse();
        return list;
    }
    public static Vector2Int getQuadCoord(List<byte> list)
    {
        Vector2Int result = new Vector2Int(0,0);
        int z = -1;
        list.Reverse();
        foreach (var element in list)
        {
            ++z;
            if (element == 0) continue;
            if (element == 1) {result.x += (int)Mathf.Pow(2, z); continue; }
            if (element == 2) { result.y += (int)Mathf.Pow(2, z); continue; }
            if (element == 2) {result.x += (int)Mathf.Pow(2, z); result.y += (int)Mathf.Pow(2, z); continue; }             
        }
        return result;
    }
    private static bool GetBit(this int b, int bitNumber)
    {
        return (b & (1 << bitNumber)) != 0;
    }
    /*public static Vector2d LatlonToXY(Vector2d latlon)
    {
        Vector2d result;
        result.x = System.Math.Log(System.Math.Tan((latlon.x + 90d) / 360d * System.Math.PI)) / System.Math.PI * 180d;
        result.x *= 20037508.34 / 180;
        result.y = latlon.y * 20037508.34 / 180;
        result -= constantOffset;
        return result;
    }
    public static Vector2d XYtoLatlon(Vector2d XY)
    {
        Vector2d result;
        XY += constantOffset;
        result.x = System.Math.Atan(System.Math.Exp(XY.x / 180d * System.Math.PI)) / System.Math.PI * 360d - 90d;
        result.x *= 180 / 20037508.34;
        result.y = XY.y * 180 / 20037508.34;
        return result;
    }*/
    /*public static double latlonToMeters(Vector2d p1, Vector2d p2)
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
    }*/

    private static Vector2d constantOffset = new Vector2d(0,0);

    public static void setOffset(double x, double y)
    {
        constantOffset.x = x;
        constantOffset.y = y;
    }

    static Color c0 = new Color(0, 0, 0, 1);
    static Color c1 = new Color(0, 0, 1, 1);
    static Color c2 = new Color(0, 1, 1, 1);
    static Color c3 = new Color(0, 1, 0, 1);
    static Color c4 = new Color(1, 1, 0, 1);
    static Color c5 = new Color(1, 0, 0, 1);
    static Color c6 = new Color(1, 1, 1, 1);

    public static Color valueToHeatColor(float value)
    {
        Color c = new Color();

        if (value > 1) value = 1;
        if (value < 0) value = 0;
        value = value * 7;
        int seg = (int)value;
        value = value - seg;
        if (seg == 0) c = Color.Lerp(c0,c1, value);

        switch (seg)
        {
            case 0: c = Color.Lerp(c0, c1, value); break;
            case 1: c = Color.Lerp(c1, c2, value); break;
            case 2: c = Color.Lerp(c2, c3, value); break;
            case 3: c = Color.Lerp(c3, c4, value); break;
            case 4: c = Color.Lerp(c4, c5, value); break;
            case 5: c = Color.Lerp(c5, c6, value); break;
            case 6: c = c6; break;
            default: c = c0; break;
        }

        return c;
    }
}
