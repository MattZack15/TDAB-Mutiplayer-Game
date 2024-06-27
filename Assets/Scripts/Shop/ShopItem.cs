using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{

    private int unitID;
    private Unit unit;

    private Shop shop;
    public Image picture;
    public int shopIndex;

    public UnitToolTip unitToolTip;

    // Start is called before the first frame update
    void Start()
    {
        shop = FindObjectOfType<Shop>();
        unitToolTip = FindObjectOfType<UnitToolTip>();
    }


    public void OnItemClick()
    {
        shop.TryBuyUnit(unitID, shopIndex);
    }

    public void PopulateDisplay(GameObject Unit, int UnitID, int shopIndex)
    {
        this.shopIndex = shopIndex;
        unitID = UnitID;
        unit = Unit.GetComponent<Unit>();
        picture.sprite = unit.UnitIcon;

    }

    public void OnItemHover()
    {
        unitToolTip.SetDisplay(unit);
    }
}
