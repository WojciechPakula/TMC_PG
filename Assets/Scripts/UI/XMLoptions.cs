using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XMLoptions : MonoBehaviour {
    public LayerMenu menu;
    public GISlayerOSMXML layer;
    public Slider sliderOpacity;
    public InputField input;
    // Use this for initialization
    void Start()
    {
        sliderOpacity.value = layer.getOpacity();
    }

    // Update is called once per frame
    void Update()
    {
        var val = sliderOpacity.value;  //od 0 do 1
        layer.setOpacity(val);
    }

    public void buttonClick()
    {
        layer.init(input.text);
    }
}
