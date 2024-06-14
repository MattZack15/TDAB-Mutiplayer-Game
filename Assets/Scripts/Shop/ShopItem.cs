using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{

    private int unitID;
    private Shop shop;
    public UnityEngine.UI.Image picture;
    public int shopIndex;

    // Start is called before the first frame update
    void Start()
    {
        shop = FindObjectOfType<Shop>();
    }


    public void OnItemClick()
    {
        shop.TryBuyUnit(unitID, shopIndex);
    }

    public void PopulateDisplay(GameObject Unit, int UnitID, int shopIndex)
    {
        this.shopIndex = shopIndex;
        unitID = UnitID;
        picture.sprite = Unit.GetComponent<Unit>().UnitIcon;

    }
}
