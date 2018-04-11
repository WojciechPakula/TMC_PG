using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISmap : MonoBehaviour {
	// Use this for initialization
	void Start () {

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
    }

    //kamera
    private GameObject ghostPivot = null;   //wirtualny punkt na który patrzy kamera, punkt dotyka mapy.
    private GameObject ghostCamera = null;  //wirtualna kamera która porusza się gwałtownie i prawdziwa kamera jest do niej "dociągana"
    private float r = 100f;                  //odległość kamery od pivota (zoom)
    private float alpha = 70f;              //kąt pomiędzy płaszczyzną mapy, a prostą przechodzącą przez wirtualną kamerę i pivot. 90f to widok od góry.                

    //dane tymczasowe, zbiera zadania do wykonania i wykonuje je wszystkie na raz podczas update.
    private Vector3 tmpDirection = Vector3.zero;
    private float tmpRotation = 0;
    private float tmpBend = 0;
    private float tmpZoom = 0;

    //Prędkości różnych typów ruchów.
    private const float MoveVelocity = 200.0f;   //prędkość przesuwania się
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
        Zoom(-1f);
    }
    public void ZoomOut()
    {
        Zoom(1f);
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
        ghostPivot.transform.Translate(0, 3, 0);
        //cam.orthographic = true;       
    }

    void camUpdate()
    {
        if (cam == null)
        {
            Debug.LogError("Błąd, cam = null");
            return;
        }
        float moveOffset = MoveVelocity * Time.deltaTime;
        float rotOffset = RotVelocity * Time.deltaTime * tmpRotation;
        float bendOffset = BendVelocity * Time.deltaTime;
        float zoomOffset = ZoomVelocity * Time.deltaTime * tmpZoom;

        Vector3 move = new Vector3(moveOffset * tmpDirection.x, 0, moveOffset * tmpDirection.z);
        Quaternion rotation = Quaternion.Euler(0, rotOffset, 0);

        //Party Hard
        float rAlpha = Mathf.PI / 180f * alpha;
        float z = 0, y = 0;
        r += zoomOffset;
        cam.orthographicSize += zoomOffset;
        cam.orthographicSize = Limit(0.001f, 100f, cam.orthographicSize);
        r = Limit(10f, 100f, r);   //Ograniczenie zooma
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
