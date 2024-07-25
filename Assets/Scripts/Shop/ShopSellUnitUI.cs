using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ShopSellUnitUI : MonoBehaviour
{
    private bool hoveringWithUnit;
    private GameObject unit;
    private Vector3 unitTileID;
    
    [SerializeField] UnitPlacement unitPlacement;
    [SerializeField] Shop shop;

    // Update is called once per frame
    void Update()
    {
        if (hoveringWithUnit)
        {
            if (Input.GetMouseButtonUp(0))
            {
                shop.TrySellUnit(unit, unitTileID);
            }
        }
    }

    public void PointerEnter()
    {
        if (unitPlacement.GetHeldUnit())
        {
            hoveringWithUnit = true;
            unit = unitPlacement.GetHeldUnit();
            unitTileID = unitPlacement.GetHeldUnitTileID();

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
