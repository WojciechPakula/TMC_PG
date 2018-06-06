using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerMenu : MonoBehaviour {
    public Dropdown listaRozwijana;
    public GameObject content;
    public GameObject extendedContainer;
    public GISmap3 map;

    public int selectedId = -1;

    public void buttonPlus()
    {
        string wybrane = listaRozwijana.captionText.text;
        Debug.Log(wybrane);
        GISlayer l = null;
        switch (wybrane)
        {
            case "Testowa":
                l = new GISlayerTest();
                break;
            case "Open Street Map":
                l = new GISlayerOSM();
                break;
            case "Bing":
                l = new GISlayerBING();
                break;
            case "OSMXML":
                l = new GISlayerOSMXML();
                break;
            case "HeatMap":
                l = new GISlayerHeatMap();
                break;
            case "2137":
                l = new GISlayer2137();
                break;
        }
        if (l != null)
        {
            l.map = map;
            map.insertLayerAt(l);
        }
        foreach (Transform element in content.transform)
        {
            var button = element.GetComponentInChildren<Button>();
            if (button != null)
            {
                var te = button.GetComponentInChildren<Text>();
                Debug.Log(te.text);
            }
        }
        
    }
    public void buttonMinus()
    {
        map.removeLayerById(selectedId);
        selectedId = -1;
        clearExtendedOptions();
    }
    public void buttonUp()
    {
        var but = getButtonById(selectedId);
        if (but != null)
        {
            var layers = map.getLayers();
            int index;
            var layer = map.getLayerById(selectedId, out index);
            if (layer != null)
            {
                if (layers.Count-1 > index)
                {
                    var tmp = layers[index + 1];
                    layers[index + 1] = layers[index];
                    layers[index] = tmp;
                    map.setLayers(layers);
                }
            }
        }
    }
    public void buttonDown()
    {
        var but = getButtonById(selectedId);
        if (but != null)
        {
            var layers = map.getLayers();
            int index;
            var layer = map.getLayerById(selectedId, out index);
            if (layer != null)
            {
                if (0 < index)
                {
                    var tmp = layers[index - 1];
                    layers[index - 1] = layers[index];
                    layers[index] = tmp;
                    map.setLayers(layers);
                }
            }           
        }
    }
    public void layerClick(int id)
    {
        //jakieś działanie związane z opcjami i zaznaczeniem warstwy
        if (selectedId != id)
        {
            selectedId = id;
            Debug.Log("Zaznaczono warstwę o id: " + id);
            insertExtendedOptions();
        }     
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var layers = map.getLayers();
        layers.Reverse();
        /*foreach (var layer in layers)
        {

        }*/
        bool fail = false;
        if (layers.Count == content.transform.childCount)
        {
            for (int i = 0; i < layers.Count; ++i)
            {
                var layer = layers[i];
                Transform tran = null;
                try
                {
                    tran = content.transform.GetChild(i);
                }
                catch
                {
                    fail = true;
                    break;
                }

                var button = tran.GetComponentInChildren<Button>();
                if (button == null) fail = true; else if (button.GetComponent<ButtonData>().id == layer.getId()) fail = false; else fail = true;
                if (fail) break;
            }
        } else
        {
            fail = true;
        }
        
        if (fail) warstwySieZmienily(layers);
    }

    void warstwySieZmienily(List<GISlayer> layers)
    {        
        int c = content.transform.childCount;
        //usun wszystkie przyciski
        for (int i = 0; i < c; ++i)
        {
            Transform tran = content.transform.GetChild(i);
            Destroy(tran.gameObject);
        }

        //stworz nowe przyciski
        for (int i = 0; i < layers.Count; ++i)
        {
            var button = Resources.Load("Prefabs/layerUI", typeof(GameObject));
            GameObject go = (GameObject)Instantiate(button);
            go.name = layers[i].getId().ToString();
            go.transform.SetParent(content.transform);
            //go.transform.parent = content.transform;
            var data = go.GetComponent<ButtonData>();
            data.id = layers[i].getId();
            data.layer = layers[i];
            data.menu = this;
        }

        var sel = getButtonById(selectedId);
        if (sel == null) odznaczWarstwe();
    }

    void odznaczWarstwe()
    {
        selectedId = -1;
    }

    Button getButtonById(int id)
    {
        foreach (Transform element in content.transform)
        {
            var button = element.GetComponentInChildren<Button>();           
            if (button != null)
            {
                var comp = button.GetComponent<ButtonData>();
                if (comp.id == id) return button;
            }
        }
        return null;
    }

    void insertExtendedOptions()
    {
        clearExtendedOptions();

        int index;
        var layer = map.getLayerById(selectedId, out index);
        if (layer != null)
        {
            if (layer is GISlayerOSMXML)
            {
                var ob = Resources.Load("Prefabs/XMLoptions", typeof(GameObject));
                GameObject go = (GameObject)Instantiate(ob, extendedContainer.transform.position, extendedContainer.transform.rotation);
                go.transform.SetParent(extendedContainer.transform);
                var opt = go.GetComponent<XMLoptions>();
                opt.layer = (GISlayerOSMXML)layer;
                opt.menu = this;
            } else if (layer is GISlayerHeatMap) {
                var ob = Resources.Load("Prefabs/HeatOptions", typeof(GameObject));
                GameObject go = (GameObject)Instantiate(ob, extendedContainer.transform.position, extendedContainer.transform.rotation);
                go.transform.SetParent(extendedContainer.transform);
                var opt = go.GetComponent<HeatOptions>();
                opt.layer = (GISlayerHeatMap)layer;
                opt.menu = this;
            } else
            {
                var ob = Resources.Load("Prefabs/defaultOptions", typeof(GameObject));
                GameObject go = (GameObject)Instantiate(ob, extendedContainer.transform.position, extendedContainer.transform.rotation);
                go.transform.SetParent(extendedContainer.transform);
                var opt = go.GetComponent<DefaultOptions>();
                opt.layer = layer;
                opt.menu = this;
            }           
        }        
    }

    void clearExtendedOptions()
    {
        int c = extendedContainer.transform.childCount;
        if (c > 0)
        {
            var ch = extendedContainer.transform.GetChild(0);
            Destroy(ch.gameObject);
        }
    }
}
