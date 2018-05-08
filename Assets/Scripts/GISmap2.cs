using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;

public class GISmap2 : MonoBehaviour {
    GISdata gisdata;

    public int zoom;
    private int oldzoom;

    public Camera cam;

    public GameObject test1;
    public GameObject test2;

    int layerId = 0;

    //punkty wyznaczają prostokąt który zawiera widoczne chunki
    Vector2Int ch1;
    Vector2Int ch2;

    /*private void ExecuteInForeground(Thread main)
    {
        while(true)
        {
            try
            {
                if (main.ThreadState == ThreadState.AbortRequested) { Debug.Log("AbortRequested - Aplikacja zamknieta"); return; }
                
                Vector3Int element;
                try
                {
                    element = toProcessChunks[0];
                } catch
                {
                    continue;
                }
                toProcessChunks.RemoveAt(0);
                //_pool.Release();
                if (element.x >= ch1.x && element.y >= ch1.y && element.x < ch2.x && element.y < ch2.y)
                {
                    int size = (1 << zoom);
                    double length = 256.0 / (double)size;
                    var tex = generateTexture(element.x, element.y,zoom);
                    //_pool.WaitOne();
                    toAddChunks.Add(new textureContainer(element,tex));
                    //_pool.Release();
                }
            } catch
            {           
            }
        }
    }*/

    private Texture2D generateTexture(int cx, int cy, int z)
    {
        Texture2D tex = new Texture2D(256, 256);

        for (int x = 0; x < 256; ++x)
        {
            for (int y = 0; y < 256; ++y)
            {
                tex.SetPixel(x, y, BitMapTest.randomColor());
            }
        }
        return tex;
    }

    Thread thread;
    // Use this for initialization
    void Start () {
        //thread = new Thread(() => ExecuteInForeground(Thread.CurrentThread));
        //thread.Start();
        //loadFile("G:\\POLITECHNIKA\\PROJEKTY\\#8 Technologie map cyfrowych\\maly.osm");
        //setPlane(new Vector2d(0,0),2);
        //fillMap();
        //cam.orthographicSize = 256.0f / Mathf.Pow(2, zoom) * ((float)cam.pixelHeight/256);
        //cam.orthographicSize = (float)cam.pixelHeight / Mathf.Pow(2, zoom)/2;
        //Debug.Log(worldPosToChunk(new Vector2d(200,200)));
        //getChunkCoordsList();
        oldzoom = zoom;
    }

    private void OnDestroy()
    {
        try
        {
            thread.Abort();
        } catch
        {

        }        
    }

    List<chunkContainer> doneChunks = new List<chunkContainer>();


    //List<Vector3Int> toProcessChunks = new List<Vector3Int>();
    //List<textureContainer> toAddChunks = new List<textureContainer>();

    // Update is called once per frame
    void Update () {
        Vector2d range = getCameraRange();
        test1.transform.position = new Vector3(-(float)range.x/2f + cam.transform.position.x, 0, -(float)range.y/2f+ cam.transform.position.z);
        test2.transform.position = new Vector3((float)range.x / 2f+ cam.transform.position.x, 0, (float)range.y / 2f+ cam.transform.position.z);
        cam.orthographicSize = (float)cam.pixelHeight / Mathf.Pow(2, zoom) / 2.0f;
        updateChunks();
        if (oldzoom != zoom) {  //zoom uległ zmianie
            if (zoom > 16) zoom = oldzoom; else
            if (zoom < 0) zoom = oldzoom; else
            {
                oldzoom = zoom;
                layerId++;
            }
        }
    }

    /*void updateChunks()
    {
        var requestedChunks = getChunkCoordsList();

        int size = (1 << zoom);
        double length = 256.0 / (double)size;

        foreach (var ele in requestedChunks)
        {
            //if (!doneChunks.Exists(x => x.pos == ele))
            var chunk = doneChunks.Find(x => x.pos == ele);
            if (chunk == null)
            {
                var go = setPlane(new Vector2d(ele.x * length, -ele.y * length), length);//tworzy chunki tylko gdy nie istnieją
                var spr = go.GetComponent<SpriteRenderer>();
                doneChunks.Add(new chunkContainer(ele, spr));
            } else
            {
                chunk.sr.sortingOrder = layerId;
            }
        }
    }*/

