using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{

    private int unitID;
    private Shop shop;
    public UnityEngine.UI.Image picture;

    // Start is called before the first frame update
    void Start()
    {
        shop = FindObjectOfType<Shop>();
    }


    public void OnItemClick()
    {
        shop.TryBuyUnit(unitID);
    }

    public void PopulateDisplay(GameObject Unit, int UnitID)
    {
        unitID = UnitID;
        picture.sprite = Unit.GetComponent<Unit>().UnitIcon;

    }
}
