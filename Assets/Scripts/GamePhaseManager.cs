using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GamePhaseManager : NetworkBehaviour
{
    public enum GamePhases { ShopPhase, BattlePhase }

    public GamePhases GamePhase = GamePhases.ShopPhase;

    [SerializeField] float ShopPhaseLength = 25f;
    public NetworkVariable<float> turnTimer = new NetworkVariable<float>();
    private bool forceStart = false;

    [SerializeField] PlayerWarband playerWarband;
    [SerializeField] PlayerBoardsManager playerBoardsManager;
    [SerializeField] UnitDex unitDex;
    [SerializeField] Shop shop;
    [SerializeField] ServerPlayerDataManager ServerPlayerDataManager;


    List<GameObject> deactivateOnBattleEnd = new List<GameObject>();
    
    List<AttackerSpawner> AttackerSpawners = new List<AttackerSpawner>();


    public void StartGame()
    {
        StartShopPhase();
    }

    public void StartBattlePhase()
    {
        if (!IsServer) { return; }

        EnterBattlePhase();
    }

    private void EnterBattlePhase()
    {
        // Reset Previous Data
        deactivateOnBattleEnd = new List<GameObject>();

        // Match making
        List<ulong> playerIDs = new List<ulong>();
        foreach (ulong clientID in NetworkManager.ConnectedClientsIds)
        {
            playerIDs.Add(clientID);
        }


        while (playerIDs.Count > 0)
        {
            MakeMatch(playerIDs[0], playerIDs[1]);

            playerIDs.RemoveAt(0);
            playerIDs.RemoveAt(0);
        }


        // Start Battle
        foreach (AttackerSpawner attackerSpawner in AttackerSpawners)
        {
            attackerSpawner.StartSpawner();
        }

        ulong[] towerIds = new ulong[deactivateOnBattleEnd.Count];
        int i = 0;
        foreach(GameObject tower in deactivateOnBattleEnd)
        {
            towerIds[i] = tower.GetComponent<NetworkObject>().NetworkObjectId;
            i++;
        }
        
        BroadCastBattlePhaseStartRPC(towerIds);

        StartCoroutine(WaitForBattleEnd());
    }

    private void MakeMatch(ulong player1, ulong player2)
    {
        PrepareBattle(player1, player2);
        PrepareBattle(player2, player1);
    }

    private void PrepareBattle(ulong attackerID, ulong defenderID)
    {
        // Prepare attackers 
        PrepareAttackers(attackerID, defenderID);

        PrepareTowers(defenderID);

    }

    private void PrepareAttackers(ulong attackerID, ulong defenderID)
    {
        AttackerSpawner attackerSpawner = playerBoardsManager.PlayerBoardTable[defenderID].AttackerSpawner;

        // Used for being able to check when battle is over
        if (!AttackerSpawners.Contains(attackerSpawner)) 
        {
            AttackerSpawners.Add(attackerSpawner);
        }

        // Get List of attackers from sideboard
        List<GameObject> attackers = playerBoardsManager.PlayerBoardTable[attackerID].GetComponent<SideBoard>().GetAttackers();

        List<GameObject> attackersInstances = new List<GameObject>();

        // Spawn them all disabled
        foreach (GameObject attacker in attackers) 
        {
            GameObject newAttacker = Instantiate(attacker, attackerSpawner.transform.position, Quaternion.identity);

            newAttacker.SetActive(false);
            attackersInstances.Add(newAttacker);
        }

        attackerSpawner.UpdateAttackerQueue(attackersInstances);
    }

    private void PrepareTowers(ulong defenderID)
    {
        // Loop Through all Boards and Enable their Towers

        PlayerBoard DefenderBoard = playerBoardsManager.PlayerBoardTable[defenderID];
        HexagonGrid hexagonGrid = DefenderBoard.HexagonGrid;
        foreach (Vector2 TileId in hexagonGrid.Tiles.Keys)
        {
            HexagonTile tile = hexagonGrid.GetTileById(TileId).GetComponent<HexagonTile>();

            if (tile.inhabitor != null && tile.inhabitor.GetComponent<Tower>() != null)
            {
                GameObject tower = tile.inhabitor;
                tower.GetComponent<Unit>().SetActive();
                tower.GetComponent<Tower>().Init(DefenderBoard.BoardID);

                deactivateOnBattleEnd.Add(tower);
            }
        }
    }



    [Rpc(SendTo.ClientsAndHost)]
    private void BroadCastBattlePhaseStartRPC(ulong[] towersIds)
    {
        foreach (ulong towerId in towersIds)
        {
            NetworkObject Unit;
            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(towerId, out Unit);
            Unit.gameObject.GetComponent<Unit>().SetActive();
        }

        
        GamePhase = GamePhases.BattlePhase;

    }


    IEnumerator WaitForBattleEnd()
    {
        bool battleIsOver = false;
        while (!battleIsOver)
        {
            battleIsOver = true;
            // Check every board
            foreach (AttackerSpawner spawner in AttackerSpawners)
            {
                if (spawner.activeAtttack)
                {
                    battleIsOver = false;
                    break;
                }
            }
            
            yield return null;
        }

        StartShopPhase();
    }

    public void ForceStart()
    {
        if (!IsServer) { return; }

        forceStart = true;
    }

    IEnumerator WaitForShopEnd()
    {
        float timer = ShopPhaseLength;
        
        while (timer > 0 && !forceStart)
        {
            timer -= Time.deltaTime;
            turnTimer.Value = timer;
            yield return null;
        }

        turnTimer.Value = 0f;
        forceStart = false;

        StartBattlePhase();
    }

    private void StartShopPhase()
    {
        if (!IsServer) { return; }

        // Clean up Objects

        ulong[] towerIds = new ulong[deactivateOnBattleEnd.Count];
        int i = 0;
        foreach (GameObject tower in deactivateOnBattleEnd)
        {
            towerIds[i] = tower.GetComponent<NetworkObject>().NetworkObjectId;
            i++;
        }

        foreach (GameObject Tower in deactivateOnBattleEnd)
        {
            if (Tower != null)
            {
                Tower.GetComponent<Unit>().SetInactive();
            }
        }

        // Give Everyone Money
        foreach (ulong playerID in NetworkManager.ConnectedClientsIds)
        {
            ServerPlayerDataManager.GetPlayerData(playerID).coins += Shop.RoundEarnings;
        }
        // Set GamePhase
        GamePhase = GamePhases.ShopPhase;

        
        BroadCastShopPhaseStartRPC(towerIds);


        // Refresh Everyones shop
        foreach (ulong clientID in NetworkManager.ConnectedClientsIds)
        {
            shop.ShopRefreshServerRPC(clientID);
        }

        StartCoroutine(WaitForShopEnd());
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BroadCastShopPhaseStartRPC(ulong[] towersIds)
    {
        // Set Towers Inactive
        foreach (ulong towerId in towersIds)
        {
            NetworkObject Unit;
            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(towerId, out Unit);

            Unit.gameObject.GetComponent<Unit>().SetInactive();
        }

        // Set GamePhase
        GamePhase = GamePhases.ShopPhase;

        // Get Money
        shop.coins += Shop.RoundEarnings;
    }

}
