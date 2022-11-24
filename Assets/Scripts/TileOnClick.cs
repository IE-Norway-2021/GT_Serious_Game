using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class TileOnClick : MonoBehaviour
{

    public Action<GameObject> OnTileClick;

    // // Start is called before the first frame update
    void Start()
    {

    }

    // // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                GameObject tile = hit.transform.gameObject;
                toggleOutline(tile);
            }
        }
    }

    void toggleOutline(GameObject gameobject)
    {
        // Disable the outline for all tiles
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tiles)
        {
            tile.GetComponent<Outline>().enabled = false;
        }

        // Enable the outline for the clicked tile
        gameobject.GetComponent<Outline>().enabled = true;
        OnTileClick?.Invoke(gameobject);
    }
}
