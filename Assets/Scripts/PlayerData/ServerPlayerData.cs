using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayerData : MonoBehaviour
{
    // Keeps Track of a Player and their Data

    public ulong clientID;
    public int coins;
    // (ShopIndex, UnitID)
    public Dictionary<int, int> shop = new Dictionary<int, int>();
    public List<int> warband = new List<int>();

    public void Init(ulong clientID)
    {
        this.clientID = clientID;
        coins = Shop.StartingCoins;
    }

    public void SetNewShop(int[] shopItems)
    {
        int shopIndex = 0;
        foreach (int shopItem in shopItems)
        {
            shop[shopIndex] = shopItem;
            shopIndex++;
        }
    }



}
