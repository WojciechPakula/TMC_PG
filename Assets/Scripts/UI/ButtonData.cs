using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonData : MonoBehaviour {
    public GISlayer layer = null;
    public LayerMenu menu;
    Button button;
    public int id;
	// Use this for initialization
	void Start () {
        button = this.GetComponent<Button>();
        button.onClick.AddListener(TaskOnClick);
        if (layer != null) {
            button.GetComponentInChildren<Text>().text = layer.getId() + "-" +layer.getTypeName();
            id = layer.getId();
        }
	}

    void TaskOnClick()
    {
        if (layer != null)
            menu.layerClick(layer.getId());
    }

    // Update is called once per frame
    void Update () {
		
	}
}
