using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISlayerTest : GISlayer
{
    public override Texture2D renderSegment(int x, int y, int z)
    {
        Texture2D tex = new Texture2D(256, 256);
        Color c = randomColor();
        for (int x0 = 0; x0 < tex.width; ++x0)
        {
            for (int y0 = 0; y0 < tex.height; ++y0)
            {
                if (Random.Range(0f, 1f)<0.1f) 
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
        Color c = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        return c;
    }
}
