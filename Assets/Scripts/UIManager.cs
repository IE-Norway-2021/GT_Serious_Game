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


	// Start is called before the first frame update
	void Start()
	{
        tileOnClick = GameObject.Find("TileOnClickHandler").GetComponent<TileOnClick>();
        removeButton = GameObject.Find("RemoveButton").GetComponent<Button>();
		// Update resource every 1 second
		InvokeRepeating("UpdateResource", 0, 1);
	}

	void Update()
	{
        // Enable button according to selected tile/object
        if (tileOnClick.selectedTile != null)
        {
            removeButton.interactable = true;
        }
        else
        {
            removeButton.interactable = false;
        }
	}

	private void UpdateResource()
	{
		// Money
		money = GameObject.Find("MoneyProgressBar").GetComponent<ProgressBar>();
		moneyText = GameObject.Find("MoneyCount").GetComponent<TMP_Text>();
		newMoney = GameObject.Find("GameManager").GetComponent<GameManager>().moneyCount;
		money.current = newMoney;
		moneyText.text = newMoney.ToString();

		// CO2
		co2 = GameObject.Find("CO2ProgressBar").GetComponent<ProgressBar>();
		co2Text = GameObject.Find("CO2Count").GetComponent<TMP_Text>();
		newCo2 = GameObject.Find("GameManager").GetComponent<GameManager>().co2Count;
		co2.current = newCo2;
		co2Text.text = newCo2.ToString();
	}

	public void ButtonClicked()
	{
		// Log current clicked button
		selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        if (selectedButton.name == "RemoveButton")
        {
            Destroy(tileOnClick.selectedTile.gameObject);
        }
        tileOnClick.selectedTile = null;
	}
}
