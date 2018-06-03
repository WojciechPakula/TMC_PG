using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

public class threadTest : MonoBehaviour {
    private static Semaphore _pool;
    int c = 0;
    SpriteRenderer rend;
    // Use this for initialization
    void Start () {
        rend = GetComponent<SpriteRenderer>();

        _pool = new Semaphore(1, 1);

        int x = 0;
        int y = 0;
        int z = 1;
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
                //fileData = File.ReadAllBytes(filePath);
                Bitmap original = new Bitmap(filePath);
                Bitmap clone = new Bitmap(original.Width, original.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                //var arr = GISlayer.ImageToByte2(clone);

                //var arr = GISlayer.getImageRasterBytes(clone, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                Texture2D tex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
                //Texture2D tex = new Texture2D(16, 16, TextureFormat.PVRTC_RGBA4, false);

                /*for (int xx = 0; xx < tex.width; ++xx)
                {
                    for (int yy = 0; yy < tex.height; ++yy)
                    {
                        tex.SetPixel(xx, yy, randomColor());
                    }
                }
                tex.SetPixel(0, 0, UnityEngine.Color.black);
                tex.SetPixel(1, 0, UnityEngine.Color.black);
                tex.SetPixel(2, 0, UnityEngine.Color.black);
                tex.filterMode = FilterMode.Point;*/
                var arr2 = GISlayer.bingPath(filePath);
                tex.LoadRawTextureData(arr2);
                tex.filterMode = FilterMode.Point;
                tex.Apply();
                /*fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);*/

                Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
                rend.sprite = newSprite;
                Debug.Log("obrazek");
                //tex.LoadImage(fileData);
            }
            else
            {               
                Debug.Log("Błąd sieci, nie można pobrać pliku png");
            }
        }
    }
    public static UnityEngine.Color randomColor()
    {
        UnityEngine.Color c = new UnityEngine.Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        return c;
    }

    // Update is called once per frame
    void Update () {       
        _pool.WaitOne();
        Debug.Log("Semaphore "+(c++));
        _pool.Release();
    }
}
