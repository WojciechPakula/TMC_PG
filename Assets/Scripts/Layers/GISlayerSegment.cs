using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISlayerSegment : MonoBehaviour {

    public Vector2d position;
    public Vector3Int segmentPosition;

    public int x=256;
    public int y=256;
    public Vector2 wsp;

    SpriteRenderer rend;
    Texture2D tex = null;
    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }
    /*void Start()
    {
        rend = GetComponent<SpriteRenderer>();

        tex = new Texture2D(x, y);

        for (int x = 0; x < tex.width; ++x)
        {
            for (int y = 0; y < tex.height; ++y)
            {
                tex.SetPixel(x, y, randomColor());
            }
        }
        tex.SetPixel(0, 0, Color.black);
        tex.SetPixel(1, 0, Color.black);
        tex.SetPixel(2, 0, Color.black);
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));

        rend.sprite = newSprite;
    }*/
    public void refreshWorldPosition(Vector2d worldOffset)
    {
        this.transform.localPosition = new Vector3((float)(this.position.x- worldOffset.x), -(float)(this.position.y + worldOffset.y), (float)(this.transform.localPosition.z));//nooffset
    }
    public static Color randomColor()
    {
        Color c = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        return c;
    }

    public void setTexture(Texture2D tex2)
    {
        tex = tex2;
        //tex = GISparser.testOSMpng();
        tex.Apply();
        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
        rend.sprite = newSprite;
    }
    public bool isTexture()
    {
        if (tex == null) return false;
        return true;
    }
    /*void Update()
    {
        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), wsp);

        rend.sprite = newSprite;
    }*/
}
