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

	// Pelrin Noise settings
	[Range(0f, 0.1f)]
	public float PNxVariation;
	[Range(0f, 0.1f)]
	public float PNzVariation;
	[Range(0f, 10f)]
	public float PNMultiplier;

	// The volcano dims
	public int VolcanoRadius;
	public int VolcanoHeight;

	// The island dims
	public int IslandRadius;

	[Header("Map Generation")]
	public int TreeProbability;

	public int MineralProbability;

    public Vector3 zoomMax;
    public Vector3 zoomMin;
    public float rotateSpeed = 8f;
    public float angleView;

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
	public long hotelCost;
	public long treeCost;
	public long grassCost;
	public long groundCost;

	public long goldMineCO2Threshold;
	public long uraniumMineCO2Threshold;
	public long nuclearPowerPlantCO2Threshold;
	public long pipelineCO2Threshold;

	[Header("Other")]
	public long startingMoney;
	public long gameDuration;
	public long maxCO2;


}
