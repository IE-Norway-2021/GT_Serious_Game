using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RemoveTile : MonoBehaviour
{

    public Button removeButton;
    public GameObject selectedTile;

	void Start () {
        selectedTile = GameObject.Find("GameManager").GetComponent<TileOnClick>().selectedTile;
        Debug.Log(selectedTile);
		removeButton.onClick.AddListener(TaskOnClick);
	}

    public void TaskOnClick ()
    {
        Debug.Log("Btn clicked");
        Debug.Log("Current selected tile: " + selectedTile);

        if (selectedTile != null)
        {
            Debug.Log("Current selected tile: " + selectedTile);
            // Destroy(selectedTile);
        }
    }
}
