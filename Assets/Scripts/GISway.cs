using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISway
{
    public long id { get; set; }
    public Dictionary<string, string> tags { get; set; }
    public List<GISnode> localNodeContainer { get; set; }
    public bool visible { get; set; }

    public bool closed
    {
        get
        {
            if (localNodeContainer != null && localNodeContainer.Count > 0)
            {
                GISnode first = localNodeContainer[0];
                GISnode last = localNodeContainer[localNodeContainer.Count - 1];
                return first.id == last.id;
            }
            else
            {
                return false;
            }
        }
    }

    public GISway(long id = 0)
    {
        this.id = id;
        localNodeContainer = new List<GISnode>();
        tags = new Dictionary<string, string>();
    }
}
