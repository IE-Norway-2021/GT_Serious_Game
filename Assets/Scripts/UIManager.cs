using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TileOnClick tileOnClick;
    public GameObject currentSelectedTile;

    // Buttons
    public Button selectedButton;
    public Button removeButton;
    public Button digButton;

    public Button buildMetalButton;
    public Button buildGoldButton;
    public Button buildUraniumButton;
    public Button buildNuclearButton;
    public Button buildPipelineButton;

    // Progress bars and infos
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
        buildMetalButton = GameObject.Find("BuildMetalButton").GetComponent<Button>();
        buildGoldButton = GameObject.Find("BuildGoldButton").GetComponent<Button>();
        buildUraniumButton = GameObject.Find("BuildUraniumButton").GetComponent<Button>();
        buildNuclearButton = GameObject.Find("BuildNuclearButton").GetComponent<Button>();
        buildPipelineButton = GameObject.Find("BuildPipelineButton").GetComponent<Button>();
        digButton = GameObject.Find("DigButton").GetComponent<Button>();

        // Update resource every 1 second
        gameManager.onUpdateDone += UpdateResource;
        // Update buttons every 1 second
        // gameManager.onUpdateDone += UpdateButtons;
        // Game over
        gameManager.onGameOver += GameOver;

        // CO2
        co2 = GameObject.Find("CO2ProgressBar").GetComponent<ProgressBar>();
        co2Text = GameObject.Find("CO2Count").GetComponent<TMP_Text>();
        co2.maximum = gameManager.gameSettings.maxCO2;

        // Time
        time = GameObject.Find("TimeProgressBar").GetComponent<ProgressBar>();
        time.maximum = gameManager.gameSettings.gameDuration;
        time.current = gameManager.gameSettings.gameDuration;
    }

    void Update()
    {
        // Enable button according to selected tile/object
        if (tileOnClick.selectedTile != null)
        {

            if (tileOnClick.selectedTile != currentSelectedTile)
            {
                // Disable all buttons when selecting a new tile
                disableAllButtons();
                currentSelectedTile = tileOnClick.selectedTile;
            }

            // Enable button according to selected tile/object and cast action
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            if (current != null)
            {
                // If it's a ground tile...
                if (canRemoveTile(current))
                {
                    // We can remove the tile...
                    removeButton.interactable = true;
                    digButton.interactable = true;
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        removeTile(current);
                        UpdateResource();
                        removeButton.interactable = false;
                        digButton.interactable = false;

                    }
                    if (Input.GetKeyDown(KeyCode.T))
                    {
                        gameManager.handleAction(current, UserActionType.dig);
                        UpdateResource();
                        removeButton.interactable = false;
                        digButton.interactable = false;

                    }

                }
                else if (gameManager.isBuildable(UserActionType.buildMetalMine, current) && computeCostOfBuilding(UserActionType.buildMetalMine) <= gameManager.moneyCount)
                {
                    buildMetalButton.interactable = true;
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        gameManager.handleAction(current, UserActionType.buildMetalMine);
                        UpdateResource();
                        disableAllButtons();
                    }
                }
                else if (gameManager.isBuildable(UserActionType.buildGoldMine, current) && computeCostOfBuilding(UserActionType.buildGoldMine) <= gameManager.moneyCount)
                {
                    buildGoldButton.interactable = true;
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        gameManager.handleAction(current, UserActionType.buildGoldMine);
                        UpdateResource();
                        disableAllButtons();
                    }
                }
                else if (gameManager.isBuildable(UserActionType.buildUraniumMine, current) && computeCostOfBuilding(UserActionType.buildUraniumMine) <= gameManager.moneyCount)
                {
                    buildUraniumButton.interactable = true;
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        gameManager.handleAction(current, UserActionType.buildUraniumMine);
                        UpdateResource();
                        disableAllButtons();
                    }
                }
                else
                {
                    disableAllButtons();
                }

                // ... or build on it.
                if (canBuildOnTile(current, UserActionType.buildNuclearPlant) || canBuildOnTile(current, UserActionType.buildPipeline))
                {
                    // Nuclear Plant
                    if (canBuildOnTile(current, UserActionType.buildNuclearPlant))
                    {
                        buildNuclearButton.interactable = true;
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            gameManager.handleAction(current, UserActionType.buildNuclearPlant);
                            UpdateResource();
                            disableAllButtons();
                        }
                    }

                    // Pipeline
                    if (canBuildOnTile(current, UserActionType.buildPipeline) && gameManager.isBuildable(UserActionType.buildPipeline, current))
                    {
                        buildPipelineButton.interactable = true;
                        if (Input.GetKeyDown(KeyCode.C))
                        {
                            gameManager.handleAction(current, UserActionType.buildPipeline);
                            UpdateResource();
                            disableAllButtons();
                        }
                    }
                }
            }
        }
        else
        {
            disableAllButtons();
        }

        // Costs
        TMP_Text removeCostTextButton = GameObject.Find("RemoveCost").GetComponent<TMP_Text>();
        string removeCostSetting = "0";
        if (tileOnClick.selectedTile != null)
        {
            TileStack currentTilestack = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            if (currentTilestack != null)
            {
                removeCostSetting = computeCostOfRemoval(currentTilestack).ToString();
            }
        }
        removeCostTextButton.text = $"-{removeCostSetting}$";

        TMP_Text digCostTextButton = GameObject.Find("DigCost").GetComponent<TMP_Text>();
        long digCost = 0;
        if (tileOnClick.selectedTile != null)
        {
            TileStack currentTilestack = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            if (currentTilestack != null)
            {
                // if there are trees 
                if (currentTilestack.tree.exists)
                {
                    digCost += gameManager.gameSettings.treeCost;
                }
                // if there is grass
                if (currentTilestack.grass.exists)
                {
                    digCost += gameManager.gameSettings.grassCost;
                }
                // if there is ground
                if (currentTilestack.ground.exists)
                {
                    digCost += gameManager.gameSettings.groundCost * currentTilestack.ground.tilesObject.Count;
                }
            }
        }
        string digCostSetting = digCost.ToString();
        digCostTextButton.text = $"-{digCostSetting}$";

        TMP_Text metalCostTextButton = GameObject.Find("BuildMetalCost").GetComponent<TMP_Text>();
        string metalCostSetting = gameManager.gameSettings.metalMineCost.ToString();
        metalCostTextButton.text = $"-{metalCostSetting}$";

        TMP_Text goldCostTextButton = GameObject.Find("BuildGoldCost").GetComponent<TMP_Text>();
        string goldCostSetting = gameManager.gameSettings.goldMineCost.ToString();
        goldCostTextButton.text = $"-{goldCostSetting}$";

        TMP_Text uraniumCostTextButton = GameObject.Find("BuildUraniumCost").GetComponent<TMP_Text>();
        string uraniumCostSetting = gameManager.gameSettings.uraniumMineCost.ToString();
        uraniumCostTextButton.text = $"-{uraniumCostSetting}$";

        TMP_Text nuclearCostTextButton = GameObject.Find("BuildNuclearCost").GetComponent<TMP_Text>();
        string nuclearCostSetting = gameManager.gameSettings.nuclearPowerPlantCost.ToString();
        nuclearCostTextButton.text = $"-{nuclearCostSetting}$";

        TMP_Text pipelineCostTextButton = GameObject.Find("BuildPipelineCost").GetComponent<TMP_Text>();
        string pipelineCostSetting = gameManager.gameSettings.pipelineCost.ToString();
        pipelineCostTextButton.text = $"-{pipelineCostSetting}$";
    }

    private void disableAllButtons()
    {
        removeButton.interactable = false;
        digButton.interactable = false;
        buildMetalButton.interactable = false;
        buildGoldButton.interactable = false;
        buildUraniumButton.interactable = false;
        buildNuclearButton.interactable = false;
        buildPipelineButton.interactable = false;
    }

    private void UpdateResource()
    {
        // Money
        moneyText = GameObject.Find("MoneyCount").GetComponent<TMP_Text>();
        newMoney = gameManager.moneyCount;
        moneyText.text = newMoney.ToString();

        // CO2
        newCo2 = gameManager.co2Count;
        co2.current = newCo2;
        co2Text.text = newCo2.ToString();

        // Time
        time.current = gameManager.timeCount;
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
        // take the button that was clicked
        Button selectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        if (selectedButton.name == "RemoveButton") // remove top tile if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            if (canRemoveTile(current))
            {
                removeTile(current);
            }
        }

        if (selectedButton.name == "DigButton") // remove top tile if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            if (canRemoveTile(current))
            {
                gameManager.handleAction(current, UserActionType.dig);
                UpdateResource();
            }
        }

        if (selectedButton.name == "BuildMetalButton") // Build correct building if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.buildMetalMine);
            UpdateResource();
        }

        if (selectedButton.name == "BuildGoldButton") // Build correct building if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.buildGoldMine);
            UpdateResource();
        }

        if (selectedButton.name == "BuildUraniumButton") // Build correct building if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.buildUraniumMine);
            UpdateResource();
        }

        if (selectedButton.name == "BuildNuclearButton") // Build correct building if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.buildNuclearPlant);
            UpdateResource();
        }

        if (selectedButton.name == "BuildPipelineButton") // Build correct building if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.buildPipeline);
            UpdateResource();
        }

        if (selectedButton.name == "BuildHotelButton") // Build correct building if possible
        {
            TileStack current = gameManager.getTileStackFromPosition(tileOnClick.selectedTile.transform.position);
            gameManager.handleAction(current, UserActionType.buildHotel);
            UpdateResource();
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

    public long computeCostOfBuilding(UserActionType actionType)
    {
        switch (actionType)
        {
            case UserActionType.buildMetalMine:
                return gameManager.gameSettings.metalMineCost;
            case UserActionType.buildGoldMine:
                return gameManager.gameSettings.goldMineCost;
            case UserActionType.buildUraniumMine:
                return gameManager.gameSettings.uraniumMineCost;
            case UserActionType.buildNuclearPlant:
                return gameManager.gameSettings.nuclearPowerPlantCost;
            case UserActionType.buildPipeline:
                return gameManager.gameSettings.pipelineCost;
            case UserActionType.buildHotel:
                return gameManager.gameSettings.hotelCost;
            default:
                return 0;
        }
    }


    public bool canRemoveTile(TileStack tileStack)
    {
        if (tileStack.tree.exists || tileStack.grass.exists || (tileStack.ground.exists && (!tileStack.metalMine.exists && !tileStack.goldMine.exists && !tileStack.uraniumMine.exists && !tileStack.nuclearPlant.exists && !tileStack.pipeline.exists && !tileStack.hotel.exists)))
        {
            // check if player has enough money
            if (gameManager.moneyCount >= computeCostOfRemoval(tileStack))
            {
                return true;
            }
        }
        return false;
    }

    public bool canBuildOnTile(TileStack tileStack, UserActionType actionType)
    {
        if (tileStack.ground.exists && !tileStack.tree.exists && !tileStack.grass.exists && !tileStack.nuclearPlant.exists && !tileStack.pipeline.exists && !tileStack.hotel.exists && !tileStack.water.exists && !tileStack.volcano.exists && !tileStack.volcanoBorder.exists)
        {
            // check if player has enough money
            if (gameManager.moneyCount >= computeCostOfBuilding(actionType))
            {
                switch (actionType)
                {
                    case UserActionType.buildNuclearPlant:
                        // check if co2 is high enough
                        if (gameManager.co2Count < gameManager.gameSettings.nuclearPowerPlantCO2Threshold)
                        {
                            return false;
                        }
                        break;
                    case UserActionType.buildPipeline:
                        // check if co2 is high enough
                        if (gameManager.co2Count < gameManager.gameSettings.pipelineCO2Threshold)
                        {
                            return false;
                        }
                        break;
                    default:
                        return true;
                }
                return true;
            }
        }
        return false;
    }

    public void removeTile(TileStack current)
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
