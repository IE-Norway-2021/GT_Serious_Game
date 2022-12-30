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
    removeTree = 1,
    removeGrass = 2,
    removeGround = 3,
    buildMetalMine = 4,
    buildGoldMine = 5,
    buildUraniumMine = 6,
    buildNuclearPlant = 7,
    buildPipeline = 8,
    buildHotel = 9,
    dig = 10,
};

public enum GameState
{ //TODO a completer
    startState = 0, // au début du lancement du jeu
    gameOnGoing = 1, // après le lancement d'une partie
    gameOver = 2, //la partie est finie
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

    public long metalMineCount = 0;

    public long goldMineCount = 0;

    public long uraniumMineCount = 0;

    public long nuclearPlantCount = 0;

    public long pipelineCount = 0;


    // CO2 related :

    public long co2Count = 0;

    public long treeCount = 0;

    // Time related :
    public long timeCount = 0;

    public bool pipelineFinished = false;
    public bool hotelBuilt;

    // Events for the UI

    public Action onUpdateDone;

    public Action onGameOver;

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

        // TODO a modifier une fois la prefab de l'hotel et le bouton de construction de l'hotel fait
        if (timeCount >= gameSettings.gameDuration || co2Count >= gameSettings.maxCO2 || hotelBuilt || pipelineFinished)
        {
            // Game is over
            CancelInvoke("GameStateUpdate");
            onGameOver?.Invoke();

            // Store player prefs: co2, tree count, money, time, hotel built
            PlayerPrefs.SetInt("co2", (int)co2Count);
            PlayerPrefs.SetInt("trees", (int)treeCount);
            PlayerPrefs.SetInt("money", (int)moneyCount);
            PlayerPrefs.SetInt("time", (int)timeCount);
            // TODO : a modifier une fois la prefab de l'hotel et le bouton de construction de l'hotel fait
            PlayerPrefs.SetInt("hotelBuilt", pipelineFinished ? 1 : 0);

            ChangeScene.LoadScene("EndScene");
        }

