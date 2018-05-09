using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHeat : MonoBehaviour {
    public float value;
    SpriteRenderer sr;
	// Use this for initialization
	void Start () {
        sr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        sr.color = GISparser.valueToHeatColor(value);

    }
}
