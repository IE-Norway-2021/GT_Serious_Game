/*
Class tile
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TileStack
{

    // Map position
    public float x;
    public float z;

    public List<GameObject> tiles = new List<GameObject>();

    public bool water;
    public bool tree;
    public bool grass;
    public bool building; //TODO
    public bool ground;
    public bool rock;
    public bool metal;
    public bool gold;
    public bool uranium;
    public bool volcano;
    public bool volcanoBorder;

    public TileStack(float x, float z)
    {
        this.x = x;
        this.z = z;
        rock = true;
    }

    public void addTile(GameObject tile)
    {
        tiles.Add(tile);
    }

    public override string ToString()
    {
        if (water)
        {
            return "This is some good water";
        }
        return "No water";
    }
}