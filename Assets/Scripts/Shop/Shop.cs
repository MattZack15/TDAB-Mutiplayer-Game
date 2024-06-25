using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class Shop : NetworkBehaviour
{
    // Start is called before the first frame update
    public static int ShopSize = 3;
    public static int StartingCoins = 40;
    // How much money you get at the end of each round
    public static int RoundEarnings = 4;
    public static int RefreshCost = 1;
    public static int UnitCost = 3;
    public static int SellValue = 1;

    [SerializeField] private PlayerWarband PlayerWarband;
    [SerializeField] private UnitDex unitDex;
    [SerializeField] private ShopItemsUI ShopItemsUI;
    [SerializeField] private GamePhaseManager GamePhaseManager;
    [SerializeField] private ServerPlayerDataManager ServerPlayerDataManager;
    [SerializeField] private PlayerBoardsManager PlayerBoardsManager;

    // Client Side Coins 
    public int coins;

    private void Start()
    {
        coins = StartingCoins;
    }

    public void TryBuyUnit(int UnitID, int shopIndex)
    {
        BuyUnitRPC(UnitID, shopIndex, NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Server)]
    private void BuyUnitRPC(int UnitID, int shopIndex, ulong playerID)
    {
        // Check if thing is a unit
        if (!unitDex.Dex[UnitID]) { print("No Such Unit is in the Game"); return; }
        // Check if its in players Shop
        ServerPlayerData playerData = ServerPlayerDataManager.GetPlayerData(playerID);
        if (!playerData.shop.ContainsKey(shopIndex)) { print("No Such Shop item there");  return; }
        if (playerData.shop[shopIndex] != UnitID) { print("That unit is not at that shop index"); return; }
        // Must Have the coins
        if (ServerPlayerDataManager.GetPlayerData(playerID).coins < UnitCost) { print("Not enough coins"); return; }

        // Write Info To Server Player Data
        playerData.shop.Remove(shopIndex);
        playerData.coins -= UnitCost;

        // Send Info To Client
        BoughtUnitRPC(UnitID, shopIndex, playerID);

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BoughtUnitRPC(int UnitID, int shopIndex, ulong playerID)
    {
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            PlayerWarband.AddUnit(unitDex.Dex[UnitID]);
            PlayerBoard MyBoard = PlayerBoardsManager.PlayerBoardTable[playerID];
            MyBoard.SideBoard.AddUnitToSideBoard(unitDex.Dex[UnitID]);
            ShopItemsUI.RemoveItem(shopIndex);
            coins -= UnitCost;
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

        // Must Have the coins
        if (ServerPlayerDataManager.GetPlayerData(playerID).coins < RefreshCost) { print("Not enough coins"); return; }

        // Otherwise send give a new shop list
        int[] newShopItems = CreateShopItems();
        // Track On Server
        ServerPlayerDataManager.GetPlayerData(playerID).SetNewShop(newShopItems);
        ServerPlayerDataManager.GetPlayerData(playerID).coins -= RefreshCost;

        ShopRefreshClientRPC(newShopItems, playerID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ShopRefreshClientRPC(int[] ShopItems, ulong playerID)
    {
        if (playerID != NetworkManager.Singleton.LocalClientId) return;

        coins -= RefreshCost;

        ShopItemsUI.ReciveNewShopItems(ShopItems);
    }
}
