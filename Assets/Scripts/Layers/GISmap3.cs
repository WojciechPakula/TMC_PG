using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISmap3 : MonoBehaviour {

    public Camera cam;
    public int zoom;
    public Vector2d worldOffset = new Vector2d(0, 0);

    private int oldzoom;
    List<GISlayer> layers = new List<GISlayer>();
    //Vector2d ghostCameraPosition;   
    int segmentCounter = 0;

    //debug
    public GameObject o1;
    public GameObject o2;
    public GameObject cur;

    private Dictionary<Vector3Int, GameObject> segmentCache = new Dictionary<Vector3Int, GameObject>();
    private List<GameObject> allSegments = new List<GameObject>();

    // Use this for initialization
    void Start () {
        cam.orthographic = true;
        //ghostCameraPosition = new Vector2d(0,0);
        oldzoom = zoom;
        layers.Add(new GISlayerTest());
        ghostCamera = new Vector3d(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
        //doTestowania();
    }
    // Update is called once per frame
    void Update () {
        keysUpdate();
        zoomUpdate();
        //updateOffset();
        cameraCenterizer();
        updateCamera();
        setCameraSize();       
        updateSegments();
        //cur.transform.position = new Vector3((float)wp.x,0,(float)wp.y);
    }

    void cameraCenterizer()
    {
        if (ghostCamera.x > 0.1 || ghostCamera.x < -0.1 || ghostCamera.y > 0.1 || ghostCamera.y < -0.1)
        {
            offsetWorld();            
        }
    }

    void offsetWorld()
    {
        var p0 = ghostCamera;
        worldOffset.x += p0.x;
        worldOffset.y += p0.y;
        var cp = cam.transform.localPosition;
        cp.x -= (float)p0.x;
        cp.y -= (float)p0.y;
        cam.transform.localPosition = cp;
        ghostCamera.x = 0;
        ghostCamera.y = 0;
        updateOffset();
    }

    //przesówa obiekty na spód aby nowe były na pewno widoczne
    private void allObjectsStepBack()
    {
        foreach(var ob in allSegments)
        {
            var p = ob.transform.localPosition;
            p.z++;
            ob.transform.localPosition = p;
        }
    }

    private void doTestowania()
    {
        var empty = new GameObject("doTestowania");
        empty.transform.parent = this.transform;
        empty.transform.localScale *= 1 / 2.56f;
        var sprite = empty.AddComponent<SpriteRenderer>();
        var segment = empty.AddComponent<GISlayerSegment>();
        //empty.transform.rotation = Quaternion.Euler(-90,0,0);        
    }

    //position in unitary units
    private Vector3d getGhostCamPosition()
    {
        //return ghostCameraPosition;
        return ghostCamera;
    }

    //sprawdza czy trzeba coś wyrenderować czy pobrać z cache
    private void updateSegments()
    {
        //pobierz jakie segmenty są widoczne
        var visibleSegments = getVisibleSegmentCoordsList();

        //sprawdź czy są już wyrenderowane

        //renderuj segmenty
        foreach (var segment in visibleSegments)
        {
            GameObject go;
            bool czyIstnieje = segmentCache.TryGetValue(segment, out go);
            if (czyIstnieje&&go!=null)//warunek renderowania 
            {
                Vector3 p = go.transform.position;
                p.z = 0;//nooffset
                go.transform.position = p;
                continue;
            }
            
            Texture2D tmpTex = null;
            foreach (var layer in layers)
            {
                var tex = layer.renderSegment(segment);
                if (tmpTex != null)
                {
                    var tmpc = tmpTex.GetPixels();
                    var tc = tex.GetPixels();
                    for (int i = 0; i < tmpc.Length; ++i)
                    {
                        Color c = new Color(
                            (tmpc[i].r * tmpc[i].a) + (tc[i].r * (1.0f - tmpc[i].a)),
                            (tmpc[i].g * tmpc[i].a) + (tc[i].g * (1.0f - tmpc[i].a)),
                            (tmpc[i].b * tmpc[i].a) + (tc[i].b * (1.0f - tmpc[i].a)),
                            tc[i].a);
                        tmpc[i] = c;
                    }
                    tex.SetPixels(tmpc);
                }
                tmpTex = tex;
            }            
            //stworz gameobject i dodaj do slownika
            createNewSegment(segment, tmpTex);
            break;
        }
    }

    void updateOffset()//nooffset
    {
        foreach(var ob in allSegments)
        {
            var ls = ob.GetComponent<GISlayerSegment>();
            ls.refreshWorldPosition(worldOffset);
        }
    }

    GameObject createNewSegment(Vector3Int pos, Texture2D tex)
    {
        GameObject nowySegment = new GameObject("Segment-"+ segmentCounter+",x:"+pos.x+",y:"+pos.y+",z:"+pos.z); segmentCounter++;

        nowySegment.transform.parent = this.transform;
        nowySegment.transform.localScale *= 1 / 2.56f;
        var sprite = nowySegment.AddComponent<SpriteRenderer>();
        var segment = nowySegment.AddComponent<GISlayerSegment>();
        nowySegment.transform.rotation = Quaternion.Euler(180, 0, 0);
        segment.setTexture(tex);
        segment.segmentPosition = pos;
        segment.position = GISlayer.segmentToUnitary(pos);
        nowySegment.transform.localScale /= (1<<zoom);
        //segment.rend;

        segment.refreshWorldPosition(worldOffset);//nooffset

        allSegments.Add(nowySegment);
        segmentCache.Add(pos, nowySegment);
        return nowySegment;
    }

    //zwraca krawędzie widoczności w unitarnych jednostkach
    private void getGhostCamCorners(int pixelWidth, int pixelHeight, out Vector2d c0, out Vector2d c1)
    {
        var camPos = getGhostCamPosition();
        
        double hw = ((double)pixelWidth / (256.0 * (double)(1 << zoom)))/2.0;//half width in unitary
        double hh = ((double)pixelHeight / (256.0 * (double)(1 << zoom)))/2.0;

        c0 = new Vector2d(camPos.x + worldOffset.x - hw, -camPos.y - worldOffset.y - hh);//nooffset
        c1 = new Vector2d(camPos.x + worldOffset.x + hw, -camPos.y - worldOffset.y + hh);
    }

    //zwraca pozycje segmentów które są widoczne
    List<Vector3Int> getVisibleSegmentCoordsList()
    {
        List<Vector3Int> result = new List<Vector3Int>();
        int width = cam.pixelWidth;
        int height = cam.pixelHeight;
        Vector2d c0;
        Vector2d c1;
        getGhostCamCorners(width, height, out c0, out c1);
        Vector3Int ch0 = GISlayer.unitaryToSegment(c0, zoom);
        Vector3Int ch1 = GISlayer.unitaryToSegment(c1, zoom);

        if (ch0.x < 0) ch0.x = 0;
        if (ch0.y < 0) ch0.y = 0;
        if (ch1.x < 0) ch1.x = 0;
        if (ch1.y < 0) ch1.y = 0;

        for (int y = ch0.y; y <= ch1.y && y < (1 << zoom); ++y)
        {
            for (int x = ch0.x; x <= ch1.x && x < (1 << zoom); ++x)
            {
                result.Add(new Vector3Int(x, y, zoom));
            }
        }

        return result;
    }

    //ustawia rozmiar kamery tak, aby segmenty pikseli mapy pasowały do pikseli monitora
    private void setCameraSize()
    {
        Vector2d range = getCameraRange();
        Vector2d p1 = new Vector2d(-(float)range.x / 2f + cam.transform.position.x, -(float)range.y / 2f + -cam.transform.position.y);
        Vector2d p2 = new Vector2d((float)range.x / 2f + cam.transform.position.x, (float)range.y / 2f + -cam.transform.position.y);
        //o1.transform.position = new Vector3((float)p1.x,0, -(float)p1.y);low
        //o2.transform.position = new Vector3((float)p2.x, 0, -(float)p2.y);high
        //cam.orthographicSize = (float)cam.pixelHeight / ((1 << (zoom)) * 256) / 2.0f;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, (float)cam.pixelHeight / ((1 << (zoom)) * 256) / 2.0f, Rapidity * Time.deltaTime);
        var wp = cursorToWorldPosition();
    }

    private Vector2d getCameraRange()
    {
        float c = (float)cam.pixelWidth / (float)cam.pixelHeight;
        double size = cam.orthographicSize;
        //double size = (float)cam.pixelHeight / Mathf.Pow(2, zoom) / 2.0f;
        Vector2d result = new Vector2d(2 * size * c, 2 * size);
        return result;
    }

    //
    Vector2d cursorToWorldPosition()
    {
        Vector2d result = new Vector2d();
        float cc = (float)cam.pixelWidth / (float)cam.pixelHeight;
        double size = cam.orthographicSize;
        Vector2d range = new Vector2d(2 * size * cc, 2 * size);
        Vector2d a = new Vector2d(-(float)range.x / 2f + cam.transform.position.x, (float)range.y / 2f + -cam.transform.position.y);
        Vector2d b = new Vector2d((float)range.x / 2f + cam.transform.position.x, -(float)range.y / 2f + -cam.transform.position.y);
        Vector2d c = new Vector2d(Input.mousePosition.x, Input.mousePosition.y);
        result.x = (float)(-(cam.pixelWidth - c.x)/ cam.pixelWidth*(b.x-a.x)+b.x);
        result.y = (float)((cam.pixelHeight - c.y) / cam.pixelHeight * (b.y - a.y) - b.y);
        return result;
    }

    void zoomUpdate()
    {
        if (oldzoom != zoom)
        {  //zoom uległ zmianie
            if (zoom > 20) zoom = oldzoom;
            else
            if (zoom < 1) zoom = oldzoom;
            else
            {
                oldzoom = zoom;
                allObjectsStepBack();
                //++layerId;
                //if (fullPlane != null) { Destroy(fullPlane); fullPlane = null; }
            }
        }
    }

    //obsługa klawiszy
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
            //ladowaniePrzyciskiem();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            //heatMapaZPrzycisku();
        }
    }
    //kamera
    private Vector3d tmpDirection = Vector3d.zero;
    //private GameObject ghostCamera = null;
    private Vector3d ghostCamera;
    private const float MoveVelocity = 3.0f;
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
        tmpDirection += new Vector3d(direction.x, direction.y, 0);
    }

    public void updateCamera()
    {
        tmpsize = 5.0f / (float)(1 << zoom);
        tmpsize = Limit(0.00000001f, 100f, tmpsize);

        double moveOffset = MoveVelocity * Time.deltaTime * tmpsize;
        Vector3d move = new Vector3d(moveOffset * tmpDirection.x, moveOffset * tmpDirection.y, moveOffset * tmpDirection.z);  //nooffset
        //ghostCamera.transform.Translate(move);
        ghostCamera += move;
        //Vector3 ghostFloat = new Vector3((float)ghostCamera.x, (float)ghostCamera.y, (float)ghostCamera.z);
        Vector3d dpos = new Vector3d(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
        Vector3d tmp = Vector3d.Lerp(dpos, ghostCamera, Rapidity * Time.deltaTime);
        cam.transform.localPosition = (Vector3)tmp;
        //cam.transform.localPosition = new Vector3(0,1,0);
        //cam.transform.position = Vector3d.Lerp(dpos, ghostFloat, Rapidity * Time.deltaTime);
        //cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, (float)cam.pixelHeight / Mathf.Pow(2, zoom) / 2.0f, Rapidity * Time.deltaTime);

        tmpDirection = Vector3d.zero;
    }
    //Jeżeli wartośc wyjdzie poza przedział to otrzymuje wartość graniczną
    private float Limit(float down, float up, float value)
    {
        if (value < down) value = down;
        if (value > up) value = up;
        return value;
    }
}