    void updateChunks()
    {
        var requestedChunks = getChunkCoordsList();

        int size = (1 << zoom);
        double length = 256.0 / (double)size;

        /*foreach (var ele in toAddChunks)
        {
            var go = setPlane(new Vector2d(ele.pos.x * length, -ele.pos.y * length), length);//tworzy chunki tylko gdy nie istnieją
            var bmt = go.GetComponent<BitMapTest>();
            bmt.setTexture(ele.tex);
            
        }*/
        foreach (var ele in requestedChunks)
        {
            //if (!doneChunks.Exists(x => x.pos == ele))
            var chunk = doneChunks.Find(x => x.pos == ele);
            //bool process = toProcessChunks.Exists(x => x == ele);
            if (chunk == null)
            {
                var go = setPlane(new Vector2d(ele.x * length, -ele.y * length), length);//tworzy chunki tylko gdy nie istnieją
                var spr = go.GetComponent<SpriteRenderer>();
                var com = go.GetComponent<BitMapTest>();
                com.setTexture(generateTexture(ele.x, -ele.y, zoom));
                //toProcessChunks.Add(ele);
                doneChunks.Add(new chunkContainer(ele, spr));
            }
            else if(chunk != null)
            {
                chunk.sr.sortingOrder = layerId;
            }
        }
    }

    public void zoomin()
    {
        zoom++;
    }

    public void zoomout()
    {
        zoom--;
    }

    List<Vector3Int> getChunkCoordsList()
    {
        List<Vector3Int> result = new List<Vector3Int>();
        Vector2d range = getCameraRange();
        Vector2d p1 = new Vector2d(-(float)range.x / 2f + cam.transform.position.x, -(float)range.y / 2f + -cam.transform.position.z);
        Vector2d p2 = new Vector2d((float)range.x / 2f + cam.transform.position.x, (float)range.y / 2f + -cam.transform.position.z);

        ch1 = worldPosToChunk(p1);
        ch2 = worldPosToChunk(p2);

        if (ch1.x < 0) ch1.x = 0;
        if (ch1.y < 0) ch1.y = 0;

        for (int y = ch1.y; y <= ch2.y && y < 1 << zoom; ++y)
        {
            for (int x = ch1.x; x <= ch2.x && x < 1 << zoom; ++x)
            {
                result.Add(new Vector3Int(x,y,zoom));
            }
        }

        return result;
    }

    Vector2Int worldPosToChunk(Vector2d pos)
    {
        pos *= (1 << zoom) / 256.0f;
        return new Vector2Int((int)pos.x, (int)pos.y);
    }

    private Vector2d getCameraRange()
    {
        float c = (float)cam.pixelWidth/ (float)cam.pixelHeight;
        double size = cam.orthographicSize;
        Vector2d result = new Vector2d(2*size*c, 2 * size);
        return result;
    }

    private void Awake()
    {
        cam.orthographic = true;
    }

    public void loadFile(string text)
    {
        gisdata = GISparser.LoadOSM(text);
    }

    public GameObject setPlane(Vector2d pos, double length)
    {
        double size = length / 2.56;
        var tmp = Resources.Load("Prefabs/testPlane", typeof(GameObject));
        GameObject go = (GameObject)Instantiate(tmp);
        var bt = go.GetComponent<BitMapTest>();
        var spr = go.GetComponent<SpriteRenderer>();
        spr.sortingOrder = layerId;
        go.transform.position = new Vector3((float)pos.x, 0, (float)pos.y);
        go.transform.localScale = new Vector3((float)size, (float)size, 0);
        return go;
    }

    //debug
    /*public void fillMap()
    {
        int size = (1 << zoom);
        double length = 256.0 / (double)size;
        for (int y = 0; y < size && y<5; ++y)
        {
            for (int x = 0; x < size && x < 5; ++x)
            {
                setPlane(new Vector2d(x*length,-y*length), length);
            }
        }
    }*/
}

public class chunkContainer
{
    public Vector3Int pos;
    public SpriteRenderer sr;

    public chunkContainer(Vector3Int pos, SpriteRenderer sr)
    {
        this.pos = pos;
        this.sr = sr;
    }
}

/*public class textureContainer
{
    public Vector3Int pos;
    public Texture2D tex;// = new int[4, 2] tex;

    public textureContainer(Vector3Int pos, Texture2D tex)
    {
        this.pos = pos;
        this.tex = tex;
    }
}*/