        // Update the UI
        onUpdateDone?.Invoke();
    }



    // Generates a random map of TileStacks
    public void GenerateMap()
    {

        // Create an list of tile stacks using the dims in the mapData
        float x = (gameSettings.XLimit / 2) * -TILE_X_DEFAULT;
        float z = (gameSettings.ZLimit / 2) * -TILE_Z_DEFAULT;

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
                tileStacks[i].Add(new TileStack(x + decalage, 0, z, i, j));
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
                    TileStack[] neighbours = getNeighbours(tileStacks[i][j]);
                    int nbOfNeighboursWithTrees = 0;
                    foreach (TileStack neighbour in neighbours)
                    {
                        if (neighbour.tree.exists)
                        {
                            ++nbOfNeighboursWithTrees;
                        }
                    }
                    if (nbOfNeighboursWithTrees > 0)
                    {
                        if (UnityEngine.Random.Range(0, 100) < gameSettings.TreeProbabilityPerNeighbour * nbOfNeighboursWithTrees)
                        {
                            tileStacks[i][j].tree.exists = true;
                            ++treeCount;
                        }
                    }
                    else if (UnityEngine.Random.Range(0, 100) < gameSettings.TreeProbability)
                    {
                        tileStacks[i][j].tree.exists = true;
                        ++treeCount;
                    }
                    if (UnityEngine.Random.Range(0, 100) < gameSettings.MineralProbability)
                    {
                        int mineral = UnityEngine.Random.Range(0, 100) % 10;
                        switch (mineral)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                tileStacks[i][j].metal.exists = true;
                                break;
                            case 5:
                            case 6:
                            case 7:
                                tileStacks[i][j].gold.exists = true;
                                break;
                            case 8:
                            case 9:
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
                float y = Mathf.PerlinNoise(i * gameSettings.PNxVariation, j * gameSettings.PNzVariation);
                if (tileStacks[i][j].water.exists) { y = 0; }
                createTile(tileStacks[i][j].x, tileStacks[i][j].y + (y * gameSettings.PNMultiplier), tileStacks[i][j].z, tileStacks[i][j]);

            }
        }
    }

    private GameObject createTile(float x, float y, float z, TileStack tileStack)
    {

        float delta = y - TILE_HEIGHT_DEFAULT * 2;
        if (delta < 0) { delta = 0f; }
        int nbOfDirtTiles = 1;
        while (delta > TILE_HEIGHT_DEFAULT)
        {
            ++nbOfDirtTiles;
            delta -= TILE_HEIGHT_DEFAULT;
        }

        //Couche de base
        tileStack.addTile(InstantiateObject(rockTilePrefab, x, 0 + delta, z, 6), TileType.rock);
        // stack the tiles depending on the tileStack
        if (tileStack.water.exists)
        {
            GameObject tile = InstantiateObject(waterTilePrefab, x, TILE_HEIGHT_DEFAULT + delta, z, 4);
            tileStack.addTile(tile, TileType.water);
        }
        else if (tileStack.ground.exists)
        {
            // check if we need to add minerals
            if (tileStack.metal.exists)
            {
                tileStack.addTile(InstantiateObject(metalTilePrefab, x, 0.5f + delta, z, 6), TileType.metal);
            }
            else if (tileStack.gold.exists)
            {
                tileStack.addTile(InstantiateObject(goldTilePrefab, x, 0.5f + delta, z, 6), TileType.gold);
            }
            else if (tileStack.uranium.exists)
            {
                tileStack.addTile(InstantiateObject(uraniumTilePrefab, x, 0.5f + delta, z, 6), TileType.uranium);
            }
            for (int i = 0; i < nbOfDirtTiles; i++)
            {
                float height = TILE_HEIGHT_DEFAULT * (i + 1) + delta;
                tileStack.addTile(InstantiateObject(groundTilePrefab, x, height, z, 6), TileType.ground);
            }
            if (tileStack.grass.exists)
            {
                tileStack.addTile(InstantiateObject(grassTilePrefab, x, TILE_HEIGHT_DEFAULT * nbOfDirtTiles + 1 + delta, z, 6), TileType.grass);

                if (tileStack.tree.exists)
                {
                    // Randomly add trees on tiles
                    int max = UnityEngine.Random.Range(1, 5);
                    for (int i = 0; i < max; i++)
                    {
                        float xOffset = UnityEngine.Random.Range(-0.5f, 0.5f);
                        float zOffset = UnityEngine.Random.Range(-0.5f, 0.5f);
                        float size = UnityEngine.Random.Range(0.8f, 1.5f);
                        GameObject tree = InstantiateObject(treeTilePrefab, x + xOffset, TILE_HEIGHT_DEFAULT * nbOfDirtTiles + 1.5f + delta, z + zOffset, 6);
                        tree.transform.localScale = new Vector3(size, size, size);
                        tileStack.addTile(tree, TileType.tree);
                    }
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
            // highlight the tile
            toggleOutline(tileStack);
        }
    }

    public TileStack getTileStackFromPosition(Vector3 position)
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

    public void handleAction(TileStack tileStack, UserActionType action)
    {
        float aboveRock = tileStack.rock.tilesObject[0].transform.position.y + (TILE_HEIGHT_DEFAULT - 0.4f);
        switch (action)
        {
            case UserActionType.removeTree:
                if (tileStack.tree.exists)
                {
                    tileStack.tree.exists = false;
                    // remove all trees on the tile
                    while (tileStack.tree.tilesObject.Count > 0)
                    {
                        tileStack.tree.tilesObject[0].SetActive(false);
                        tileStack.tree.tilesObject.RemoveAt(0);
                    }

                    --treeCount;
                    moneyCount -= gameSettings.treeCost;
                }
                break;
            case UserActionType.removeGrass:
                if (tileStack.grass.exists && !tileStack.tree.exists)
                {
                    tileStack.grass.exists = false;
                    tileStack.grass.tilesObject[0].SetActive(false);
                    tileStack.grass.tilesObject.RemoveAt(0);
                    moneyCount -= gameSettings.grassCost;
                }
                break;
            case UserActionType.removeGround:
                if (tileStack.ground.exists && !tileStack.grass.exists && !tileStack.tree.exists)
                {
                    tileStack.ground.tilesObject[tileStack.ground.tilesObject.Count - 1].SetActive(false);
                    tileStack.ground.tilesObject.RemoveAt(tileStack.ground.tilesObject.Count - 1);
                    if (tileStack.ground.tilesObject.Count == 0)
                    {
                        tileStack.ground.exists = false;
                    }
                    moneyCount -= gameSettings.groundCost;
                }
                break;
            case UserActionType.buildMetalMine:
                if (isBuildable(action, tileStack))
                {
                    // Disable metal tile
                    tileStack.metal.exists = false;
                    tileStack.metal.tilesObject[0].SetActive(false);
                    tileStack.metal.tilesObject.RemoveAt(0);
                    // Add mine
                    tileStack.addTile(InstantiateObject(metalMineTilePrefab, tileStack.x, aboveRock, tileStack.z, 6), TileType.metalMine);
                    ++metalMineCount;
                    moneyCount -= gameSettings.metalMineCost;
                }
                break;
            case UserActionType.buildGoldMine:
                if (isBuildable(action, tileStack))
                {
                    // Disable gold tile
                    tileStack.gold.exists = false;
                    tileStack.gold.tilesObject[0].SetActive(false);
                    tileStack.gold.tilesObject.RemoveAt(0);
                    // Add mine
                    tileStack.addTile(InstantiateObject(goldMineTilePrefab, tileStack.x, aboveRock, tileStack.z, 6), TileType.goldMine);
                    ++goldMineCount;
                    moneyCount -= gameSettings.goldMineCost;
                }
                break;
            case UserActionType.buildUraniumMine:
                if (isBuildable(action, tileStack))
                {
                    // Disable uranium tile
                    tileStack.uranium.exists = false;
                    tileStack.uranium.tilesObject[0].SetActive(false);
                    tileStack.uranium.tilesObject.RemoveAt(0);
                    // Add mine
                    tileStack.addTile(InstantiateObject(uraniumMineTilePrefab, tileStack.x, aboveRock, tileStack.z, 6), TileType.uraniumMine);
                    ++uraniumMineCount;
                    moneyCount -= gameSettings.uraniumMineCost;
                }
                break;
            case UserActionType.buildNuclearPlant:
                if (isBuildable(action, tileStack))
                {

                    float lastGroundTilePositionY = tileStack.ground.tilesObject[tileStack.ground.tilesObject.Count - 1].transform.position.y;
                    tileStack.addTile(InstantiateObject(nuclearPlantTilePrefab, tileStack.x, lastGroundTilePositionY + (TILE_HEIGHT_DEFAULT / 2), tileStack.z, 6), TileType.nuclearPlant);
                    ++nuclearPlantCount;
                    moneyCount -= gameSettings.nuclearPowerPlantCost;
                }
                break;
            case UserActionType.buildPipeline:
                if (isBuildable(action, tileStack))
                {
                    float lastGroundTilePositionY = tileStack.ground.tilesObject[tileStack.ground.tilesObject.Count - 1].transform.position.y;
                    tileStack.addTile(InstantiateObject(pipelineTilePrefab, tileStack.x, lastGroundTilePositionY + (TILE_HEIGHT_DEFAULT / 2), tileStack.z, 6), TileType.pipeline);
                    ++pipelineCount;
                    moneyCount -= gameSettings.pipelineCost;
                    // Check if there is water nearby
                    TileStack[] neighbours = getNeighbours(tileStack);
                    foreach (TileStack neighbour in neighbours)
                    {
                        if (neighbour.water.exists)
                        {
                            pipelineFinished = true;
                            break;
                        }
                    }
                }
                break;
            case UserActionType.buildHotel:
                // get the tile at map position 0,0
                TileStack hotelTileStack = null;
                for (int i = 0; i < tileStacks.Count; ++i)
                {
                    for (int j = 0; j < tileStacks[i].Count; ++j)
                    {
                        if (tileStacks[i][j].volcano.exists)
                        {
                            hotelTileStack = tileStacks[i][j];
                            break;
                        }
                    }
                    if (hotelTileStack != null)
                    {
                        break;
                    }
                }

                if (isBuildable(action, hotelTileStack))
                {
                    // Idealement, instancier un joli hotel
                    hotelBuilt = true;
                }
                break;
            case UserActionType.dig:
                handleAction(tileStack, UserActionType.removeTree);
                handleAction(tileStack, UserActionType.removeGrass);
                while (tileStack.ground.tilesObject.Count > 0)
                {
                    handleAction(tileStack, UserActionType.removeGround);
                }
                break;


            case UserActionType.none:
            default:
                return;
        }
        currentAction = UserActionType.none;
    }

    public bool isBuildable(UserActionType actionType, TileStack tileStack)
    {
        if (tileStack.metalMine.exists || tileStack.goldMine.exists || tileStack.uraniumMine.exists || tileStack.nuclearPlant.exists || tileStack.pipeline.exists)
        {
            return false;
        }

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
                    return false;
                case UserActionType.buildPipeline:
                    return false;
                default:
                    return false;
            }
        }
        else if (!tileStack.tree.exists && !tileStack.grass.exists && tileStack.ground.exists)
        {
            switch (actionType)
            {
                case UserActionType.buildNuclearPlant:
                    return true;
                case UserActionType.buildPipeline:
                    // check surrounding tiles. One of them needs to have a pipeline or a volcano border
                    TileStack[] neighbours = getNeighbours(tileStack);
                    foreach (TileStack neighbour in neighbours)
                    {
                        if (neighbour != null && (neighbour.pipeline.exists || neighbour.volcanoBorder.exists))
                        {
                            return true;
                        }
                    }
                    return false;
                default:
                    return false;
            }
        }
        else if (pipelineFinished && actionType == UserActionType.buildHotel)
        {
            return true;
        }
        return false;
    }

    public void toggleOutline(TileStack tileStack)
    {
        // Disable the outline for all tiles
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tiles)
        {
            if (tile.GetComponent<Outline>() != null)
            {
                tile.GetComponent<Outline>().enabled = false;
            }
        }
        // Outline the highest tile
        if (tileStack.tree.exists)
        {
            tileStack.tree.highlightTile();
        }
        else if (tileStack.grass.exists)
        {
            tileStack.grass.highlightTile();
        }
        else if (tileStack.ground.exists)
        {
            tileStack.ground.highlightTile();
        }
        else if (tileStack.metal.exists)
        {
            tileStack.metal.highlightTile();
        }
        else if (tileStack.gold.exists)
        {
            tileStack.gold.highlightTile();
        }
        else if (tileStack.uranium.exists)
        {
            tileStack.uranium.highlightTile();
        }
        else if (tileStack.metalMine.exists)
        {
            tileStack.metalMine.highlightTile();
        }
        else if (tileStack.goldMine.exists)
        {
            tileStack.goldMine.highlightTile();
        }
        else if (tileStack.uraniumMine.exists)
        {
            tileStack.uraniumMine.highlightTile();
        }
        else if (tileStack.nuclearPlant.exists)
        {
            tileStack.nuclearPlant.highlightTile();
        }
        else if (tileStack.pipeline.exists)
        {
            tileStack.pipeline.highlightTile();
        }
        else if (tileStack.hotel.exists)
        {
            tileStack.hotel.highlightTile();
        }
    }


    // a function that returns the 6 neighbours of a tile, taking into account the hexagonal grid
    public TileStack[] getNeighbours(TileStack tileStack)
    {
        TileStack[] neighbours = new TileStack[6];
        int x = tileStack.xIndex;
        int z = tileStack.zIndex;
        if (x % 2 == 0)
        {
            neighbours[0] = tileStacks[x - 1][z];
            neighbours[1] = tileStacks[x - 1][z + 1];
            neighbours[2] = tileStacks[x][z + 1];
            neighbours[3] = tileStacks[x + 1][z];
            neighbours[4] = tileStacks[x][z - 1];
            neighbours[5] = tileStacks[x - 1][z - 1];
        }
        else
        {
            neighbours[0] = tileStacks[x - 1][z];
            neighbours[1] = tileStacks[x][z + 1];
            neighbours[2] = tileStacks[x + 1][z + 1];
            neighbours[3] = tileStacks[x + 1][z];
            neighbours[4] = tileStacks[x + 1][z - 1];
            neighbours[5] = tileStacks[x][z - 1];
        }
        // display the neighbours in the console
        Debug.Log("Current : " + tileStack + ", 1 : " + neighbours[0] + ", 2 : " + neighbours[1] + ", 3 : " + neighbours[2] + ", 4 : " + neighbours[3] + ", 5 : " + neighbours[4] + ", 6 : " + neighbours[5]);
        return neighbours;
    }
}