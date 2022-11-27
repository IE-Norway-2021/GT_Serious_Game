/*
Loads the island from the json file
The json file contains a 3D array that describes the island using the different tile types
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TileType
{ //the values that are stored in the json file
    water = 0,
    tree = 1,
    grass = 2,
    ground = 3,
    rock = 4, // unbuildable
    metal = 5,
    gold = 6,
    uranium = 7,
    none = 8
};

public class GameManager : MonoBehaviour
{
    public List<List<TileStack>> tileStacks = new List<List<TileStack>>();

    public GameObject waterTilePrefab;
    public GameObject treeTilePrefab;
    public GameObject grassTilePrefab;
    public GameObject groundTilePrefab;
    public GameObject rockTilePrefab;
    public GameObject metalTilePrefab;
    public GameObject goldTilePrefab;
    public GameObject uraniumTilePrefab;
    public GameObject volcanoTilePrefab;

    public Transform tileHolder;

    private const float TILE_HEIGHT_DEFAULT = 1f;
    private const float TILE_X_DEFAULT = 1.7f;
    private const float TILE_Z_DEFAULT = 1.5f;
    private const float TILE_X_OFFSET = 0.4f;

    // camera control

    public CameraMovement cameraMovement;

    public InputManager inputManager;

    public TileOnClick tileOnClick;

    public MapData mapData;

    // Start is called before the first frame update
    void Start()
    {

        // camera control 
        //inputManager.OnMouseClick += HandleMouseClick;
        tileOnClick.OnTileClick += HandleMouseClick;



        Debug.Log("Start");
        GenerateMap();

        InstantiateIsland();

        Debug.Log("Island created");
        // Set center to position 0,0,0 of the map
        //tileHolder.position = new Vector3(-tileStacks.Count * TILE_X_DEFAULT / 2, 0, -tileStacks[0].Count * TILE_Z_DEFAULT / 2);
        tileHolder.position = new Vector3(0, 0, 0);

    }

    // Update is called once per frame
    void Update()
    {
        // camera control
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x, 0, inputManager.CameraMovementVector.y));
    }

    // Generates a random map of TileStacks
    public void GenerateMap()
    {
        // Create an list of tile stacks using the dims in the mapData
        float x = (mapData.XLimit / 2) * -TILE_X_DEFAULT, z = (mapData.ZLimit / 2) * -TILE_Z_DEFAULT;
        for (int i = 0; i < mapData.XLimit; i++, x += TILE_X_DEFAULT)
        {
            tileStacks.Add(new List<TileStack>());
            for (int j = 0; j < mapData.ZLimit; j++, z += TILE_Z_DEFAULT)
            {
                float decalage = 0;
                if (j % 2 == 0)
                {
                    decalage = TILE_X_OFFSET;
                }
                else
                {
                    decalage = -TILE_X_OFFSET;
                }
                tileStacks[i].Add(new TileStack(x + decalage, z));
            }
            z = (mapData.ZLimit / 2) * -TILE_Z_DEFAULT;
        }


        // handle the volcano
        float VolcanoXLimit = mapData.VolcanoRadius * TILE_X_DEFAULT, VolcanoZLimit = mapData.VolcanoRadius * TILE_Z_DEFAULT;
        for (int i = 0; i < mapData.XLimit; i++)
        {
            for (int j = 0; j < mapData.ZLimit; j++)
            {
                float decalage = 0;
                if (j % 2 == 0)
                {
                    decalage = TILE_X_OFFSET;
                }
                else
                {
                    decalage = -TILE_X_OFFSET;
                }
                if (isOnCircleBorder(tileStacks[i][j].x, tileStacks[i][j].z, VolcanoXLimit + decalage, VolcanoZLimit))
                {
                    tileStacks[i][j].volcanoBorder = true;
                }
                else if (isInCircle(tileStacks[i][j].x, tileStacks[i][j].z, VolcanoXLimit + decalage, VolcanoZLimit))
                {
                    tileStacks[i][j].volcano = true;
                }
            }
        }


        // handle the water
        float IslandXLimit = mapData.IslandRadius * TILE_X_DEFAULT, IslandZLimit = mapData.IslandRadius * TILE_Z_DEFAULT;
        for (int i = 0; i < mapData.XLimit; i++)
        {
            for (int j = 0; j < mapData.ZLimit; j++)
            {
                float decalage = 0;
                if (j % 2 == 0)
                {
                    decalage = TILE_X_OFFSET;
                }
                else
                {
                    decalage = -TILE_X_OFFSET;
                }
                if (!isInCircle(tileStacks[i][j].x, tileStacks[i][j].z, IslandXLimit + decalage, IslandZLimit))
                {
                    tileStacks[i][j].water = true;
                }
            }
        }

        // handle the rest
        for (int i = 0; i < mapData.XLimit; i++)
        {
            for (int j = 0; j < mapData.ZLimit; j++)
            {
                if (!tileStacks[i][j].water && !tileStacks[i][j].volcano && !tileStacks[i][j].volcanoBorder)
                {
                    tileStacks[i][j].ground = true;
                    tileStacks[i][j].grass = true;
                    if (UnityEngine.Random.Range(0, 100) < mapData.TreeProbability)
                    {
                        tileStacks[i][j].tree = true;
                    }
                    if (UnityEngine.Random.Range(0, 100) < mapData.MineralProbability)
                    {
                        int mineral = UnityEngine.Random.Range(0, 100) % 6;
                        switch (mineral)
                        {
                            case 0:
                            case 1:
                            case 2:
                                tileStacks[i][j].metal = true;
                                break;
                            case 3:
                            case 4:
                                tileStacks[i][j].gold = true;
                                break;
                            case 5:
                                tileStacks[i][j].uranium = true;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

    private bool isInCircle(float x, float z, float xLimit, float zLimit)
    {
        return (x * x) / (xLimit * xLimit) + (z * z) / (zLimit * zLimit) < 2;
    }

    private bool isOnCircleBorder(float x, float z, float xLimit, float zLimit)
    {
        float value = (x * x) / (xLimit * xLimit) + (z * z) / (zLimit * zLimit);
        return value >= 1.8 && value <= 2.6;
    }

    // Instantiates the map
    public void InstantiateIsland()
    {
        for (int i = 0; i < tileStacks.Count; i++)
        {
            for (int j = 0; j < tileStacks[i].Count; j++)
            {
                createTile(tileStacks[i][j].x, tileStacks[i][j].z, tileStacks[i][j]);
            }
        }
    }

    private GameObject createTile(float x, float z, TileStack tileStack)
    {
        //Couche de base
        tileStack.addTile(InstantiateObject(rockTilePrefab, x, -TILE_HEIGHT_DEFAULT, z, 6));
        // stack the tiles depending on the tileStack
        if (tileStack.water)
        {
            GameObject tile = InstantiateObject(waterTilePrefab, x, 0, z, 4);
            tileStack.addTile(tile);
        }
        else if (tileStack.ground)
        {
            Debug.Log("Is ground");
            // TODO : add minerals
            tileStack.addTile(InstantiateObject(groundTilePrefab, x, 0, z, 6));
            if (tileStack.grass)
            {
                tileStack.addTile(InstantiateObject(grassTilePrefab, x, TILE_HEIGHT_DEFAULT, z, 6));
                if (tileStack.tree)
                {
                    tileStack.addTile(InstantiateObject(treeTilePrefab, x, 1.5f, z, 6));
                }
            }
        }
        else if (tileStack.volcano)
        {
            for (int i = 0; i < mapData.VolcanoHeight - 1; i++)
            {
                tileStack.addTile(InstantiateObject(volcanoTilePrefab, x, i * TILE_HEIGHT_DEFAULT, z, 6));
            }
        }
        else if (tileStack.volcanoBorder)
        {
            for (int i = 0; i < mapData.VolcanoHeight; i++)
            {
                tileStack.addTile(InstantiateObject(rockTilePrefab, x, i * TILE_HEIGHT_DEFAULT, z, 6));
            }
        }
        return null;
    }

    private GameObject InstantiateObject(GameObject prefab, float x, float y, float z, int layer)
    {
        GameObject tmpTile = Instantiate(prefab);
        tmpTile.transform.position = new Vector3(x, y, z);
        tmpTile.transform.SetParent(tileHolder);
        // set layer to Base
        tmpTile.layer = layer;
        return tmpTile;
    }


    private void HandleMouseClick(GameObject clickedObject)
    {
        if (clickedObject != null)
        {
            Debug.Log("Clicked on " + clickedObject.name);
            // log position
            Debug.Log("Position : " + clickedObject.transform.position);
        }
    }




}