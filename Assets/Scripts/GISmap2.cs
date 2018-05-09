using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;
using UnityEngine.UI;

public class GISmap2 : MonoBehaviour {
    GISdata gisdata = null;
    GISquadtree qt;

    public int zoom;
    private int oldzoom;

    public Camera cam;

    int layerId = 0;

    //punkty wyznaczają prostokąt który zawiera widoczne chunki
    Vector2Int ch1;//min
    Vector2Int ch2;//max

    public InputField ipath;

    public GameObject c0, c1;

    GameObject fullPlane = null;

    Color globalColor = Color.blue;

    Texture2D generateFullViewTexture()
    {       
        int width = ch2.x - ch1.x+1;
        int height = ch2.y - ch1.y+1;

        Vector2d range = getCameraRange();
        Vector2d p1 = new Vector2d(-(float)range.x / 2f + cam.transform.position.x, -(float)range.y / 2f + -cam.transform.position.z);
        Vector2d p2 = new Vector2d((float)range.x / 2f + cam.transform.position.x, (float)range.y / 2f + -cam.transform.position.z);

        Texture2D tex = new Texture2D(width*256, height*256);

        Vector2d chunkLow = new Vector2d(((double)(ch1.x) / (double)(1 << zoom)) * 256.0, ((double)(ch1.y) / (double)(1 << zoom)) * 256.0);
        Vector2d chunkHigh = new Vector2d(((double)(ch2.x+1) / (double)(1 << zoom)) * 256.0, ((double)(ch2.y+1) / (double)(1 << zoom)) * 256.0);

        var c = Color.cyan;
        //fill
        for (int x = 0; x < tex.width; ++x)
        {
            for (int y = 0; y < tex.height; ++y)
            {
                tex.SetPixel(x, y, c);
            }
        }
        //heatmap
        float[,] heatmap = new float[resolution.x, resolution.y];
        

        //details
        foreach (var way in gisdata.wayContainer)
        {
            drawWayOptimal(ref tex, way, chunkLow, chunkHigh);
        }

        return tex;
    }

    float[,] heatmapCreator(Texture2D tex, Vector2d p1, Vector2d p2, string key, string value)
    {
        //node.tags[key] == value
        float[,] result = new float[tex.width,tex.height];
        foreach (var node in gisdata.nodeContainer)
        {
            try
            {
                if (node.tags[key] == value)
                {
                    for (int x = 0; x < tex.width; ++x)
                    {
                        for (int y = 0; y < tex.height; ++y)
                        {
                            
                        }
                    }
                } 
            } catch { }
        }
        return result;
    }

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
        var path = GISparser.getQuadPath(new Vector2Int(cx, cy), z);
        var waysList = qt.getObjects(path);

        

        Vector2d chunkLow = new Vector2d(((double)(cx)/ (double)(1 << z))*256.0, ((double)(cy) / (double)(1 << z)) * 256.0);
        Vector2d chunkHigh = new Vector2d(((double)(cx+1) / (double)(1 << z)) * 256.0, ((double)(cy+1) / (double)(1 << z)) * 256.0);

        var c = Color.white;
        for (int x = 0; x < 256; ++x)
        {
            for (int y = 0; y < 256; ++y)
            {
                tex.SetPixel(x, y, c);               
            }
        }

        foreach (var way in waysList)
        {
            //lineChecker(Vector2d l1, Vector2d l2, Vector2d p, float thickness);
            //drawWay(ref tex, way, chunkLow, chunkHigh);
            drawWayOptimal(ref tex, way, chunkLow, chunkHigh);
        }


        if (true)
        {
            Color bor = Color.gray;
            for (int i = 0; i < 256; ++i)
            {
                tex.SetPixel(0, i, bor);
                tex.SetPixel(255, i, bor);
                tex.SetPixel(i, 0, bor);
                tex.SetPixel(i, 255, bor);
            }
        }

