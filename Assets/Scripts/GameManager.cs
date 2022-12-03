/*
Loads the island from the json file
The json file contains a 3D array that describes the island using the different tile types
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

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
    none = 8,
    volcano = 9,
    volcanoBorder = 10,
    // Buildings
    metalMine = 11,
    goldMine = 12,
    uraniumMine = 13,
    nuclearPlant = 14,
    pipeline = 15,
    hotel = 16,
};

public enum UserActionType
{
    none = 0,
    cutTree = 1,
    removeGrass = 2,
    digGround = 3,
    buildMetalMine = 4,
    buildGoldMine = 5,
    buildUraniumMine = 6,
    buildNuclearPlant = 7,
    buildPipeline = 8,
    buildHotel = 9,
};

public enum GameState
{ //TODO a completer
    startState = 0, // au début du lancement du jeu
    gameOnGoing = 1, // après le lancement d'une partie
    gameDone = 2, //la partie est finie
}


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
    public GameObject metalMineTilePrefab;
    public GameObject goldMineTilePrefab;
    public GameObject uraniumMineTilePrefab;
    public GameObject nuclearPlantTilePrefab;
    public GameObject pipelineTilePrefab;

    public Transform tileHolder;

    private const float TILE_HEIGHT_DEFAULT = 1f;
    private const float TILE_X_DEFAULT = 1.7f;
    private const float TILE_Z_DEFAULT = 1.5f;
    private const float TILE_X_OFFSET = 0.4f;

    // camera control
    public TileOnClick tileOnClick;

    public GameSettings gameSettings;

    // Action management

    public UserActionType currentAction = UserActionType.none;

    // Counters for all the resources and various other things

    // Resources : 
    public long money = 0;
    public long metalCount = 0;

    public long goldCount = 0;

    public long uraniumCount = 0;

    public long moneyCount = 0;

    // Buildings :

    public long metalMineCount = 40;
    public long goldMineCount = 0;
    public long uraniumMineCount = 0;
    public long nuclearPlantCount = 0;
    public long pipelineCount = 0;

    // CO2 related :

    public long co2Count = 0;

    public long treeCount = 0;

    // Time related :
    public long timeCount = 0;

    // Events for the UI

    public Action<long, long, long> onUpdateDone;

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

        // Set the starting money
        moneyCount = gameSettings.startingMoney;

        //TODO : pour l'instant on lance le jeu direct, a enlever
        startGame();

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void startGame() //TODO appeler cette fonction quand on lance le jeu
    {
        // TODO : se baser sur l'état du jeu pour savoir si on peut lancer une partie
        // Reset time
        timeCount = 0;
        InvokeRepeating("GameStateUpdate", 0, gameSettings.updateDelay);
    }

    private void GameStateUpdate()
    {
        // Compute the CO2 added
        long co2Added = 0;
        co2Added += metalMineCount * gameSettings.metalMineCO2Production;
        co2Added += goldMineCount * gameSettings.goldMineCO2Production;
        co2Added += uraniumMineCount * gameSettings.uraniumMineCO2Production;
        co2Added += nuclearPlantCount * gameSettings.nuclearPlantCO2Production;
        co2Added += pipelineCount * gameSettings.pipelineCO2Production;
        // Remove the CO2 taken by the trees
        co2Added -= treeCount * gameSettings.treeCO2Decrease;
        // Compute the money added
        long moneyAdded = 0;
        moneyAdded += metalMineCount * gameSettings.metalMineMoneyProduction;
        moneyAdded += goldMineCount * gameSettings.goldMineMoneyProduction;
        moneyAdded += uraniumMineCount * gameSettings.uraniumMineMoneyProduction;
        // Compute the increase for every nuclear plant present
        if (nuclearPlantCount > 0)
        {
            moneyAdded += moneyAdded * gameSettings.nuclearPlantMoneyIncrease * nuclearPlantCount;
        }

        // Update the counters
        co2Count += co2Added;
        if (co2Count < 0) // if trees remove too much
        {
            co2Count = 0;
        }

        moneyCount += moneyAdded;

        // Update the time
        timeCount += gameSettings.updateDelay;

        // Update the UI
        onUpdateDone?.Invoke(co2Count, moneyCount, timeCount);
        

        Debug.Log("CO2 : " + co2Count + " Money : " + moneyCount + " Time : " + timeCount);
    }

    // Generates a random map of TileStacks
    public void GenerateMap()
    {
        // Create an list of tile stacks using the dims in the mapData
        float x = (gameSettings.XLimit / 2) * -TILE_X_DEFAULT, z = (gameSettings.ZLimit / 2) * -TILE_Z_DEFAULT;
        for (int i = 0; i < gameSettings.XLimit; i++, x += TILE_X_DEFAULT)
        {
            tileStacks.Add(new List<TileStack>());
            for (int j = 0; j < gameSettings.ZLimit; j++, z += TILE_Z_DEFAULT)
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
            z = (gameSettings.ZLimit / 2) * -TILE_Z_DEFAULT;
        }


        // handle the volcano
        float VolcanoXLimit = gameSettings.VolcanoRadius * TILE_X_DEFAULT, VolcanoZLimit = gameSettings.VolcanoRadius * TILE_Z_DEFAULT;
        for (int i = 0; i < gameSettings.XLimit; i++)
        {
            for (int j = 0; j < gameSettings.ZLimit; j++)
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
                    tileStacks[i][j].volcanoBorder.exists = true;
                }
                else if (isInCircle(tileStacks[i][j].x, tileStacks[i][j].z, VolcanoXLimit + decalage, VolcanoZLimit))
                {
                    tileStacks[i][j].volcano.exists = true;
                }
            }
        }


        // handle the water
        float IslandXLimit = gameSettings.IslandRadius * TILE_X_DEFAULT, IslandZLimit = gameSettings.IslandRadius * TILE_Z_DEFAULT;
        for (int i = 0; i < gameSettings.XLimit; i++)
        {
            for (int j = 0; j < gameSettings.ZLimit; j++)
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
                    tileStacks[i][j].water.exists = true;
                }
            }
        }

        // handle the rest
        for (int i = 0; i < gameSettings.XLimit; i++)
        {
            for (int j = 0; j < gameSettings.ZLimit; j++)
            {
                if (!tileStacks[i][j].water.exists && !tileStacks[i][j].volcano.exists && !tileStacks[i][j].volcanoBorder.exists)
                {
                    tileStacks[i][j].ground.exists = true;
                    tileStacks[i][j].grass.exists = true;
                    if (UnityEngine.Random.Range(0, 100) < gameSettings.TreeProbability)
                    {
                        tileStacks[i][j].tree.exists = true;
                        ++treeCount;
                    }
                    if (UnityEngine.Random.Range(0, 100) < gameSettings.MineralProbability)
                    {
                        int mineral = UnityEngine.Random.Range(0, 100) % 6;
                        switch (mineral)
                        {
                            case 0:
                            case 1:
                            case 2:
                                tileStacks[i][j].metal.exists = true;
                                break;
                            case 3:
                            case 4:
                                tileStacks[i][j].gold.exists = true;
                                break;
                            case 5:
                                tileStacks[i][j].uranium.exists = true;
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
        tileStack.addTile(InstantiateObject(rockTilePrefab, x, -TILE_HEIGHT_DEFAULT, z, 6), TileType.rock);
        // stack the tiles depending on the tileStack
        if (tileStack.water.exists)
        {
            GameObject tile = InstantiateObject(waterTilePrefab, x, 0, z, 4);
            tileStack.addTile(tile, TileType.water);
        }
        else if (tileStack.ground.exists)
        {
            // TODO : add minerals
            tileStack.addTile(InstantiateObject(groundTilePrefab, x, 0, z, 6), TileType.ground);
            if (tileStack.grass.exists)
            {
                tileStack.addTile(InstantiateObject(grassTilePrefab, x, TILE_HEIGHT_DEFAULT, z, 6), TileType.grass);
                if (tileStack.tree.exists)
                {
                    tileStack.addTile(InstantiateObject(treeTilePrefab, x, 1.5f, z, 6), TileType.tree);
                }
            }
        }
        else if (tileStack.volcano.exists)
        {
            for (int i = 0; i < gameSettings.VolcanoHeight - 1; i++)
            {
                tileStack.addTile(InstantiateObject(volcanoTilePrefab, x, i * TILE_HEIGHT_DEFAULT, z, 6), TileType.volcano);
            }
        }
        else if (tileStack.volcanoBorder.exists)
        {
            for (int i = 0; i < gameSettings.VolcanoHeight; i++)
            {
                tileStack.addTile(InstantiateObject(rockTilePrefab, x, i * TILE_HEIGHT_DEFAULT, z, 6), TileType.volcanoBorder);
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
        TileStack tileStack = getTileStackFromPosition(clickedObject.transform.position);
        if (tileStack == null)
        {
            Debug.Log("No tileStack found, panic now");
        }
        else
        {
            // Do action depending on the button clicked
            handleAction(tileStack);

            // highlight the tile
            toggleOutline(tileStack);
        }
    }

    private TileStack getTileStackFromPosition(Vector3 position)
    {
        // Do brute force search
        for (int i = 0; i < tileStacks.Count; i++)
        {
            for (int j = 0; j < tileStacks[i].Count; j++)
            {
                if (tileStacks[i][j].x == position.x && tileStacks[i][j].z == position.z)
                {
                    return tileStacks[i][j];
                }
            }
        }
        return null;
    }

    private void handleAction(TileStack tileStack)
    {
        switch (currentAction)
        {
            case UserActionType.cutTree:
                if (tileStack.tree.exists)
                {
                    tileStack.tree.exists = false;
                    tileStack.tree.tileObject.SetActive(false);
                    --treeCount;
                }
                break;
            case UserActionType.removeGrass:
                if (tileStack.grass.exists && !tileStack.tree.exists)
                {
                    tileStack.grass.exists = false;
                    tileStack.grass.tileObject.SetActive(false);
                }
                break;
            case UserActionType.digGround:
                if (tileStack.ground.exists && !tileStack.grass.exists && !tileStack.tree.exists)
                {
                    tileStack.ground.exists = false;
                    tileStack.ground.tileObject.SetActive(false);
                }
                break;
            case UserActionType.buildMetalMine:
                if (isBuildable(currentAction, tileStack))
                {
                    // Disable metal tile
                    tileStack.metal.exists = false;
                    tileStack.metal.tileObject.SetActive(false);
                    // Add mine
                    tileStack.addTile(InstantiateObject(metalMineTilePrefab, tileStack.x, 0, tileStack.z, 6), TileType.metalMine);
                    ++metalMineCount;
                }
                break;
            case UserActionType.buildGoldMine:
                if (isBuildable(currentAction, tileStack))
                {
                    // Disable gold tile
                    tileStack.gold.exists = false;
                    tileStack.gold.tileObject.SetActive(false);
                    // Add mine
                    tileStack.addTile(InstantiateObject(goldMineTilePrefab, tileStack.x, 0, tileStack.z, 6), TileType.goldMine);
                    ++goldMineCount;
                }
                break;
            case UserActionType.buildUraniumMine:
                if (isBuildable(currentAction, tileStack))
                {
                    // Disable uranium tile
                    tileStack.uranium.exists = false;
                    tileStack.uranium.tileObject.SetActive(false);
                    // Add mine
                    tileStack.addTile(InstantiateObject(uraniumMineTilePrefab, tileStack.x, 0, tileStack.z, 6), TileType.uraniumMine);
                    ++uraniumMineCount;
                }
                break;
            case UserActionType.buildNuclearPlant:
                if (isBuildable(currentAction, tileStack))
                {
                    // Disable uranium tile
                    tileStack.uranium.exists = false;
                    tileStack.uranium.tileObject.SetActive(false);
                    // Add mine
                    tileStack.addTile(InstantiateObject(nuclearPlantTilePrefab, tileStack.x, 0, tileStack.z, 6), TileType.nuclearPlant);
                    ++nuclearPlantCount;
                }
                break;
            case UserActionType.buildPipeline:
                if (isBuildable(currentAction, tileStack))
                {
                    // Disable uranium tile
                    tileStack.uranium.exists = false;
                    tileStack.uranium.tileObject.SetActive(false);
                    // Add mine
                    tileStack.addTile(InstantiateObject(pipelineTilePrefab, tileStack.x, 0, tileStack.z, 6), TileType.pipeline);
                    ++pipelineCount;
                }
                break;
            case UserActionType.none:
            default:
                return;
        }
        currentAction = UserActionType.none;
    }

    private bool isBuildable(UserActionType actionType, TileStack tileStack)
    {
        if (!tileStack.tree.exists && !tileStack.grass.exists && !tileStack.ground.exists)
        {
            switch (actionType)
            {
                case UserActionType.buildMetalMine:
                    return tileStack.metal.exists;
                case UserActionType.buildGoldMine:
                    return tileStack.gold.exists;
                case UserActionType.buildUraniumMine:
                    return tileStack.uranium.exists;
                case UserActionType.buildNuclearPlant:
                case UserActionType.buildPipeline:
                    return true;
                default:
                    return false;
            }
        }
        return false;
    }

    public void toggleOutline(TileStack tileStack)
    {
        // Disable the outline for all tiles
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<Outline>().enabled = false;
        }
        // Outline the highest tile
        if (tileStack.tree.exists)
        {
            tileStack.tree.tileObject.GetComponent<Outline>().enabled = true;
        }
        else if (tileStack.grass.exists)
        {
            tileStack.grass.tileObject.GetComponent<Outline>().enabled = true;
        }
        else if (tileStack.ground.exists)
        {
            tileStack.ground.tileObject.GetComponent<Outline>().enabled = true;
        }
        else if (tileStack.metal.exists)
        {
            tileStack.metal.tileObject.GetComponent<Outline>().enabled = true;
        }
        else if (tileStack.gold.exists)
        {
            tileStack.gold.tileObject.GetComponent<Outline>().enabled = true;
        }
        else if (tileStack.uranium.exists)
        {
            tileStack.uranium.tileObject.GetComponent<Outline>().enabled = true;
        }
        else if (tileStack.metalMine.exists)
        {
            tileStack.metalMine.tileObject.GetComponent<Outline>().enabled = true;
        }
        else if (tileStack.goldMine.exists)
        {
            tileStack.goldMine.tileObject.GetComponent<Outline>().enabled = true;
        }
        else if (tileStack.uraniumMine.exists)
        {
            tileStack.uraniumMine.tileObject.GetComponent<Outline>().enabled = true;
        }
        else if (tileStack.nuclearPlant.exists)
        {
            tileStack.nuclearPlant.tileObject.GetComponent<Outline>().enabled = true;
        }
        else if (tileStack.pipeline.exists)
        {
            tileStack.pipeline.tileObject.GetComponent<Outline>().enabled = true;
        }
        else if (tileStack.hotel.exists)
        {
            tileStack.hotel.tileObject.GetComponent<Outline>().enabled = true;
        }
    }


}