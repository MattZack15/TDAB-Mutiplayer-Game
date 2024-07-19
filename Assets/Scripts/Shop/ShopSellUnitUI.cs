using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ShopSellUnitUI : MonoBehaviour
{
    private bool hoveringWithUnit;
    private GameObject unit;
    
    [SerializeField] UnitPlacement unitPlacement;
    [SerializeField] Shop shop;

    // Update is called once per frame
    void Update()
    {
        if (hoveringWithUnit)
        {
            if (Input.GetMouseButtonUp(0))
            {
                shop.TrySellUnit(unit);
            }
        }
    }

    public void PointerEnter()
    {
        if (unitPlacement.GetHeldUnit())
        {
            hoveringWithUnit = true;
            unit = unitPlacement.GetHeldUnit();

        }
        else
        {
            hoveringWithUnit = false;
            unit = null;

        }
    }

    public void PointerExit()
    {
        hoveringWithUnit = false;
        unit = null;
    }

    public void OnDisable()
    {
        PointerExit();
    }



}
