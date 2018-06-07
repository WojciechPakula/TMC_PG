using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISlayerHeatMap : GISlayer {

    GISdata gisdata = null;
    List<GISnode> uzytecznePunkty;
    public Dictionary<string, string> dostepneNazwy;

    private Dictionary<Vector3Int, float[]> heatCache = new Dictionary<Vector3Int, float[]>();

    float minvalue = float.PositiveInfinity;
    float maxvalue = float.NegativeInfinity;

    List<keyValue> zadania = new List<keyValue>();

    public bool isHeatActivated = false;

    public override string getTypeName()
    {
        return "GISlayerHeatMap";
    }

    public override Texture2D renderSegment(int x, int y, int z)
    {
        throw new NotImplementedException();
    }

    public override byte[] renderSegmentThreadSafe(int x, int y, int z)
    {
        return generateTexture(x, y, z);
    }

    public void setHeatActivated()
    {
        isHeatActivated = true;
        heatCache.Clear();
        map.clearSegmentCacheThreadSafe();
        minvalue = float.PositiveInfinity;
        maxvalue = float.NegativeInfinity;
    }

    public void setHeatDeactivated()
    {
        isHeatActivated = false;
    }

    public void addAbility(string value, int m)
    {
        keyValue kv = new keyValue("amenity", value);
        if (m == 0) kv.isMax = true; else kv.isMax = false;
        zadania.Add(kv);
        string oooo = kv.isMax ? "MAX" : "MIN";
        heatCache.Clear();
        minvalue = float.PositiveInfinity;
        maxvalue = float.NegativeInfinity;
        //wyswietlaczZadan.text += key + " " + value + " " + oooo + "\n";   //jakos to zastapic
    }

    public float[] renderFloat(int cx, int cy, int z)
    {
        float[] tryHeat = null;
        heatCache.TryGetValue(new Vector3Int(cx, cy, z), out tryHeat);        

        if (tryHeat != null) return tryHeat;

        if (isHeatActivated == false)
        {
            return null;
        }

        Vector2d chunkLow = new Vector2d(((double)(cx) / (double)(1 << z)), ((double)(cy) / (double)(1 << z)));
        Vector2d chunkHigh = new Vector2d(((double)(cx + 1) / (double)(1 << z)), ((double)(cy + 1) / (double)(1 << z)));

        float[] heatmap = new float[256*256];

        //float[,] heatmap = new float[tex.width, tex.height];
        foreach (var zad in zadania)
        {
            float[] tmpheat=null;
            if (zad.isMax) tmpheat = heatmapCreatorMax(chunkLow, chunkHigh, zad.key, zad.value);
            else tmpheat = heatmapCreatorMin(chunkLow, chunkHigh, zad.key, zad.value);
            for (int x = 0; x < 256; ++x)
            {
                for (int y = 0; y < 256; ++y)
                {
                    heatmap[x + 256 * y] += tmpheat[x + 256 * y];
                }
            }
        }
        //minmax
        for (int x = 0; x < 256; ++x)
        {
            for (int y = 0; y < 256; ++y)
            {
                var v = heatmap[x + 256*y];
                if (v < minvalue) minvalue = v;
                if (v > maxvalue) maxvalue = v;
            }
        }
        heatCache.Add(new Vector3Int(cx, cy, z), heatmap);
        map.clearSegmentCacheThreadSafe();
        this.clearCache();
        return heatmap;
    }

    float[] heatmapCreatorMax(Vector2d p1, Vector2d p2, string key, string value)
    {
        //node.tags[key] == value
        float[] result = new float[256*256];
        Vector2d ddd;
        ddd.x = (p2.x - p1.x) / (p2.x - p1.x);
        ddd.y = (p2.y - p1.y) / (p2.y - p1.y);

        List<GISnode> matched = new List<GISnode>();
        foreach (var node in uzytecznePunkty)
        {
            try
            {
                if (node.tags[key] == value)
                {
                    matched.Add(node);
                }
            }
            catch
            {

            }
        }
        for (int x = 0; x < 256; ++x)
        {
            for (int y = 0; y < 256; ++y)
            {
                result[x + 256 * y] = float.PositiveInfinity;
                foreach (var node in matched)
                {
                    Vector2d web = new Vector2d(((double)x / 256.0) * (p2.x - p1.x) + p1.x, ((double)y / 256.0) * (p2.y - p1.y) + p1.y);
                    var diff = node.XY - web;
                    double metry = diff.magnitude * 40075000.0 / 256.0;
                    if (result[x + 256 * y] > metry) result[x + 256 * y] = (float)metry;
                }
            }
        }
        return result;
    }
    float[] heatmapCreatorMin(Vector2d p1, Vector2d p2, string key, string value)
    {
        //node.tags[key] == value
        float[] result = new float[256 * 256];
        Vector2d ddd;
        ddd.x = (p2.x - p1.x) / (p2.x - p1.x);
        ddd.y = (p2.y - p1.y) / (p2.y - p1.y);

        List<GISnode> matched = new List<GISnode>();
        foreach (var node in uzytecznePunkty)
        {
            try
            {
                if (node.tags[key] == value)
                {
                    matched.Add(node);
                }
            }
            catch
            {

            }
        }
        for (int x = 0; x < 256; ++x)
        {
            for (int y = 0; y < 256; ++y)
            {
                result[x + 256 * y] = float.PositiveInfinity;
                foreach (var node in matched)
                {
                    Vector2d web = new Vector2d(((double)x / 256.0) * (p2.x - p1.x) + p1.x, ((double)y / 256.0) * (p2.y - p1.y) + p1.y);
                    var diff = node.XY - web;
                    double metry = diff.magnitude * 40075000.0 / 256.0;
                    if (result[x + 256 * y] > metry) result[x + 256 * y] = (float)metry;
                }
                result[x + 256 * y] *= -1;
            }
        }
        return result;
    }   

    public void init(string path)
    {
        gisdata = GISparser.LoadOSM(path);
        this.clearCache();
        map.clearSegmentCache();
        uzytecznePunkty = new List<GISnode>();
        dostepneNazwy = new Dictionary<string, string>();
        foreach (var node in gisdata.nodeContainer)
        {
            string output = null;
            node.tags.TryGetValue("amenity", out output);
            if (output != null)
            {
                uzytecznePunkty.Add(node);
                try
                {
                    dostepneNazwy.Add(output, output);
                } catch
                {

                }
            }
        }
    }

    private byte[] generateTexture(int cx, int cy, int z)
    {
        byte[] tex = new byte[256 * 256 * 4];
        //var path = GISparser.getQuadPath(new Vector2Int(cx, cy), z);
        //List<GISway> waysList = null;

        Vector2d chunkLow = new Vector2d(((double)(cx) / (double)(1 << z)), ((double)(cy) / (double)(1 << z)));
        Vector2d chunkHigh = new Vector2d(((double)(cx + 1) / (double)(1 << z)), ((double)(cy + 1) / (double)(1 << z)));

        var c = new Vector4(0f, 0f, 0f, 0f);
        for (int x = 0; x < 256; ++x)
        {
            for (int y = 0; y < 256; ++y)
            {
                setPixel(tex, x, y, c);
            }
        }

        if (gisdata == null) return tex;

        float[] heatMap = renderFloat(cx,cy,z);

        if (heatMap != null)
        {
            for (int x = 0; x < 256; ++x)
            {
                for (int y = 0; y < 256; ++y)
                {
                    var v = (heatMap[x + 256* y] - minvalue) / (maxvalue - minvalue);
                    var hc = GISparser.valueToHeatColor(v);
                    setPixel(tex,x, y, hc);
                }
            }
        }

        foreach (var punkt in uzytecznePunkty)
        {
            drawPointOptimal(tex, punkt, chunkLow, chunkHigh);
        }

        /*foreach (var way in waysList)
        {
            drawWayOptimal(tex, way, chunkLow, chunkHigh);
        }*/

        return tex;
    }

    void drawPointOptimal(byte[] tex, GISnode node, Vector2d chunkLow, Vector2d chunkHigh)
    {
        Vector2d a1 = node.XY;
        var p1 = new Vector2Int();
        p1.x = (int)((node.XY.x - chunkLow.x) / (chunkHigh.x - chunkLow.x) * 256);
        p1.y = (int)((node.XY.y - chunkLow.y) / (chunkHigh.y - chunkLow.y) * 256);
        drawPoint(tex, p1, new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
        //koniec    
    }

    void drawPoint(byte[] tex, Vector2Int p1, Vector4 color)
    {
        setPixel(tex, p1.x, p1.y, color);

        setPixel(tex, p1.x+1, p1.y, color);
        setPixel(tex, p1.x, p1.y+1, color);
        setPixel(tex, p1.x-1, p1.y, color);
        setPixel(tex, p1.x, p1.y-1, color);
        setPixel(tex, p1.x-1, p1.y-1, color);
        setPixel(tex, p1.x+1, p1.y+1, color);
        setPixel(tex, p1.x+1, p1.y-1, color);
        setPixel(tex, p1.x-1, p1.y+1, color);
    }

    public string updateStatus(Vector2d cur)
    {
        string stat = ".";
        if (uzytecznePunkty == null) return stat;
        double min = double.PositiveInfinity;        
        foreach (var el in uzytecznePunkty)
        {
            double dist = (cur-el.XY).magnitude;
            if (min > dist) { el.tags.TryGetValue("amenity", out stat); min = dist; }
        }
        return stat;
    }

    /*void drawWayOptimal(byte[] tex, GISway way, Vector2d chunkLow, Vector2d chunkHigh)
    {
        GISnode pprev = null;
        foreach (var node in way.localNodeContainer)
        {
            if (pprev == null) { pprev = node; continue; }
            bool czyRysowac = checkLineInBox(pprev.XY, node.XY, chunkLow, chunkHigh);
            if (czyRysowac)
            {
                Vector2d a0 = pprev.XY;
                Vector2d a1 = node.XY;
                var p0 = new Vector2Int();
                p0.x = (int)((pprev.XY.x - chunkLow.x) / (chunkHigh.x - chunkLow.x) * 256);
                p0.y = (int)((pprev.XY.y - chunkLow.y) / (chunkHigh.y - chunkLow.y) * 256);
                var p1 = new Vector2Int();
                p1.x = (int)((node.XY.x - chunkLow.x) / (chunkHigh.x - chunkLow.x) * 256);
                p1.y = (int)((node.XY.y - chunkLow.y) / (chunkHigh.y - chunkLow.y) * 256);
                drawLine(tex, p0, p1, new Vector4(1.0f, 1.0f, 1.0f, 1.0f));


            }
            //koniec
            pprev = node;
        }
    }*/
    /*bool checkLineInBox(Vector2d p0, Vector2d p1, Vector2d bl, Vector2d bh)
    {
        if (p0.x < bl.x && p1.x < bl.x) return false;
        if (p0.y < bl.y && p1.y < bl.y) return false;
        if (p0.x > bh.x && p1.x > bh.x) return false;
        if (p0.y > bh.y && p1.y > bh.y) return false;
        //możliwe przecięcie
        //duża złożoność!!!
        Vector2d blu = new Vector2d(bl.x, bh.y);
        Vector2d bhd = new Vector2d(bh.x, bl.y);

        var lewy = intersection(p0, p1, bl, blu); if (lewy.y > bl.y && lewy.y < blu.y) return true;
        var gorny = intersection(p0, p1, blu, bh); if (gorny.x > blu.x && gorny.x < bh.x) return true;
        var prawy = intersection(p0, p1, bh, bhd); if (prawy.y > bhd.y && prawy.y < bh.y) return true;
        var dolny = intersection(p0, p1, bhd, bh); if (dolny.x > bl.x && dolny.x < bhd.x) return true;
        return false;
    }*/
    //p1 i p2 wyznaczają prostą, p3 i p4 drugą prostą. Zwraca punkt przecięcia
    /*Vector2d intersection(Vector2d p1, Vector2d p2, Vector2d p3, Vector2d p4)
    {
        Vector2d result = new Vector2d();
        var mianownik = ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));
        result.x = ((p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.x)) / mianownik;
        result.y = ((p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.x)) / mianownik;
        return result;
    }*/
    /*void drawLine(byte[] tex, Vector2Int p0, Vector2Int p1, Vector4 color)
    {
        int x = p0.x, y = p0.y, x2 = p1.x, y2 = p1.y;

        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            if (!(x < 0 || y < 0 || x > 255 || y > 255))
                setPixel(tex, x, y, color);
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
    }*/

    void setPixel(byte[] tex, int x, int y, Vector4 color)
    {
        if (x < 0 || y < 0 || x > 255 || y > 255) return;
        tex[(x + 256 * y) * 4 + 0] = (byte)(color.x * 255f);
        tex[(x + 256 * y) * 4 + 1] = (byte)(color.y * 255f);
        tex[(x + 256 * y) * 4 + 2] = (byte)(color.z * 255f);
        tex[(x + 256 * y) * 4 + 3] = (byte)(color.w * 255f);
    }
}