using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemsUI : MonoBehaviour
{
    [SerializeField] Transform ShopItemsLayoutGroup;
    [SerializeField] private GameObject ShopItemPrefab;
    [SerializeField] UnitDex UnitDex;

    List<GameObject> ShopItemObjs = new List<GameObject>();
    

    public void ReciveNewShopItems(int[] UnitIDs)
    {
        // Reset
        foreach (GameObject ShopItemObj in ShopItemObjs)
        {
            Destroy(ShopItemObj);
        }
        ShopItemObjs = new List<GameObject>();

        int shopIndex = 0;
        foreach (int UnitID in UnitIDs)
        {
            GameObject newShopItem = Instantiate(ShopItemPrefab, ShopItemsLayoutGroup);

            GameObject Unit = UnitDex.Dex[UnitID];
            newShopItem.GetComponent<ShopItem>().PopulateDisplay(Unit, UnitID, shopIndex);

            ShopItemObjs.Add(newShopItem);
            shopIndex++;
        }
    }

    public void RemoveItem(int shopIndex)
    {
        // Search for that index // Note cant use list[shopIndex] because positions may change
        foreach (GameObject ShopItemObj in ShopItemObjs)
        {
            int thisShopIndex = ShopItemObj.GetComponent<ShopItem>().shopIndex;
            if (thisShopIndex == shopIndex)
            {
                ShopItemObjs.Remove(ShopItemObj);
                Destroy(ShopItemObj);
                break;
            }
        }
    }
}
