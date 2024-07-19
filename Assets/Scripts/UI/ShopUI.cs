using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GamePhaseManager;

public class ShopUI : MonoBehaviour
{

    //Hide Shop During Battle Not During Shop
    [SerializeField] GamePhaseManager gamePhaseManager;

    [SerializeField] GameObject shopUIObj;

    [SerializeField] GameObject shopItemsUI;
    [SerializeField] GameObject sellUnitUI;
    
    [SerializeField] UnitPlacement unitPlacement;

    // Update is called once per frame
    void Update()
    {
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

    private void LateUpdate()
    {
        SetInteractionAreaDisplay();
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
}
