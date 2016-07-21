using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Level
{
    public int height;
    public Layer[] layers;
    public int nextobjectid;
    public string orientation;
    public string renderorder;
    public int tileheight;
    public Tileset[] tilesets;
    public int tilewidth;
    public string version;
    public int width;
}

[Serializable]
public class Layer
{
    public int[] data;
    public int height;
    public string name;
    public float opacity;
    public string type;
    public bool visible;
    public int width;
    public int x;
    public int y;
}

[Serializable]
public class Tileset
{
    public int columns;
    public string image;
    public int imageheight;
    public int imagewidth;
    public int margin;
    public string name;
    public int spacing;
    public int tilecount;
    public int tileheight;
    public int tilewidth;
}

[Serializable]
public class Property
{

}

