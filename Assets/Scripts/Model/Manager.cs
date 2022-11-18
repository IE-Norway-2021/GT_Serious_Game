/*
Loads the island from the json file
The json file contains a 3D array that describes the island using the different tile types
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { //the values that are stored in the json file
    water = 0,
    tree = 1,
    grass = 2,
    ground = 3,
    rock = 4, // unbuildable
    metal = 5, 
    gold = 6,
    uranium = 7
};

public class Manager : MonoBehaviour
{
    

    // create a List with an array of the different tile types
    public TileType[,,] tilesLoaded = new TileType[,,] { { {0},{0} },{{0},{0}},{{0},{0}} };

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



    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        loadIsland();

        Debug.Log("TileStacks created");

        createIsland();

        Debug.Log("Island created");
    }

    // Update is called once per frame
    void Update()
    {
        
    }   

    public void loadIsland(){
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

    public void createIsland() {
        for (int i = 0; i < tileStacks.Count; i++)
        {
            for (int j = 0; j < tileStacks[i].Count; j++)
            {
                createTile(i, j, tileStacks[i][j]);
            }
        }
    }

    private GameObject createTile(float x, float z, TileStack tileStack) {
        // stack the tiles depending on the tileStack
        if (tileStack.water) {
            GameObject tmpTile = Instantiate(waterTilePrefab);
            tmpTile.transform.position = new Vector3(x, 0, z);
            tmpTile.transform.SetParent(tileHolder);
            return tmpTile;
        }
        return null;
    }
    
}