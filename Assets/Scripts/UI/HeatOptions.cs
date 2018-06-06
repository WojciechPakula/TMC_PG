using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatOptions : MonoBehaviour {
    public LayerMenu menu;
    public GISlayerHeatMap layer;
    public Slider sliderOpacity;
    public InputField input;
    public Text status;
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

        var coords = layer.map.logicCursorPosition;
        status.text = layer.updateStatus(coords);

    }

    public void buttonClick()
    {
        layer.init(input.text);
    }
}
