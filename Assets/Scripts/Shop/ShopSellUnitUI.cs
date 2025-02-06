using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ShopSellUnitUI : MonoBehaviour
{
    private bool hoveringWithUnit;
    private GameObject heldUnit;
    
    [SerializeField] UnitPlacement unitPlacement;
    [SerializeField] Shop shop;

    // Update is called once per frame
    void Update()
    {
        if (hoveringWithUnit)
        {
            if (Input.GetMouseButtonUp(0))
            {
                shop.TrySellUnit(heldUnit);
            }
        }
    }

    public void PointerEnter()
    {
        if (unitPlacement.GetMyHeldUnit())
        {
            hoveringWithUnit = true;
            heldUnit = unitPlacement.GetMyHeldUnit();
        }
        else
        {
            hoveringWithUnit = false;
            heldUnit = null;
        }
    }

    public void PointerExit()
    {
        hoveringWithUnit = false;
        heldUnit = null;
    }

    public void OnDisable()
    {
        PointerExit();
    }



}
