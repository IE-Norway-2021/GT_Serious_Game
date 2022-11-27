using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Map", menuName = "Map")]
public class MapData : ScriptableObject
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
}