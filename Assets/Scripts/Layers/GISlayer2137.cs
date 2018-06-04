using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

public class GISlayer2137 : GISlayer
{
    public static Texture2D sys0 = null;
    public static Texture2D sys1 = null;

    static byte[] sys0b=null;
    static byte[] sys1b=null;

    static System.Random rand;

    public static void loadResources()
    {
        rand = new System.Random();
        sys0 = Resources.Load("sys/sys0", typeof(Texture2D)) as Texture2D;
        sys0.Apply();
        sys1 = Resources.Load("sys/sys1", typeof(Texture2D)) as Texture2D;
        sys1.Apply();
        //var res0 = Resources.Load("sys/sys0");
        //var res1 = Resources.Load("sys/sys1");

        sys0b = texToByte(sys0);
        sys1b = texToByte(sys1);
    }

    private static byte[] texToByte(Texture2D tex)
    {
        byte[] b = new byte[256*256*4];
        for (int y = 0; y < 256; ++y)
        {
            for (int x = 0; x < 256; ++x)
            {
                var c = tex.GetPixel(x,255-y);
                b[((x + y * 256) * 4) + 0] = (byte)(c.r * 255);
                b[((x + y * 256) * 4) + 1] = (byte)(c.g * 255);
                b[((x + y * 256) * 4) + 2] = (byte)(c.b * 255);
                b[((x + y * 256) * 4) + 3] = (byte)(c.a * 255);
            }
        }
        return b;
    }

    public override Texture2D renderSegment(int x, int y, int z)
    {
        throw new NotImplementedException();
    }

    public override byte[] renderSegmentThreadSafe(int x, int y, int z)
    {        
        var r = rand.Next(0,2);
        if (r == 0) return sys0b; else return sys1b;
    }
}
