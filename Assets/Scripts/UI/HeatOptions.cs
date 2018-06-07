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
    public Dropdown dropDown;
    public Dropdown minmax;
    public Text tekstZadaniowy;
    public Toggle toggle;
    bool activation = false;
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

        if (toggle.isOn != activation)
        {
            if (toggle == true)
            {
                layer.setHeatActivated();
            } else
            {
                layer.setHeatDeactivated();
            }
            activation = toggle.isOn;
        }
        //= toggle.isOn
    }

    public void buttonClick()
    {
        layer.init(input.text);
        foreach (var ele in layer.dostepneNazwy)
        {
            Dropdown.OptionData od = new Dropdown.OptionData(ele.Value);
            dropDown.options.Add(od);
        }       
    }

    public void dodajZadanie()
    {
        int rodzaj = minmax.value;
        string wybrane = dropDown.captionText.text;

        Debug.Log(wybrane);

        layer.addAbility(wybrane, rodzaj);

        tekstZadaniowy.text += minmax.captionText.text +", "+ wybrane;
    }
}
