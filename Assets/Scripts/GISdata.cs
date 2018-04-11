using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISdata
{
    //+jakieś dane o obszerze(xd) 
    //czyli to co jest w węźle bounds w pliku OSM

    public List<GISway> wayContainer { get; set; }
    public List<GISnode> nodeContainer { get; set; }

    GISdata()
    {
        wayContainer = new List<GISway>();
        nodeContainer = new List<GISnode>();
    }
}
