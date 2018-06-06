using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefaultOptions : MonoBehaviour {
    public LayerMenu menu;
    public GISlayer layer;
    public Slider sliderOpacity;
	// Use this for initialization
	void Start () {
        sliderOpacity.value = layer.getOpacity();
    }
	
	// Update is called once per frame
	void Update () {
        var val = sliderOpacity.value;  //od 0 do 1
        layer.setOpacity(val);
    }
}
