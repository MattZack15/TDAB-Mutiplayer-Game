using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GamePhaseManager;

public class ShopUI : MonoBehaviour
{
    [SerializeField] GamePhaseManager gamePhaseManager;

    [SerializeField] GameObject shopUIObj;

    [SerializeField] GameObject shopItemsUI;
    [SerializeField] GameObject sellUnitUI;
    
    [SerializeField] UnitPlacement unitPlacement;

    [SerializeField] ServerPlayerDataManager ServerPlayerDataManager;
    [SerializeField] TMP_Text coinsText;
    [SerializeField] TMP_Text levelText;

    [SerializeField] TMP_Text levelCostText;
    [SerializeField] TMP_Text refreshCostText;

    // Update is called once per frame
    void Update()
    {
        // Hide Shop During Battle Phase
        HideShop();
        // Update level and coins information
        UpdateInformationAreaDisplay();

    }

    private void LateUpdate()
    {
        SetInteractionAreaDisplay();
    }

    private void HideShop()
    {
        //Hide Shop During Battle Phase
        bool hide = !(gamePhaseManager.GamePhase == GamePhases.ShopPhase);

        if (hide)
        {
            shopUIObj.SetActive(false);
        }
        else
        {
            shopUIObj.SetActive(true);
        }
    }

    private void SetInteractionAreaDisplay()
    {
        // Displays either the Shop items or the sell area
        if (unitPlacement.GetHeldUnit())
        {
            shopItemsUI.SetActive(false);
            sellUnitUI.SetActive(true);
        }
        else
        {
            shopItemsUI.SetActive(true);
            sellUnitUI.SetActive(false);
        }
    }

    private void UpdateInformationAreaDisplay()
    {
        // Updates the Player Level and Coins Information
        ServerPlayerData myPlayerData = ServerPlayerDataManager.GetMyPlayerData();
        if (myPlayerData == null) return;

        // Coins
        coinsText.SetText(myPlayerData.coins.Value.ToString());

        // Level
        levelText.SetText(myPlayerData.level.Value.ToString());

        // Level Cost
        levelCostText.SetText($"Level\n(Cost {myPlayerData.levelCost.Value})");

        // Refresh Cost
        int refreshCost = 1;
        if (myPlayerData.freeRefreshes.Value > 0)
        {
            refreshCost = 0;
        }
        refreshCostText.SetText($"Refresh\n(Cost {refreshCost})");

    }
}
