using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISlayerTest : GISlayer
{
    static System.Random ran = new System.Random();

    public override string getTypeName()
    {
        return "GISlayerTest";
    }

    public override Texture2D renderSegment(int x, int y, int z)
    {
        Texture2D tex = new Texture2D(256, 256);
        Color c = randomColor();
        for (int x0 = 0; x0 < tex.width; ++x0)
        {
            for (int y0 = 0; y0 < tex.height; ++y0)
            {
                if (ran.NextDouble() < 0.1f) 
                    tex.SetPixel(x0, y0, randomColor());
                else
                    tex.SetPixel(x0, y0, c);
            }
        }
        tex.SetPixel(0, 0, Color.black);
        tex.SetPixel(1, 0, Color.black);
        tex.SetPixel(2, 0, Color.black);
        tex.filterMode = FilterMode.Point;
        return tex;
    }

    public static Color randomColor()
    {
        Color c = new Color((float)ran.NextDouble(), (float)ran.NextDouble(), (float)ran.NextDouble());
        return c;
    }

    public static byte[] randomColorThreadSafe()//RGBA
    {
        byte[] c = new byte[4];
        c[0] = (byte)ran.Next(0,256);
        c[1] = (byte)ran.Next(0, 256);
        c[2] = (byte)ran.Next(0, 256);
        c[3] = 255;
        return c;
    }

    public override byte[] renderSegmentThreadSafe(int x, int y, int z)
    {
        byte[] tex = new byte[256 * 256 * 4];
        var randomColor = randomColorThreadSafe();
        for (int x0 = 0; x0 < 256; ++x0)
        {
            for (int y0 = 0; y0 < 256; ++y0)
            {
                if (ran.NextDouble() < 0.1f)
                {
                    //tex.SetPixel(x0, y0, randomColor());
                    int index = (x0 + 256 * y0) * 4;
                    var c = randomColorThreadSafe();
                    tex[index + 0] = c[0];
                    tex[index + 1] = c[1];
                    tex[index + 2] = c[2];
                    tex[index + 3] = c[3];
                }                   
                else
                {
                    int index = (x0 + 256 * y0) * 4;
                    var c = randomColor;
                    tex[index + 0] = c[0];
                    tex[index + 1] = c[1];
                    tex[index + 2] = c[2];
                    tex[index + 3] = c[3];
                }                   
            }
        }
        return tex;
    }
}
