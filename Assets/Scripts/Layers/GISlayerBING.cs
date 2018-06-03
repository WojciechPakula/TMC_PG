using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Drawing;

public class GISlayerBING : GISlayer {
    Dictionary<Vector3Int, Texture2D> done = new Dictionary<Vector3Int, Texture2D>();

    override
    public Texture2D renderSegment(int x, int y, int z)
    {
        Texture2D tex = null;
        Vector3Int para = new Vector3Int(x, y, z);
        bool czyJest = done.TryGetValue(para, out tex);
        if (czyJest)
        {
            return tex;
        }
        else
        {
            var qpath = GISparser.getQuadPath(x, y, z);
            string qpaths = "";
            foreach (var ele in qpath)
            {
                qpaths += ele.ToString();
            }
            string url = @"http://t.ssl.ak.dynamic.tiles.virtualearth.net/comp/ch/" + qpaths + @"?mkt=pl-PL&it=A,G,RL&shading=hill&n=z&og=268&c4w=1";
            string name = "BING." + z + "." + x + "." + y + ".png";
            string filePath = GISparser.webImagesPath + name;
            byte[] fileData;
            using (WebClient client = new WebClient())
            {
                if (File.Exists(filePath))
                {
                    fileData = File.ReadAllBytes(filePath);
                    tex = new Texture2D(2, 2);
                    tex.LoadImage(fileData);
                }
                else
                {
                    client.DownloadFile(new Uri(url), filePath);
                    if (File.Exists(filePath))
                    {
                        fileData = File.ReadAllBytes(filePath);
                        tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                        tex.LoadImage(fileData);
                    }
                    else
                    {
                        Debug.Log("Błąd sieci, nie można pobrać pliku png");
                    }
                }
            }
            tex = GISparser.flipYTexture(tex);
            tex.Apply();
            done.Add(para, tex);
            return tex;
        }
    }
    public override byte[] renderSegmentThreadSafe(int x, int y, int z)
    {
        var qpath = GISparser.getQuadPath(x, y, z);
        string qpaths = "";
        foreach (var ele in qpath)
        {
            qpaths += ele.ToString();
        }
        string url = @"http://t.ssl.ak.dynamic.tiles.virtualearth.net/comp/ch/" + qpaths + @"?mkt=pl-PL&it=A,G,RL&shading=hill&n=z&og=268&c4w=1";
        string name = "BING." + z + "." + x + "." + y + ".png";
        string filePath = GISparser.webImagesPath + name;
        byte[] fileData = null;
        using (WebClient client = new WebClient())
        {
            if (File.Exists(filePath))
            {
                fileData = GISlayer.bingPath(filePath);
            }
            else
            {
                client.DownloadFile(new Uri(url), filePath);
                if (File.Exists(filePath))
                {
                    fileData = GISlayer.bingPath(filePath);
                }
                else
                {
                    Debug.Log("Błąd sieci, nie można pobrać pliku png");
                }
            }
        }
        return fileData;
    }
}
