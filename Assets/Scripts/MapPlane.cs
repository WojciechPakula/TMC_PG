﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlane : MonoBehaviour {
    public Vector2Int resolution;   
    public Vector2 wsp;

    SpriteRenderer rend;
    Texture2D tex;

    // Use this for initialization
    void Start () {
        rend = GetComponent<SpriteRenderer>();

        tex = new Texture2D(resolution.x, resolution.y);

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

        rend.sprite = newSprite;
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
