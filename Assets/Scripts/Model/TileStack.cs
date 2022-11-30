/*
Class tile
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public bool exists;
    public GameObject tileObject;

    public Tile()
    {
        exists = false;
        tileObject = null;
    }
}

[System.Serializable]
public class TileStack
{

    // Map position
    public float x;
    public float z;

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

    public TileStack(float x, float z)
    {
        this.x = x;
        this.z = z;
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
                water.tileObject = tile;
                break;
            case TileType.tree:
                tree.tileObject = tile;
                break;
            case TileType.grass:
                grass.tileObject = tile;
                break;
            case TileType.ground:
                ground.tileObject = tile;
                break;
            case TileType.rock:
                rock.tileObject = tile;
                break;
            case TileType.metal:
                metal.tileObject = tile;
                break;
            case TileType.gold:
                gold.tileObject = tile;
                break;
            case TileType.uranium:
                uranium.tileObject = tile;
                break;
            case TileType.volcano:
                // TODO : implement volcano as a list of tiles
                break;
            case TileType.volcanoBorder:
                // TODO : implement volcano border as a list of tiles 
                break;
            case TileType.metalMine:
                metalMine.tileObject = tile;
                metalMine.exists = true;
                break;
            case TileType.goldMine:
                goldMine.tileObject = tile;
                goldMine.exists = true;
                break;
            case TileType.uraniumMine:
                uraniumMine.tileObject = tile;
                uraniumMine.exists = true;
                break;
            case TileType.nuclearPlant:
                nuclearPlant.tileObject = tile;
                nuclearPlant.exists = true;
                break;
            case TileType.pipeline:
                pipeline.tileObject = tile;
                pipeline.exists = true;
                break;
            case TileType.hotel:
                hotel.tileObject = tile;
                hotel.exists = true;
                break;
            default:
                break;
        }

    }

    public override string ToString()
    {
        if (water.exists)
        {
            return "This is some good water";
        }
        return "No water";
    }
}