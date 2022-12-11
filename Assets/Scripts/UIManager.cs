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
    public ProgressBar time;
    public float newTime;
    public TMP_Text timeText;

    public GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        tileOnClick = GameObject.Find("TileOnClickHandler").GetComponent<TileOnClick>();
        removeButton = GameObject.Find("RemoveButton").GetComponent<Button>();
        // Update resource every 1 second
        gameManager.onUpdateDone += UpdateResource;
        // Update buttons every 1 second
        //gameManager.onUpdateDone += UpdateButtons;
        // Game over
        gameManager.onGameOver += GameOver;
    }

    void Update()
    {
        // Enable button according to selected tile/object
        if (tileOnClick.selectedTile != null)
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            if (current != null && canRemoveTile(current))
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

    private void UpdateResource()
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

        // Time
        // time = GameObject.Find("TimeProgressBar").GetComponent<ProgressBar>();
        // timeText = GameObject.Find("TimeCount").GetComponent<TMP_Text>();
        // newTime = gameManager.timeCount;
        // time.current = newTime;
        // timeText.text = newTime.ToString();
    }

    private void UpdateButtons()
    {
        // handle building buttons state
        // degriser les boutons des buildings dès que les conditions pour les poser sont remplies, ou les griser si elles ne le sont pas
        if (gameManager.moneyCount >= gameManager.gameSettings.metalMineCost)
        {
            GameObject.Find("BuildMetalButton").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("BuildMetalButton").GetComponent<Button>().interactable = false;
        }

        if (gameManager.moneyCount >= gameManager.gameSettings.goldMineCost && gameManager.co2Count >= gameManager.gameSettings.goldMineCO2Threshold)
        {
            GameObject.Find("BuildGoldButton").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("BuildGoldButton").GetComponent<Button>().interactable = false;
        }

        if (gameManager.moneyCount >= gameManager.gameSettings.uraniumMineCost && gameManager.co2Count >= gameManager.gameSettings.uraniumMineCO2Threshold)
        {
            GameObject.Find("BuildUraniumButton").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("BuildUraniumButton").GetComponent<Button>().interactable = false;
        }

        if (gameManager.moneyCount >= gameManager.gameSettings.nuclearPowerPlantCost && gameManager.co2Count >= gameManager.gameSettings.nuclearPowerPlantCO2Threshold)
        {
            GameObject.Find("BuildNuclearButton").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("BuildNuclearButton").GetComponent<Button>().interactable = false;
        }

        if (gameManager.moneyCount >= gameManager.gameSettings.pipelineCost && gameManager.co2Count >= gameManager.gameSettings.pipelineCO2Threshold)
        {
            GameObject.Find("BuildPipelineButton").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("BuildPipelineButton").GetComponent<Button>().interactable = false;
        }

        if (gameManager.moneyCount >= gameManager.gameSettings.hotelCost && gameManager.pipelineFinished)
        {
            GameObject.Find("BuildHotelButton").GetComponent<Button>().interactable = true;
        }
        else
        {
            GameObject.Find("BuildHotelButton").GetComponent<Button>().interactable = false;
        }
    }

    private void GameOver()
    {
        // TODO : stopper les calculs de ressources et CO2 en cours et afficher un message de fin de partie. Blocker l'ui
    }

    public void ButtonClicked()
    {
        // Log current clicked button
        selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        if (selectedButton.name == "RemoveButton") // remove top tile if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            if (canRemoveTile(current))
            {
                if (current.tree.exists)
                {
                    gameManager.handleAction(current, UserActionType.removeTree);
                }
                else if (current.grass.exists)
                {
                    gameManager.handleAction(current, UserActionType.removeGrass);
                }
                else if (current.ground.exists)
                {
                    gameManager.handleAction(current, UserActionType.removeGround);
                }
            }
        }

        if (selectedButton.name == "BuildMetalButton") // Build correct building if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.buildMetalMine);
        }

        if (selectedButton.name == "BuildGoldButton") // Build correct building if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.buildGoldMine);
        }

        if (selectedButton.name == "BuildUraniumButton") // Build correct building if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.buildUraniumMine);
        }

        if (selectedButton.name == "BuildNuclearButton") // Build correct building if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.buildNuclearPlant);
        }

        if (selectedButton.name == "BuildPipelineButton") // Build correct building if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.buildPipeline);
        }

        if (selectedButton.name == "BuildHotelButton") // Build correct building if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.buildHotel);
        }

        tileOnClick.selectedTile = null;
    }

    public long computeCostOfRemoval(TileStack tileStack)
    {
        // Calculer le cout de la destruction de la tile par rapport à ce qui est au sommet
        if (tileStack.tree.exists)
        {
            return gameManager.gameSettings.treeCost;
        }
        else if (tileStack.grass.exists)
        {
            return gameManager.gameSettings.grassCost;
        }
        else if (tileStack.ground.exists)
        {
            return gameManager.gameSettings.groundCost;
        }
        return 0;
    }

    public bool canRemoveTile(TileStack tileStack)
    {
        if (tileStack.tree.exists || tileStack.grass.exists || (tileStack.ground.exists && (!tileStack.metalMine.exists && !tileStack.goldMine.exists && !tileStack.uraniumMine.exists && !tileStack.nuclearPlant.exists && !tileStack.pipeline.exists && !tileStack.hotel.exists)))
        {
            return true;
        }
        return false;
    }
}
