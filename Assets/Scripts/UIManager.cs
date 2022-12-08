using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	public TileOnClick tileOnClick;

    // Buttons
    public Button selectedButton;
    public Button removeButton;

    // Progress bars
    public ProgressBar money;
    public float newMoney;
    public TMP_Text moneyText;
    public ProgressBar co2;
    public float newCo2;
    public TMP_Text co2Text;

    public GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        tileOnClick = GameObject.Find("TileOnClickHandler").GetComponent<TileOnClick>();
        removeButton = GameObject.Find("RemoveButton").GetComponent<Button>();
        // Update resource every 1 second
        gameManager.onUpdateDone += UpdateResource;
    }

    void Update()
    {
        // Enable button according to selected tile/object
        if (tileOnClick.selectedTile != null)
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            if (current != null && current.tree.exists)
            {
                removeButton.interactable = true;
            }
            else
            {
                removeButton.interactable = false;
            }
        }
        else
        {
            removeButton.interactable = false;
        }
    }

    private void UpdateResource(long co2Count, long moneyCount, long timeCount)
    {
        // Money
        money = GameObject.Find("MoneyProgressBar").GetComponent<ProgressBar>();
        moneyText = GameObject.Find("MoneyCount").GetComponent<TMP_Text>();
        newMoney = gameManager.moneyCount;
        money.current = newMoney;
        moneyText.text = newMoney.ToString();

        // CO2
        co2 = GameObject.Find("CO2ProgressBar").GetComponent<ProgressBar>();
        co2Text = GameObject.Find("CO2Count").GetComponent<TMP_Text>();
        newCo2 = gameManager.co2Count;
        co2.current = newCo2;
        co2Text.text = newCo2.ToString();
    }

    public void ButtonClicked()
    {
        // Log current clicked button
        selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        if (selectedButton.name == "RemoveButton")
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.cutTree);
        }
        tileOnClick.selectedTile = null;
    }
}
