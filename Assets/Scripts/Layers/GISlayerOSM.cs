using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class GISlayerOSM : GISlayer {
    Dictionary<Vector3Int, Texture2D> done = new Dictionary<Vector3Int, Texture2D>();

    override
    public Texture2D renderSegment(int x, int y, int z)
    {
        Texture2D tex = null;
        Vector3Int para = new Vector3Int(x,y,z);
        bool czyJest = done.TryGetValue(para, out tex);
        if (czyJest)
        {
            return tex;
        } else
        {
            string url = @"http://a.tile.openstreetmap.org/" + z + @"/" + x + @"/" + y + @".png";
            string name = "OSM." + z + "." + x + "." + y + ".png";
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
                        tex = new Texture2D(2, 2);
                        tex.LoadImage(fileData);
                    } else {
                        Debug.Log("Błąd sieci, nie można pobrać pliku png");
                    }
                }
            }
            tex = GISparser.flipYTexture(tex);
            tex.Apply();
            done.Add(para,tex);
            return tex;
        }
    }

    public override byte[] renderSegmentThreadSafe(int x, int y, int z)
    {
        throw new NotImplementedException();
    }
}
