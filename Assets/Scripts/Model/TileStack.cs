/*
Class tile
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public bool exists;
    public List<GameObject> tilesObject;

    public Tile()
    {
        exists = false;
        tilesObject = new List<GameObject>();
    }

    public void highlightTile()
    {
        // Highlights only the highest tile
        if (tilesObject.Count > 0)
        {
            tilesObject[tilesObject.Count - 1].GetComponent<Outline>().enabled = true;
        }
    }
}

[System.Serializable]
public class TileStack
{

    // Map position
    public float x;
    public float y;
    public float z;

    // Array position
    public int xIndex;
    public int zIndex;

    public Tile water;
    public Tile tree;
    public Tile grass;
    public Tile building;
    public Tile ground;
    public Tile rock;
    public Tile metal;
    public Tile gold;
    public Tile uranium;
    public Tile volcano;
    public Tile volcanoBorder;

    // Building 

    public Tile metalMine;
    public Tile goldMine;
    public Tile uraniumMine;
    public Tile nuclearPlant;
    public Tile pipeline;
    public Tile hotel;

    public TileStack(float x, float y, float z, int xIndex, int zIndex)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.xIndex = xIndex;
        this.zIndex = zIndex;
        // instantiate every Tile
        water = new Tile();
        tree = new Tile();
        grass = new Tile();
        building = new Tile();
        ground = new Tile();
        rock = new Tile();
        metal = new Tile();
        gold = new Tile();
        uranium = new Tile();
        volcano = new Tile();
        volcanoBorder = new Tile();
        metalMine = new Tile();
        goldMine = new Tile();
        uraniumMine = new Tile();
        nuclearPlant = new Tile();
        pipeline = new Tile();
        hotel = new Tile();

        rock.exists = true;
    }

    public void addTile(GameObject tile, TileType type)
    {
        switch (type)
        {
            case TileType.water:
                water.tilesObject.Add(tile);
                break;
            case TileType.tree:
                tree.tilesObject.Add(tile);
                break;
            case TileType.grass:
                grass.tilesObject.Add(tile);
                break;
            case TileType.ground:
                ground.tilesObject.Add(tile);
                break;
            case TileType.rock:
                rock.tilesObject.Add(tile);
                break;
            case TileType.metal:
                metal.tilesObject.Add(tile);
                break;
            case TileType.gold:
                gold.tilesObject.Add(tile);
                break;
            case TileType.uranium:
                uranium.tilesObject.Add(tile);
                break;
            case TileType.volcano:
                volcano.tilesObject.Add(tile);
                break;
            case TileType.volcanoBorder:
                volcanoBorder.tilesObject.Add(tile);
                break;
            case TileType.metalMine:
                metalMine.tilesObject.Add(tile);
                metalMine.exists = true;
                break;
            case TileType.goldMine:
                goldMine.tilesObject.Add(tile);
                goldMine.exists = true;
                break;
            case TileType.uraniumMine:
                uraniumMine.tilesObject.Add(tile);
                uraniumMine.exists = true;
                break;
            case TileType.nuclearPlant:
                nuclearPlant.tilesObject.Add(tile);
                nuclearPlant.exists = true;
                break;
            case TileType.pipeline:
                pipeline.tilesObject.Add(tile);
                pipeline.exists = true;
                break;
            case TileType.hotel:
                hotel.tilesObject.Add(tile);
                hotel.exists = true;
                break;
            default:
                break;
        }

    }

    public override string ToString()
    {
        string result = "TileStack: " + x + " " + y + " " + z + " " + xIndex + " " + zIndex + " ";
        return result;
    }
}