        return tex;
    }

    void drawWay(ref Texture2D tex, GISway way, Vector2d chunkLow, Vector2d chunkHigh)
    {
        GISnode prev = null;
        Vector2d? pprev = null;
        bool renderPrev = false;
        float thickness = 10;
        foreach (var node in way.localNodeContainer)
        {
            var p = new Vector2d();
            p.x = (node.XY.x - chunkLow.x) / (chunkHigh.x - chunkLow.x)*256.0;
            p.y = (node.XY.y - chunkLow.y) / (chunkHigh.y - chunkLow.y) * 256.0;

            //if (prev == null) {prev = node; continue;}
            if (((p.x >= 0 && p.y >= 0 && p.x < 256 && p.y < 256) || renderPrev)&& pprev != null)
            {
                //punkt node jest w chunku
                for (int y = 0; y < 256; y++)
                {
                    for (int x = 0; x < 256; x++)
                    {
                        bool rysuj = GISparser.lineChecker((Vector2d)pprev, p, new Vector2d(x,y), thickness);
                        if (rysuj)
                        {
                            tex.SetPixel(x,y,Color.black);
                        }
                    }
                }
                //koniec
                renderPrev = true;               
            } else
            {
                renderPrev = false;
            }
            pprev = p;
        }
    }

    void drawWayOptimal(ref Texture2D tex, GISway way, Vector2d chunkLow, Vector2d chunkHigh)
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
                pointsToBorder(ref a0, ref a1, chunkLow, chunkHigh);
                var p0 = new Vector2Int();
                p0.x = (int)((pprev.XY.x - chunkLow.x) / (chunkHigh.x - chunkLow.x) * tex.width);
                p0.y = (int)((pprev.XY.y - chunkLow.y) / (chunkHigh.y - chunkLow.y) * tex.height);
                var p1 = new Vector2Int();
                p1.x = (int)((node.XY.x - chunkLow.x) / (chunkHigh.x - chunkLow.x) * tex.width);
                p1.y = (int)((node.XY.y - chunkLow.y) / (chunkHigh.y - chunkLow.y) * tex.height);               
                drawLine(ref tex, p0, p1, globalColor);
            }
            //koniec
            pprev = node;
        }       
    }

    void drawLine(ref Texture2D tex, Vector2Int p0, Vector2Int p1, Color c)
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
            tex.SetPixel(x,y,c);
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

    //nie zmienia punktów które są w obszarze, ale jak są poza nim to je dociąga do krawędzi
    void pointsToBorder(ref Vector2d p0, ref Vector2d p1, Vector2d bl, Vector2d bh)
    {
        bool b0 = isInsideBorder(p0, bl, bh);
        bool b1 = isInsideBorder(p1, bl, bh);
        if (!b0 || !b1)
        {
            Vector2d blu = new Vector2d(bl.x, bh.y);
            Vector2d bhd = new Vector2d(bh.x, bl.y);

            var lewy = intersection(p0,p1,bl,blu);
            var gorny = intersection(p0, p1, blu, bh);
            var prawy = intersection(p0, p1, bh, bhd);
            var dolny = intersection(p0, p1, bhd, bh);

            bool lb = false, gb = false, pb = false, db = false;

            if (lewy.y > bl.y && lewy.y < blu.y) lb = true;
            if (gorny.x > blu.x && gorny.x < bh.x) gb = true;
            if (prawy.y > bhd.y && prawy.y < bh.y) pb = true;
            if (dolny.x > bl.x && dolny.x < bhd.x) db = true;

            if (!b0) if (lb && p0.x < bl.x) p0 = lewy;
            else
            if (gb && p0.y > blu.y) p0 = gorny;
            else
            if (pb && p0.x > bh.x) p0 = prawy;
            else 
            if (db && p0.y < bl.y) p0 = dolny;

            if (!b1) if (lb && p1.x < bl.x) p1 = lewy;
            else
            if (gb && p1.y > blu.y) p1 = gorny;
            else
            if (pb && p1.x > bh.x) p1 = prawy;
            else
            if (db && p1.y < bl.y) p1 = dolny;
        }
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

    bool isInsideBorder(Vector2d p, Vector2d bl, Vector2d bh)
    {
        if (p.x >= bl.x && p.x <= bh.x && p.y >= bl.y && p.y <= bh.y) return true;
        return false;
    }

    //sprawdza czy linia jest obecna w chunku
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

        var lewy = intersection(p0, p1, bl, blu);if (lewy.y > bl.y && lewy.y < blu.y) return true;
        var gorny = intersection(p0, p1, blu, bh);if (gorny.x > blu.x && gorny.x < bh.x) return true;
        var prawy = intersection(p0, p1, bh, bhd);if (prawy.y > bhd.y && prawy.y < bh.y) return true;
        var dolny = intersection(p0, p1, bhd, bh);if (dolny.x > bl.x && dolny.x < bhd.x) return true;
        return false;
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
        //ghostCamera.transform.position = cam.transform.position;
        //loadFile("G:\\POLITECHNIKA\\PROJEKTY\\#8 Technologie map cyfrowych\\maly.osm"); 
        ghostCamera = new GameObject();
        ghostCamera.transform.position = cam.transform.position;
        oldzoom = zoom;
        
    }
  

    //qt = new GISquadtree(null);

    public void loadFile(string text)
    {
        gisdata = GISparser.LoadOSM(text);
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
        keysUpdate();
        if (oldzoom != zoom)
        {  //zoom uległ zmianie
            if (zoom > 20) zoom = oldzoom;
            else
            if (zoom < 0) zoom = oldzoom;
            else
            {
                oldzoom = zoom;
                ++layerId;
                if (fullPlane != null) { Destroy(fullPlane); fullPlane = null; }
            }
        }
        updateCamera();
        Vector2d range = getCameraRange();
        //cam.orthographicSize = (float)cam.pixelHeight / Mathf.Pow(2, zoom) / 2.0f;
        updateChunks();             
    }

    void keysUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Move(Vector2.up);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Move(Vector2.left);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Move(Vector2.down);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Move(Vector2.right);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoom--;
        }
        if (Input.mouseScrollDelta.y > 0)
        {
            zoom++;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            loadFile(ipath.text);
            //loadFile("G:\\POLITECHNIKA\\PROJEKTY\\#8 Technologie map cyfrowych\\maly.osm");
            qt = new GISquadtree(null);
            qt.size = new Vector2d(256, 256);
            qt.position = new Vector2d(0, 0);
            foreach (var way in gisdata.wayContainer)
            {
                qt.insert(way);
            }

            //gisdata.maxLat

            //ustawienie kamery tam gdzie coś jest
            Vector2d sr = new Vector2d((gisdata.maxLat + gisdata.minLat) / 2.0, (gisdata.maxLon + gisdata.minLon) / 2.0);
            var cameraStartPos = GISparser.LatLonToWeb(sr);
            ghostCamera.transform.position = new Vector3((float)cameraStartPos.x, cam.transform.position.y, -(float)cameraStartPos.y);           
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Vector2d chunkLow = new Vector2d(((double)(ch1.x) / (double)(1 << zoom)) * 256.0, ((double)(ch1.y) / (double)(1 << zoom)) * 256.0);
            Vector2d chunkHigh = new Vector2d(((double)(ch2.x) / (double)(1 << zoom)) * 256.0, ((double)(ch2.y) / (double)(1 << zoom)) * 256.0);

            int size = (1 << zoom);
            double length = 256.0 / (double)size;

            var go = setPlane(new Vector2d(chunkLow.x, -chunkLow.y), length);//tworzy chunki tylko gdy nie istnieją
            var spr = go.GetComponent<SpriteRenderer>();
            var com = go.GetComponent<BitMapTest>();
            spr.sortingOrder = 1000+layerId;
            com.setTexture(generateFullViewTexture());
            if (fullPlane != null) Destroy(fullPlane);
            fullPlane = go;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            /*int size = (1 << zoom);
            double length = 256.0 / (double)size;
            var o = setPlane(new Vector2d(0, 0), length);
            var bt = o.GetComponent<BitMapTest>();
            var c = Color.cyan; Texture2D tex = new Texture2D(256*2, 256*2);           

            for (int x = 0; x < 256*2; ++x)
            {
                for (int y = 0; y < 256*2; ++y)
                {
                    tex.SetPixel(x, y, BitMapTest.randomColor());
                }
            }
            bt.setTexture(tex);   */        
        }
        
    }
    //kamera
    private Vector3 tmpDirection = Vector3.zero;
    private GameObject ghostCamera = null;
    private const float MoveVelocity = 50.0f;
    private float tmpsize = 5;
    //Gwałtowność ruchów
    private const float Rapidity = 5f;
    public void Move(Vector2 direction)
    {
        if (direction == null) return;
        if (direction.magnitude > 100f)
        {
            direction.Normalize();
            direction = direction * 100f;
        }
        tmpDirection += new Vector3(direction.x, 0, direction.y);
    }

    public void updateCamera()
    {
        tmpsize = 5.0f / (float)(1 << zoom);
        tmpsize = Limit(0.00000001f, 100f, tmpsize);

        float moveOffset = MoveVelocity * Time.deltaTime * tmpsize;
        Vector3 move = new Vector3(moveOffset * tmpDirection.x, 0, moveOffset * tmpDirection.z);
        ghostCamera.transform.Translate(move);

        cam.transform.position = Vector3.Lerp(cam.transform.position, ghostCamera.transform.position, Rapidity * Time.deltaTime);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, (float)cam.pixelHeight / Mathf.Pow(2, zoom) / 2.0f, Rapidity * Time.deltaTime);

        tmpDirection = Vector3.zero;
    }
    //Jeżeli wartośc wyjdzie poza przedział to otrzymuje wartość graniczną
    private float Limit(float down, float up, float value)
    {
        if (value < down) value = down;
        if (value > up) value = up;
        return value;
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
        globalColor = Color.black;
        var requestedChunks = getChunkCoordsList();
        if (gisdata == null) return;

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
                com.setTexture(generateTexture(ele.x, ele.y, zoom));
                //toProcessChunks.Add(ele);
                doneChunks.Add(new chunkContainer(ele, spr));
                break;
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

        /*c0.transform.position = new Vector3((float)p1.x,0, -(float)p1.y);
        c1.transform.position = new Vector3((float)p2.x, 0, -(float)p2.y);*/

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
        //double size = cam.orthographicSize;
        double size = (float)cam.pixelHeight / Mathf.Pow(2, zoom) / 2.0f;
        Vector2d result = new Vector2d(2*size*c, 2 * size);
        return result;
    }

    private void Awake()
    {
        cam.orthographic = true;
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

