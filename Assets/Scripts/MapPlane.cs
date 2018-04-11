using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlane : MonoBehaviour {
    
    public Vector2Int resolution;   
    public Vector2 wsp;

    SpriteRenderer rend;
    Texture2D tex;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start () {
        //rend = GetComponent<SpriteRenderer>();
        

        /*tex = new Texture2D(resolution.x, resolution.y);

        for (int x = 0; x < tex.width; ++x)
        {
            for (int y = 0; y < tex.height; ++y)
            {
                tex.SetPixel(x, y, randomColor());
            }
        }
        tex.SetPixel(0, 0, Color.black);
        tex.SetPixel(1, 0, Color.black);
        tex.SetPixel(0, 1, Color.black);
        tex.SetPixel(1, 1, Color.black);
        tex.SetPixel(2, 0, Color.black);
        tex.SetPixel(3, 0, Color.black);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

        rend.sprite = newSprite;*/
    }

    Vector2d maxL;
    Vector2d minL;

    public void fillHeatMap(Vector2d min, Vector2d max, GISdata data, string key, string value)
    {
        rend = GetComponent<SpriteRenderer>();
        tex = new Texture2D(resolution.x, resolution.y);

        var maxD = GISparser.LatlonToXY(new Vector2d(data.maxLat, data.maxLon));
        var minD = GISparser.LatlonToXY(new Vector2d(data.minLat, data.minLon));

        maxL = max;
        minL = min;

        float[,] heatmap = new float[resolution.x, resolution.y];
        float maxOdl = 255;
        float minOdl = 10000;

        foreach (var node in data.nodeContainer)
        {
            try
            {
                if (node.tags[key] == value)
                {
                    minOdl = 10000;
                    Vector2Int pos = XYtoPixel(GISparser.LatlonToXY(node.latlon));
                    float[,] heatmapLocal = new float[resolution.x, resolution.y];
                    //heatmapLocal[pos.x, pos.y] = 255;
                    for (int y = 0; y < resolution.y; ++y)
                    {
                        for (int x = 0; x < resolution.x; ++x)
                        {
                            float odl = 1000-Mathf.Sqrt((pos.x - x) * (pos.x - x) + (pos.y - y) * (pos.y - y));
                            //odl = Mathf.Sqrt(odl);
                            if (odl > maxOdl) maxOdl = odl;                            
                            heatmapLocal[x, y] = odl;                              
                        }
                    }
                    for (int y = 0; y < resolution.y; ++y)
                    {
                        for (int x = 0; x < resolution.x; ++x)
                        {
                            if (heatmapLocal[x, y] > heatmap[x, y]) heatmap[x, y] = heatmapLocal[x, y];
                            if (heatmap[x, y] < minOdl) minOdl = heatmap[x, y];
                        }
                    }
                }
            }
            catch
            {

            }
        }
        for (int y = 0; y < resolution.y; ++y)
        {
            for (int x = 0; x < resolution.x; ++x)
            {
                heatmap[x, y] = (heatmap[x, y]- minOdl) /(maxOdl- minOdl);
                heatmap[x, y] = 1 - heatmap[x, y];
                Color c = new Color(heatmap[x, y], 0, heatmap[x, y]);
                tex.SetPixel(x, y, c);
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

        rend.sprite = newSprite;
    }

    public void fillTexture(Vector2d min, Vector2d max, GISdata data)
    {
        rend = GetComponent<SpriteRenderer>();
        tex = new Texture2D(resolution.x, resolution.y);

        var maxD = GISparser.LatlonToXY(new Vector2d(data.maxLat, data.maxLon));
        var minD = GISparser.LatlonToXY(new Vector2d(data.minLat, data.minLon));

        maxL = max;
        minL = min;

        /*if (maxD.x < max.y) return;
        if (maxD.y < max.y) return;

        if (minD.x > min.y) return;
        if (minD.y > min.y) return;*/

        /*for (int y = 0; y < resolution.y; ++y)
        {
            for (int x = 0; x < resolution.x; ++x)
            {
                


                tex.SetPixel(x, y, randomColor());
            }
        }*/

        foreach (var way in data.wayContainer)
        {
            Color color = Color.black;
            Vector2d pos0 = Vector2d.zero;
            bool first = true;

            try
            {
                if (way.tags["highway"] == "residential") color = Color.red;
                if (way.tags["highway"] == "living_street") color = Color.red;
                if (way.tags["highway"] == "primary") color = Color.red;
                if (way.tags["highway"] == "secondary") color = Color.red;
                if (way.tags["highway"] == "tertiary") color = Color.red;
                if (way.tags["highway"] == "service") color = Color.red;
                if (way.tags["highway"] == "track") color = Color.red;
                if (way.tags["highway"] == "motorway") color = Color.red;
                if (way.tags["highway"] == "road") color = Color.red;
                if (way.tags["highway"] == "path") color = Color.red;
                if (way.tags["highway"] == "motorway") color = Color.red;
                if (way.tags["highway"] == "motorway") color = Color.red;
                if (way.tags["route"] == "road") color = Color.red;
            }
            catch
            {

            }
            

            foreach (var node in way.localNodeContainer)
            {               
                var pos1 = GISparser.LatlonToXY(node.latlon);
                if (first) {
                    first = false;
                    pos0 = pos1;
                    continue;
                }

                var pixel0 = XYtoPixel(pos0);
                var pixel1 = XYtoPixel(pos1);

                double thickness = 0.00001;

                double pixelSize = ((0.5) / (double)(resolution.y)) * (maxL.y - minL.y);

                int pthick = (int)(pixelSize / thickness);


                int pixminx = 0;int pixminy = 0;
                int pixmaxx = 0; int pixmaxy = 0;
                if (pixel0.x < pixel1.x)
                {
                    pixminx = pixel0.x - pthick;
                    pixmaxx = pixel1.x + pthick;
                } else
                {
                    pixminx = pixel1.x - pthick;
                    pixmaxx = pixel0.x + pthick;
                }

                if (pixel0.y < pixel1.y)
                {
                    pixminy = pixel0.y - pthick;
                    pixmaxy = pixel1.y + pthick;
                }
                else
                {
                    pixminy = pixel1.y - pthick;
                    pixmaxy = pixel0.y + pthick;
                }

                if (pixminx < 0) pixminx = 0;
                if (pixminy < 0) pixminy = 0;               

                if (pixmaxx >= resolution.x) pixmaxx = resolution.x;
                if (pixmaxy >= resolution.y) pixmaxy = resolution.y;

                for (int y = pixminy; y <= pixmaxy; ++y)
                {
                    for (int x = pixminx; x < pixmaxx; ++x)
                    {
                        bool check = GISparser.lineChecker(pos0, pos1, pixelToXY(new Vector2Int(x,y)), (float)thickness);
                        if (check) tex.SetPixel(x, y, color);
                    }
                }
                pos0 = pos1;
            }
        }

        tex.filterMode = FilterMode.Point;
        tex.Apply();

        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

        rend.sprite = newSprite;
    }

    public Vector2Int XYtoPixel(Vector2d p)
    {
        double x = (((p.x - minL.x) / (maxL.x - minL.x)) * (double)resolution.x);
        double y = (((p.y - minL.y) / (maxL.y - minL.y)) * (double)resolution.y);
        return new Vector2Int((int)x, (int)y);
    }

    public Vector2d pixelToXY(Vector2Int p)
    {
        return new Vector2d(
            ((((double)p.x+0.5)/(double)(resolution.x)) * (maxL.x - minL.x)+ minL.x),
            ((((double)p.y + 0.5) / (double)(resolution.y)) * (maxL.y - minL.y) + minL.y)
            );
    }

    Color randomColor()
    {
        Color c = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        return c;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
