using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Shop : NetworkBehaviour
{
    // Interface for buying and selling units, levels, refreshing shop

    public static int ShopSize = 3;
    public static int StartingCoins = 70;
    // How much money you get at the end of each round
    public static int RoundEarnings = 5;
    public static int RefreshCost = 1;
    public static int UnitCost = 3;
    public static int SellValue = 1;
    // Base cost of leveling to each level
    static List<int> levelCosts = new List<int> { 5, 7, 8, 9, 11};

    [SerializeField] private ShopPool ShopPool;
    [SerializeField] private PlayerWarband PlayerWarband;
    [SerializeField] private UnitDex unitDex;
    [SerializeField] private ShopItemsUI ShopItemsUI;
    [SerializeField] private GamePhaseManager GamePhaseManager;
    [SerializeField] private ServerPlayerDataManager ServerPlayerDataManager;
    [SerializeField] private PlayerBoardsManager PlayerBoardsManager;
    [SerializeField] private UnitPlacement unitPlacement;
    [SerializeField] private UnitUpgrades UnitUpgrades;
    

    public void TryBuyUnit(int UnitID, int shopIndex)
    {
        if (!CheckCoinsClientSide(UnitCost)) { return; }
        
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
        if (ServerPlayerDataManager.GetPlayerData(playerID).coins.Value < UnitCost) { print("Not enough coins"); return; }

        // Write Info To Server Player Data
        playerData.shop.Remove(shopIndex);
        playerData.coins.Value -= UnitCost;

        // Remove from shop pool
        ShopPool.RemoveUnitFromPool(unitDex.Dex[UnitID]);

        // Spawn unit
        PlayerBoard Playersboard = PlayerBoardsManager.PlayerBoardTable[playerID];
        Playersboard.SideBoard.AddUnitToSideBoard(unitDex.Dex[UnitID]);

        UnitUpgrades.CheckForUnitUpgrade(Playersboard.BoardID);

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


    public void TryShopRefresh()
    {
        // Called By Shop Refresh Button
        // Client Side Check For Coins
        if (!CheckCoinsClientSide(RefreshCost) && ServerPlayerDataManager.GetMyPlayerData().freeRefreshes.Value == 0) { return; }
        
        BuyShopRefreshServerRPC(NetworkManager.Singleton.LocalClientId);
    }
    
    [Rpc(SendTo.Server)]
    public void BuyShopRefreshServerRPC(ulong playerID)
    {
        // Must be in shop phase
        if (GamePhaseManager.GamePhase != GamePhaseManager.GamePhases.ShopPhase) { return; }
        ServerPlayerData playerData = ServerPlayerDataManager.GetPlayerData(playerID);


        // Check for free refreshes
        if (playerData.freeRefreshes.Value > 0)
        {
            // If they have them use them
            playerData.freeRefreshes.Value -= 1;
        }
        else
        {
            // Otherwise
            // Must Have the coins
            if (playerData.coins.Value < RefreshCost) { print("Not enough coins"); return; }

            // Remove Coins
            playerData.coins.Value -= RefreshCost;
        }


        ShopRefresh(playerID);
    }

    public void ShopRefresh(ulong targetPlayerID)
    {
        if (!IsServer) { return; }
        // Otherwise send give a new shop list
        int[] newShopItems = ShopPool.GenerateShopSelection(targetPlayerID, ShopSize);
        // Track On Server
        ServerPlayerDataManager.GetPlayerData(targetPlayerID).SetNewShop(newShopItems);
        
        ShopRefreshClientRPC(newShopItems, targetPlayerID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ShopRefreshClientRPC(int[] ShopItems, ulong playerID)
    {
        if (playerID != NetworkManager.Singleton.LocalClientId) return;

        ShopItemsUI.ReciveNewShopItems(ShopItems);
    }

    public void TrySellUnit(GameObject unit, Vector3 tileID)
    {
        SellUnitServerRPC(NetworkManager.Singleton.LocalClientId, unit.GetComponent<NetworkObject>().NetworkObjectId, tileID);
    }

    [Rpc(SendTo.Server)]
    public void SellUnitServerRPC(ulong playerID, ulong unitNetworkID, Vector3 tileID)
    {
        // TODO Check to make sure that player owns that unit

        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(unitNetworkID, out NetworkObject unit);

        // Add back to shop pool
        ShopPool.AddUnitBackToPool(unitDex.Dex[unit.gameObject.GetComponent<Unit>().UnitID]);
        
        // Remove Unit
        unit.Despawn();
        
        // Must Update Tile So we can still place units on it
        unitPlacement.ClearTileClientRPC(tileID);

        // Give Coins
        ServerPlayerDataManager.GetPlayerData(playerID).coins.Value += SellValue;
    }


    public void TryBuyLevel()
    {
        // Client Side Check For Coins
        if (!CheckCoinsClientSide(ServerPlayerDataManager.GetMyPlayerData().levelCost.Value)) {  return; }
        // Buy From Server
        BuyLevelServerRPC(NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.Server)]
    public void BuyLevelServerRPC(ulong playerID)
    {
        // Retrieve Player Data
        ServerPlayerData playerData = ServerPlayerDataManager.GetPlayerData(playerID);

        // Check to make sure they have enough coins
        if (playerData.coins.Value < playerData.levelCost.Value) { print("Not Enough Coins"); return; }

        // Remove Coins
        playerData.coins.Value -= playerData.levelCost.Value;

        // Add Level
        playerData.level.Value += 1;

        // Update Level Cost
        playerData.levelCost.Value = levelCosts[Mathf.Min(playerData.level.Value-1, levelCosts.Count-1)];
    }

    private bool CheckCoinsClientSide(int itemCost)
    {
        // Check if the player has enough coins to buy it before it makes a server call
        ServerPlayerData myPlayerData = ServerPlayerDataManager.GetMyPlayerData();
        if (myPlayerData.coins.Value < itemCost) 
        { 
            print("Not Enough Coins"); 
            return false; 
        }

        return true;
    }
}
