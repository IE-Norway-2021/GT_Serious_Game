/*
Loads the island from the json file
The json file contains a 3D array that describes the island using the different tile types
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVS;
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


    // create a List with an array of the different tile types
    public TileType[,,] tilesLoaded = new TileType[,,] { { { TileType.water,TileType.none,TileType.none,TileType.none }, { TileType.water,TileType.none,TileType.none,TileType.none }, { TileType.water,TileType.none,TileType.none,TileType.none }, { TileType.water,TileType.none,TileType.none,TileType.none } },
    { { TileType.water,TileType.none,TileType.none,TileType.none }, { TileType.water,TileType.none,TileType.none,TileType.none }, { TileType.water,TileType.none,TileType.none,TileType.none }, { TileType.water,TileType.none,TileType.none,TileType.none } },
    { { TileType.water,TileType.none,TileType.none,TileType.none }, { TileType.water,TileType.none,TileType.none,TileType.none }, { TileType.water,TileType.none,TileType.none,TileType.none }, { TileType.water,TileType.none,TileType.none,TileType.none } },
    { { TileType.rock, TileType.ground, TileType.grass,TileType.tree }, { TileType.rock,TileType.ground,TileType.none,TileType.none }, { TileType.rock,TileType.ground,TileType.none,TileType.none }, { TileType.rock,TileType.ground,TileType.none,TileType.none } } };

    public List<List<TileStack>> tileStacks = new List<List<TileStack>>();

    public GameObject waterTilePrefab;
    public GameObject treeTilePrefab;
    public GameObject grassTilePrefab;
    public GameObject groundTilePrefab;
    public GameObject rockTilePrefab;
    public GameObject metalTilePrefab;
    public GameObject goldTilePrefab;
    public GameObject uraniumTilePrefab;

    public Transform tileHolder;

    private const float TILE_HEIGHT_DEFAULT = 1f;



    // Start is called before the first frame update
    void Start()
    {

        // camera control 
        inputManager.OnMouseClick += HandleMouseClick;



        Debug.Log("Start");
        loadIsland();

        Debug.Log("TileStacks created");

        createIsland();

        Debug.Log("Island created");
    }

    // Update is called once per frame
    void Update()
    {
        // camera control
        cameraMovement.MoveCamera(new Vector3(inputManager.CameraMovementVector.x, 0, inputManager.CameraMovementVector.y));


    }

    public void loadIsland()
    {
        //Load json file
        //TODO

        //Create the tile stacks from the tiles loaded
        for (int x = 0; x < tilesLoaded.GetLength(0); x++)
        {
            tileStacks.Add(new List<TileStack>());
            for (int y = 0; y < tilesLoaded.GetLength(1); y++)
            {
                //create a 1D array of the TileType in position x,y
                TileType[] tmp = new TileType[tilesLoaded.GetLength(2)];
                for (int z = 0; z < tilesLoaded.GetLength(2); z++)
                {
                    tmp[z] = tilesLoaded[x, y, z];
                }
                tileStacks[x].Add(new TileStack(tmp));

            }
        }
    }

    public void createIsland()
    {
        float x = 0, z = 0;
        for (int i = 0; i < tileStacks.Count; i++, x += 1.7f)
        {
            for (int j = 0; j < tileStacks[i].Count; j++, z += 1.5f)
            {
                float decalage = 0;
                if (j % 2 == 0)
                {
                    decalage = 0.4f;
                }
                else
                {
                    decalage = -0.4f;
                }
                createTile(x + decalage, z, tileStacks[i][j]);
            }
            z = 0;
        }
    }

    private GameObject createTile(float x, float z, TileStack tileStack)
    {
        // stack the tiles depending on the tileStack
        if (tileStack.water)
        {
            return InstantiateObject(waterTilePrefab, x, 0, z, 4);
        }
        else
        {
            Debug.Log("Is ground");
            //Couche de base
            InstantiateObject(rockTilePrefab, x, -TILE_HEIGHT_DEFAULT, z, 6);
            if (tileStack.ground)
            {
                InstantiateObject(groundTilePrefab, x, 0, z, 6);
                if (tileStack.grass)
                {
                    InstantiateObject(grassTilePrefab, x, TILE_HEIGHT_DEFAULT, z, 6);
                    if (tileStack.tree)
                    {
                        InstantiateObject(treeTilePrefab, x, 1.5f, z, 6);
                    }
                }
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


    // camera control

    public CameraMovement cameraMovement;

    public InputManager inputManager;

    private void HandleMouseClick(Vector3Int position)
    {
        Debug.Log(position);
    }





}