using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class TileOnClick : MonoBehaviour
{

    public GameObject selectedTile;
    public Action<GameObject> OnTileClick;


    // // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Avoid ray casting on UI
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                GameObject tile = hit.transform.gameObject;

                // TODO: list all the clickable tiles
                if (tile.tag == "Tile")
                {
                    selectedTile = tile;
                    // OnTileClick?.Invoke(tile);
                } else {
                    selectedTile = null;
                    // OnTileClick?.Invoke(null);
                }
                // OnTileClick?.Invoke(tile);
            }
        }
    }
}
