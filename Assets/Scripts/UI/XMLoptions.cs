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
    bool jp2lock = false;
    public void buttonClick()
    {
        if (input.text == "zawadiaka" && jp2lock == false)
        {           
            Dropdown.OptionData od = new Dropdown.OptionData("2137");
            menu.listaRozwijana.options.Add(od);
            jp2lock = true;
        } else
            layer.init(input.text);
    }
}
