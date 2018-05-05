using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GISquadtree {
    public int max = 20;
    GISquadtree[] nodes = null;
    public int level = 0;
    GISquadtree parent = null;

    public Vector2d position;
    public Vector2d size;

    List<GISway> list = new List<GISway>();

    public GISquadtree(GISquadtree par)
    {       
        parent = par;
        if (parent != null)
        {
            level = parent.level + 1;
            max = parent.max;
        }
    }

    private void split()
    {
        if (nodes == null)
        {
            nodes = new GISquadtree[4];

            Vector2d halfsize = size / 2.0;

            nodes[0] = new GISquadtree(this);
            nodes[0].position = new Vector2d(position.x+ halfsize.x, position.y + halfsize.y);
            nodes[0].size = halfsize;
            nodes[1] = new GISquadtree(this);
            nodes[1].position = new Vector2d(position.x, position.y + halfsize.y);
            nodes[1].size = halfsize;
            nodes[2] = new GISquadtree(this);
            nodes[2].position = new Vector2d(position.x, position.y);
            nodes[2].size = halfsize;
            nodes[3] = new GISquadtree(this);
            nodes[3].position = new Vector2d(position.x + halfsize.x, position.y);
            nodes[3].size = halfsize;

        } else Debug.Log("WARNING - quadtree już podzielone");
    }

    public void insert(GISway element)
    {
        if (isLeaf())
        {
            //jezeli lisc
            list.Add(element);
            if (list.Count >= max)
            {
                //split
                split();
                //przenies elementy i usun liste
                foreach (var ele in list)
                {
                    insertIntoSection(ele);
                }
                list.Clear();
            }
        } else
        {
            //jezeli nie lisc
            //przenies element dalej
            insertIntoSection(element);
        }       
    }

    void insertIntoSection(GISway ele)
    {
        bool b0 = false, b1 = false, b2 = false, b3 = false;
        foreach (var node in ele.localNodeContainer)    //punkty obiektu
        {
            Vector2d tmp;

            tmp = nodes[0].position;
            tmp = node.XY - tmp;
            if (tmp.x >= 0 && tmp.y >= 0 && tmp.x < nodes[0].size.x && tmp.y < nodes[0].size.y) //ustalić co się dzieje na odcinkach/krawędziach //zrobiłem
            {
                b0 = true;
            }
            tmp = nodes[1].position;
            tmp = node.XY - tmp;
            if (tmp.x >= 0 && tmp.y >= 0 && tmp.x < nodes[0].size.x && tmp.y < nodes[1].size.y)
            {
                b0 = true;
            }
            tmp = nodes[2].position;
            tmp = node.XY - tmp;
            if (tmp.x >= 0 && tmp.y >= 0 && tmp.x < nodes[0].size.x && tmp.y < nodes[2].size.y)
            {
                b0 = true;
            }
            tmp = nodes[3].position;
            tmp = node.XY - tmp;
            if (tmp.x >= 0 && tmp.y >= 0 && tmp.x < nodes[0].size.x && tmp.y < nodes[3].size.y)
            {
                b0 = true;
            }
        }
        if (b0) nodes[0].insert(ele);
        if (b1) nodes[1].insert(ele);
        if (b2) nodes[2].insert(ele);
        if (b3) nodes[3].insert(ele);
    }

    public bool isLeaf()
    {
        if (nodes == null) return true;
        return false;
    }
}
