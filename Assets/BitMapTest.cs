using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitMapTest : MonoBehaviour {

    SpriteRenderer rend;

	// Use this for initialization
	void Start () {
        rend = GetComponent<SpriteRenderer>();

        Texture2D tex = new Texture2D(100,100);

        for (int x = 0; x < tex.width; ++x)
        {
            for (int y = 0; y < tex.height; ++y)
            {
                tex.SetPixel(x,y, randomColor());
            }
        }
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        Sprite newSprite = Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),Vector2.one*0.5f);

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
