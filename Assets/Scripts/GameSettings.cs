using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Map", menuName = "Map")]
public class GameSettings : ScriptableObject
{

    //
    // !!!!!!! All dimensions are in tiles and not in map coordinates !!!!!!!
    //

    [Header("Map Dimensions")]

    // Dimensions of the level
    public int XLimit;
    public int ZLimit;

    // The volcano dims
    public int VolcanoRadius;
    public int VolcanoHeight;

    // The island dims
    public int IslandRadius;

    [Header("Map Generation")]
    public int TreeProbability;

    public int MineralProbability;

    [Header("Camera Settings")]

    public float zoomMax;
    public float zoomMin;
    public float rotateSpeed = 8f;
    public float angleView;

    [Header("Game Settings")]
    [Header("Production")]
    public long updateDelay;
    public long metalMineMoneyProduction;
    public long metalMineCO2Production;
    public long goldMineMoneyProduction;
    public long goldMineCO2Production;
    public long uraniumMineMoneyProduction;
    public long uraniumMineCO2Production;
    public long nuclearPlantMoneyIncrease;
    public long nuclearPlantCO2Production;
    public long pipelineCO2Production;
    public long treeCO2Decrease;

    [Header("Costs")]
    public long metalMineCost;
    public long goldMineCost;
    public long uraniumMineCost;
    public long nuclearPowerPlantCost;
    public long pipelineCost;
    public long treeCost;

    [Header("Other")]
    public long startingMoney;


}
