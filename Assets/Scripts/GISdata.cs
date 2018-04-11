using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class GISdata
{
    public double minLat { get; set; }
    public double maxLat { get; set; }
    public double minLon { get; set; }
    public double maxLon { get; set; }

    public List<GISway> wayContainer { get; set; }

    [XmlElement("node")]
    public List<GISnode> nodeContainer { get; set; }

    public GISdata()
    {
        wayContainer = new List<GISway>();
        nodeContainer = new List<GISnode>();
    }
}
