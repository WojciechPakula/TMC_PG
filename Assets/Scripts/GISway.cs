using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISway
{
    public long id { get; set; }
    public Dictionary<string, string> tags { get; set; }
    public List<GISnode> localNodeContainer { get; set; }

    public int uid { get; set; }
    public string user { get; set; }

    public DateTime timestamp { get; set; }
    public long changeset { get; set; }
    public int version { get; set; }
    public bool visible { get; set; }

    GISway(long id = 0)
    {
        this.id = id;
        localNodeContainer = new List<GISnode>();
        tags = new Dictionary<string, string>();
    }
}
