using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class Shop : NetworkBehaviour
{
    // Start is called before the first frame update
    public static int ShopSize = 3;

    [SerializeField] private PlayerWarband PlayerWarband;
    [SerializeField] private UnitDex unitDex;
    [SerializeField] private ShopItemsUI ShopItemsUI;
    [SerializeField] private GamePhaseManager GamePhaseManager;

    public bool button;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (button)
        {
            TryShopRefresh();
            button = false;
        }
    }


    public void TryBuyUnit(int UnitID, int shopIndex)
    {
        BuyUnitRPC(UnitID, shopIndex, NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Server)]
    private void BuyUnitRPC(int UnitID, int shopIndex, ulong playerID)
    {
        if (!IsServer) { return; }

        if (!unitDex.Dex[UnitID]) { return; }

        // Write Info To Server

        // Send Info To Client
        BoughtUnitRPC(UnitID, shopIndex, playerID);

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BoughtUnitRPC(int UnitID, int shopIndex, ulong playerID)
    {
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            PlayerWarband.AddUnit(unitDex.Dex[UnitID]);
            ShopItemsUI.RemoveItem(shopIndex);
        }
    }

    private int[] CreateShopItems ()
    {
        if (!IsServer) return null;
        // Returns a list of Unit Ids for a shop
        int[] ShopItems = new int[ShopSize];

        int i = 0;
        while(i < ShopSize)
        {
            int RandomUnit = unitDex.UnitIDs[Random.Range(0, unitDex.UnitIDs.Count)];
            
            ShopItems[i] = RandomUnit;
            i++;
        }

        return ShopItems;
    }

    public void TryShopRefresh()
    {
        // Called By Shop Refresh Button
        ShopRefreshServerRPC(NetworkManager.Singleton.LocalClientId);
    }
    
    [Rpc(SendTo.Server)]
    public void ShopRefreshServerRPC(ulong playerID)
    {
        // Must be in shop phase
        if (GamePhaseManager.GamePhase != GamePhaseManager.GamePhases.ShopPhase) { return; }
        
        // Otherwise send them a new shop list
        ShopRefreshClientRPC(CreateShopItems(), playerID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ShopRefreshClientRPC(int[] ShopItems, ulong playerID)
    {
        if (playerID != NetworkManager.Singleton.LocalClientId) return;

        ShopItemsUI.ReciveNewShopItems(ShopItems);
    }
}
