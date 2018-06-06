using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISlayerOSMXML : GISlayer {
    GISdata gisdata = null;
    GISquadtree qt = null;

    public override string getTypeName()
    {
        return "GISlayerOSMXML";
    }

    public override Texture2D renderSegment(int x, int y, int z)
    {
        throw new NotImplementedException();
    }

    public override byte[] renderSegmentThreadSafe(int x, int y, int z)
    {
        /*string url = @"http://a.tile.openstreetmap.org/" + z + @"/" + x + @"/" + y + @".png";
        string name = "OSM." + z + "." + x + "." + y + ".png";
        string filePath = GISparser.webImagesPath + name;
        byte[] fileData = null;
        using (WebClient client = new WebClient())
        {
            if (File.Exists(filePath))
            {
                fileData = GISlayer.osmPath(filePath);
            }
            else
            {
                try
                {
                    client.DownloadFile(new Uri(url), filePath);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                if (File.Exists(filePath))
                {
                    fileData = GISlayer.osmPath(filePath);
                }
                else
                {
                    Debug.Log("Błąd sieci, nie można pobrać pliku png");
                }
            }
        }
        return fileData;*/
        return generateTexture(x,y,z);
    }

    public void init(string path)
    {
        gisdata = GISparser.LoadOSM(path);
        qt = new GISquadtree(null);
        qt.size = new Vector2d(1, 1);
        qt.position = new Vector2d(0, 0);
        foreach (var way in gisdata.wayContainer)
        {
            qt.insert(way);
        }       
    }

    private byte[] generateTexture(int cx, int cy, int z)
    {
        byte[] tex = new byte[256*256*4];
        var path = GISparser.getQuadPath(new Vector2Int(cx, cy), z);
        var waysList = qt.getObjects(path);

        //return GISparser.downloadBINGpng(cx,cy,z);

        Vector2d chunkLow = new Vector2d(((double)(cx) / (double)(1 << z)), ((double)(cy) / (double)(1 << z)));
        Vector2d chunkHigh = new Vector2d(((double)(cx + 1) / (double)(1 << z)), ((double)(cy + 1) / (double)(1 << z)));

        var c = new Vector4(0f,0f,0f,0f);
        for (int x = 0; x < 256; ++x)
        {
            for (int y = 0; y < 256; ++y)
            {
                setPixel(tex, x, y, c);
            }
        }

        if (gisdata == null || qt == null) return tex;

        foreach (var way in waysList)
        {
            //lineChecker(Vector2d l1, Vector2d l2, Vector2d p, float thickness);
            //drawWay(ref tex, way, chunkLow, chunkHigh);
            drawWayOptimal(tex, way, chunkLow, chunkHigh);
        }

        return tex;
    }
    void drawWayOptimal(byte[] tex, GISway way, Vector2d chunkLow, Vector2d chunkHigh)
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
                drawLine(tex, p0, p1, new Vector4(1.0f,1.0f,1.0f,1.0f));


            }
            //koniec
            pprev = node;
        }
    }
    bool checkLineInBox(Vector2d p0, Vector2d p1, Vector2d bl, Vector2d bh)
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
    }
    //p1 i p2 wyznaczają prostą, p3 i p4 drugą prostą. Zwraca punkt przecięcia
    Vector2d intersection(Vector2d p1, Vector2d p2, Vector2d p3, Vector2d p4)
    {
        Vector2d result = new Vector2d();
        var mianownik = ((p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x));
        result.x = ((p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.x)) / mianownik;
        result.y = ((p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.x)) / mianownik;
        return result;
    }
    void drawLine(byte[] tex, Vector2Int p0, Vector2Int p1, Vector4 color)
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
    }

    void setPixel(byte[] tex, int x, int y, Vector4 color)
    {
        tex[(x + 256 * y) * 4 + 0] = (byte)(color.x * 255f);
        tex[(x + 256 * y) * 4 + 1] = (byte)(color.y * 255f);
        tex[(x + 256 * y) * 4 + 2] = (byte)(color.z * 255f);
        tex[(x + 256 * y) * 4 + 3] = (byte)(color.w * 255f);
    }

    //usun
    /*public static byte[] bingPath(string path)
    {
        //Debug.Log("GISlayer start: "+ path);
        Bitmap bmp = new Bitmap(path);
        //Debug.Log("GISlayer bitmap");
        Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        BitmapData bmpData =
           bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

        int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
        byte[] rgbValues = new byte[bytes];
        //Debug.Log("GISlayer copy");
        // Copy the RGB values into the array.
        Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);

        byte[] rgbaValues = new byte[bmp.Width * bmp.Height * 4];
        //Debug.Log("GISlayer mem");
        for (int x = 0; x < bmp.Width; x++)
        {
            for (int y = 0; y < bmp.Width; y++)
            {
                rgbaValues[(x + bmp.Width * y) * 4 + 0] = rgbValues[(x + bmp.Width * (y)) * 3 + 2];
                rgbaValues[(x + bmp.Width * y) * 4 + 1] = rgbValues[(x + bmp.Width * (y)) * 3 + 1];
                rgbaValues[(x + bmp.Width * y) * 4 + 2] = rgbValues[(x + bmp.Width * (y)) * 3 + 0];
                rgbaValues[(x + bmp.Width * y) * 4 + 3] = 255;
            }
        }
        /*byte[] argbTex= new byte[bmp.Width* bmp.Height* 4];
        for (int i = 0; i < rgbValues.Length; i+=3)
        {
            rgbTex[i];
        }
        //Debug.Log("GISlayer end");
        return rgbaValues;
    }*/
}
