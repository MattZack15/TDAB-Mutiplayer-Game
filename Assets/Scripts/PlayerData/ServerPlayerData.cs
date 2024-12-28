using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerPlayerData : NetworkBehaviour
{
    // Keeps Track of a Player and their Data

    public NetworkVariable<ulong> clientID = new NetworkVariable<ulong>();
    public NetworkVariable<int> coins = new NetworkVariable<int>();
    public NetworkVariable<int> level = new NetworkVariable<int>();
    public NetworkVariable<int> levelCost = new NetworkVariable<int>();

    // Stores information about whats in a players Shop (ShopIndex, UnitID), SERVER SIDE
    public Dictionary<int, int> shop = new Dictionary<int, int>();

    public void Init(ulong clientID)
    {
        if (!IsServer) return;
        
        this.clientID.Value = clientID;
        coins.Value = Shop.StartingCoins;
        level.Value = 1;
        levelCost.Value = 5;
    }

    public void SetNewShop(int[] shopItems)
    {
        if (!IsServer) return;

        int shopIndex = 0;
        foreach (int shopItem in shopItems)
        {
            shop[shopIndex] = shopItem;
            shopIndex++;
        }
    }



}
