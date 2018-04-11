using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISmap : MonoBehaviour {
    // Use this for initialization
    public int planeSize = 0;
    public float whproportion = 0;
    MapPlane mapplane;    //tymczasowe
    int orderCounter = 0;

    GISdata gisdata;
    //public string path = @"G:\POLITECHNIKA\PROJEKTY\#8 Technologie map cyfrowych\geo3\geo3\TMC_PG\Assets\testowyOSM.osm";

    void Start () {
        gisdata = GISparser.LoadOSM(@"G:\POLITECHNIKA\PROJEKTY\#8 Technologie map cyfrowych\geo3\geo3\TMC_PG\Assets\testowyOSM.osm");

        /*gisdata.minLat = 54.3690100;
        gisdata.minLon = 18.6095200;
        gisdata.maxLat = 54.3745700;
        gisdata.maxLon = 18.6237900;*/

        setPlaneSize();
        //tmpsize = planeSize / 2; inaczej to zrobic
        setPlane();
    }
	
    void setPlaneSize()
    {
        int h = cam.pixelHeight;
        int w = cam.pixelWidth;
        if (h > w)
        {
            planeSize = h;
            whproportion = 1;
        } else
        {
            planeSize = w;
            whproportion = (float)w / (float)h;
        }
        
    }

	// Update is called once per frame
	void Update () {
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
        if (Input.GetKey(KeyCode.Q))
        {
            Rotate(1);
        }
        if (Input.GetKey(KeyCode.E))
        {
            Rotate(-1);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            Bend(1);
        }
        if (Input.GetKey(KeyCode.C))
        {
            Bend(-1);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            ZoomOut();
        }
        if (Input.mouseScrollDelta.y > 0)
        {
            ZoomIn();
        }
        camUpdate();
        planeUpdate();
    }

    //mapa
    public void planeUpdate()
    {
        if (
            ((mapplane.transform.localScale - (Vector3.one * (tmpsize * 2.0f * 100.0f / planeSize) * whproportion)).magnitude >= 0.001) ||
            ((mapplane.transform.position - ghostPivot.transform.position).magnitude >= tmpsize * 2.0f* whproportion*0.8)
            )
        {
            setPlane();
        }
    }

    public void setPlane()
    {
        //Destroy(mapplane);
        var tmp = Resources.Load("Prefabs/GISplane", typeof(GameObject));
        GameObject go = (GameObject)Instantiate(tmp);
        go.transform.position = new Vector3(ghostPivot.transform.position.x, 0, ghostPivot.transform.position.z);
        //go.transform.localScale = Vector3.one * (tmpsize * (float)planeSize / 1000.0f)/2.0f;
        go.transform.localScale = Vector3.one * (tmpsize * 2.0f * 100.0f / planeSize)* whproportion;
        var comp = go.GetComponent<MapPlane>();
        var spr = go.GetComponent<SpriteRenderer>();
        comp.resolution = new Vector2Int(planeSize, planeSize);
        //go.transform.rotation.eulerAngles(90,0,0);
        MapPlane mp = go.GetComponent<MapPlane>();
        go.transform.parent = this.transform;
        mapplane = mp;
        spr.sortingOrder = orderCounter;
        ++orderCounter;

        //texture
        var min = GISparser.LatlonToXY(new Vector2d(gisdata.minLon, gisdata.minLat));
        var max = GISparser.LatlonToXY(new Vector2d(gisdata.maxLon, gisdata.maxLat));

        var sr = (max - min) / 2;

        sr += max;

        double kat = 0.005;
        double obszar = kat * tmpsize * 2.0f * whproportion;
            
        min = sr - new Vector2d(obszar, obszar);
        max = sr + new Vector2d(obszar, obszar);

        min += new Vector2d(ghostPivot.transform.position.x* kat * 2.0f, ghostPivot.transform.position.z* kat * 2.0f);
        max += new Vector2d(ghostPivot.transform.position.x* kat * 2.0f, ghostPivot.transform.position.z* kat * 2.0f);

        comp.fillTexture(min, max, gisdata);


    }

    //kamera
    private GameObject ghostPivot = null;   //wirtualny punkt na który patrzy kamera, punkt dotyka mapy.
    private GameObject ghostCamera = null;  //wirtualna kamera która porusza się gwałtownie i prawdziwa kamera jest do niej "dociągana"
    private float r = 100f;                  //odległość kamery od pivota (zoom)
    private float tmpr = 1f;
    private float tmpsize = 5f;
    private float alpha = 89f;              //kąt pomiędzy płaszczyzną mapy, a prostą przechodzącą przez wirtualną kamerę i pivot. 90f to widok od góry.                

    //dane tymczasowe, zbiera zadania do wykonania i wykonuje je wszystkie na raz podczas update.
    private Vector3 tmpDirection = Vector3.zero;
    private float tmpRotation = 0;
    private float tmpBend = 0;
    private float tmpZoom = 0;

    //Prędkości różnych typów ruchów.
    private const float MoveVelocity = 5.0f;   //prędkość przesuwania się
    private const float RotVelocity = 100.0f;   //prędkość obrotu dookoła pivota
    private const float BendVelocity = 80.0f;   //prędkość zmiany wysokości kamery
    private const float ZoomVelocity = 400.0f;   //prędkość oddalania kamery od mapy

    //Gwałtowność ruchów
    private const float Rapidity = 5f;

    //referancje
    public Camera cam = null;

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
    public void Rotate(float direction)
    {
        direction = Limit(-100, 100, direction);
        tmpRotation += direction;
    }
    public void Bend(float direction)
    {
        direction = Limit(-100, 100, direction);
        tmpBend += direction;
    }
    public void ZoomIn()
    {
        Zoom(1f);
    }
    public void ZoomOut()
    {
        Zoom(-1f);
    }
    private void Zoom(float direction)
    {
        direction = Limit(-100, 100, direction);
        tmpZoom += direction;
    }

    private void Awake()
    {
        ghostPivot = new GameObject();
        ghostCamera = new GameObject();
        ghostPivot.gameObject.transform.parent = gameObject.transform;
        ghostCamera.gameObject.transform.parent = gameObject.transform;
        ghostPivot.transform.Translate(0, 0, 0);
        cam.orthographic = true;       
    }

    void camUpdate()
    {        
        if (cam == null)
        {
            Debug.LogError("Błąd, cam = null");
            return;
        }
        setPlaneSize();
        float moveOffset = MoveVelocity * Time.deltaTime* tmpsize;
        float rotOffset = RotVelocity * Time.deltaTime * tmpRotation;
        float bendOffset = BendVelocity * Time.deltaTime;
        float zoomOffset = ZoomVelocity * Time.deltaTime * tmpZoom;

        Vector3 move = new Vector3(moveOffset * tmpDirection.x, 0, moveOffset * tmpDirection.z);
        Quaternion rotation = Quaternion.Euler(0, rotOffset, 0);

        //Party Hard
        float rAlpha = Mathf.PI / 180f * alpha;
        float z = 0, y = 0;
        //tmpr += zoomOffset;
        //cam.orthographicSize += zoomOffset;
        //cam.orthographicSize = Limit(0.001f, 100f, cam.orthographicSize);
        //tmpr = Limit(-100000f, 100000f, tmpr);   //Ograniczenie zooma
        //r = Mathf.Pow(1.2f,-tmpr);
        //r = Limit(0f, 100000f, r);
        const float prop = 1.5f;
        //if (zoomOffset > 0) r = r / prop;
        //if (zoomOffset < 0) r = r * prop;

        if (zoomOffset > 0) tmpsize = tmpsize / prop;
        if (zoomOffset < 0) tmpsize = tmpsize * prop;

        tmpsize = Limit(0.00000001f, 100f, tmpsize);

        r = Limit(0f, 100000f, r);
        alpha += bendOffset * tmpBend;
        alpha = Limit(10f, 89.9f, alpha); //Ograniczenie benda
        ghostPivot.transform.localRotation *= rotation;
        z = r / (Mathf.Sqrt((1f + (Mathf.Tan(rAlpha) * Mathf.Tan(rAlpha)))));
        y = z * Mathf.Tan(rAlpha);

        ghostPivot.transform.Translate(move);

        ghostCamera.transform.position = ghostPivot.transform.position;
        ghostCamera.transform.rotation = ghostPivot.transform.rotation;
        ghostCamera.transform.Translate(0f, y, -z);
        ghostCamera.transform.rotation *= Quaternion.Euler(alpha, 0, 0);


        //płynne dosuwanie prawdziwej kamery do ghost kamery
        cam.transform.position = Vector3.Lerp(cam.transform.position, ghostCamera.transform.position, Rapidity * Time.deltaTime);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, ghostCamera.transform.rotation, Rapidity * Time.deltaTime);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, tmpsize, Rapidity * Time.deltaTime);

        //reset
        tmpDirection = Vector3.zero;
        tmpRotation = 0;
        tmpZoom = 0;
        tmpBend = 0;
    }

    //Jeżeli wartośc wyjdzie poza przedział to otrzymuje wartość graniczną
    private float Limit(float down, float up, float value)
    {
        if (value < down) value = down;
        if (value > up) value = up;
        return value;
    }

    //Teleportuje kamerę o wskazany wektor
    private void Teleportation(Vector3 offset)
    {
        cam.transform.position += offset;
        ghostPivot.transform.position += offset;
        ghostCamera.transform.position += offset;
    }

    public void SetCameraPivot(Vector3 position)
    {
        ghostPivot.transform.position = position;
    }
}
