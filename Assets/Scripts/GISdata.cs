using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISdata
{
    public double minLat { get; set; }
    public double maxLat { get; set; }
    public double minLon { get; set; }
    public double maxLon { get; set; }

    public List<GISway> wayContainer { get; set; }
    public List<GISnode> nodeContainer { get; set; }

    GISdata()
    {
        wayContainer = new List<GISway>();
        nodeContainer = new List<GISnode>();
    }
}
