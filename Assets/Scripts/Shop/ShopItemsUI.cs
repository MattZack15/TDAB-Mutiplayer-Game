using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemsUI : MonoBehaviour
{
    [SerializeField] Transform ShopItemsLayoutGroup;
    [SerializeField] private GameObject ShopItemPrefab;
    [SerializeField] UnitDex UnitDex;

    List<GameObject> ShopItemObjs = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReciveNewShopItems(int[] UnitIDs)
    {
        // Reset
        foreach (GameObject ShopItemObj in ShopItemObjs)
        {
            Destroy(ShopItemObj);
        }
        ShopItemObjs = new List<GameObject>();

        foreach (int UnitID in UnitIDs)
        {
            GameObject newShopItem = Instantiate(ShopItemPrefab, ShopItemsLayoutGroup);

            GameObject Unit = UnitDex.Dex[UnitID];
            newShopItem.GetComponent<ShopItem>().PopulateDisplay(Unit, UnitID);

            ShopItemObjs.Add(newShopItem);
        }
    }
}
