using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GamePhaseManager : NetworkBehaviour
{
    public enum GamePhases { ShopPhase, BattlePhase }

    public GamePhases GamePhase = GamePhases.ShopPhase;

    [SerializeField] PlayerWarband playerWarband;
    [SerializeField] PlayerBoardsManager playerBoardsManager;
    [SerializeField] UnitDex unitDex;
    [SerializeField] Shop shop;
    [SerializeField] ServerPlayerDataManager ServerPlayerDataManager;

    Dictionary<ulong, PlayerTurnInfo> PlayerTurnInfos = new Dictionary<ulong, PlayerTurnInfo>();

    List<GameObject> cleanUpOnBattleEnd = new List<GameObject>();
    
    List<AttackerSpawner> AttackerSpawners = new List<AttackerSpawner>();

    public struct PlayerTurnInfo
    {
        public List<GameObject> attackerList;
        public List<GameObject> towerList;
        public List<Vector3> towerPos;
    }

    public void StartBattlePhase()
    {
        if (!IsServer) { return; }
        
        StartCoroutine(EnterBattlePhase());
    }

    IEnumerator EnterBattlePhase()
    {
        yield return GetPlayerTurnInfo();

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

        BroadCastBattlePhaseStartRPC();

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

        AttackerSpawner attackerSpawner = playerBoardsManager.PlayerBoardTable[defenderID].AttackerSpawner;

        if (!AttackerSpawners.Contains(attackerSpawner))
        {
            AttackerSpawners.Add(attackerSpawner);
        }
        attackerSpawner.UpdateAttackerQueue(PlayerTurnInfos[attackerID].attackerList);

        // Prepare Towers
        int i = 0;
        foreach (GameObject tower in PlayerTurnInfos[defenderID].towerList)
        {
            GameObject newTower = Instantiate(tower, PlayerTurnInfos[defenderID].towerPos[i], Quaternion.identity);
            newTower.GetComponent<NetworkObject>().Spawn();

            PlayerBoard CurrentBoard = playerBoardsManager.PlayerBoardTable[defenderID];
            Transform EndPos = CurrentBoard.HexagonGrid.GetTileById(PlayerBoard.endTile).transform;

            newTower.GetComponent<Tower>().trackEndPoint = EndPos;

            cleanUpOnBattleEnd.Add(newTower);

            i++;
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RequestTurnInfoClientRPC()
    {
        ulong myClientId = NetworkManager.Singleton.LocalClientId;
        // Get My Towers
        PlayerBoard myPlayerBoard = playerBoardsManager.PlayerBoardTable[myClientId];
        BoardState BoardState = myPlayerBoard.GetTowerInfo();

        // Send my Attackers to Server
        SideBoard mySideBoard = myPlayerBoard.gameObject.GetComponent<SideBoard>();
        int[] AttackerIDs = mySideBoard.GetAttackers().ToArray();

        ReciveTurnInfoServerRPC(myClientId, AttackerIDs, BoardState.Towers, BoardState.positions);
    }

    [Rpc(SendTo.Server)]
    private void ReciveTurnInfoServerRPC(ulong playerID, int[] attackerIDs, int[] towerIDs, Vector3[] towerPos) 
    {
        List<GameObject> attackers = new List<GameObject>();
        List<GameObject> towers = new List<GameObject>();
        foreach (int unitID in attackerIDs) 
        {
            attackers.Add(unitDex.Dex[unitID]);
        }
        foreach (int unitID in towerIDs)
        {
            towers.Add(unitDex.Dex[unitID]);
        }

        PlayerTurnInfo playerTurnInfo = new PlayerTurnInfo();
        playerTurnInfo.attackerList = attackers;
        playerTurnInfo.towerList = towers;
        playerTurnInfo.towerPos = towerPos.ToList();

        PlayerTurnInfos.Add(playerID, playerTurnInfo);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BroadCastBattlePhaseStartRPC()
    {
        GamePhase = GamePhases.BattlePhase;

        // Hide Side Board
    }

    IEnumerator GetPlayerTurnInfo()
    {
        // Reset old List
        PlayerTurnInfos = new Dictionary<ulong, PlayerTurnInfo>();

        // Request Info
        RequestTurnInfoClientRPC();

        // Wait For answers
        while (PlayerTurnInfos.Keys.Count != NetworkManager.Singleton.ConnectedClientsIds.Count)
        {
            //print("Waiting For Attackers from Clients...");
            yield return null;
        }
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

    private void StartShopPhase()
    {
        if (!IsServer) { return; }

        // Clean up Objects
        foreach (GameObject go in cleanUpOnBattleEnd)
        {
            if (go != null)
            {
                go.GetComponent<NetworkObject>().Despawn();
            }
        }

        // Give Everyone Money
        foreach (ulong playerID in NetworkManager.ConnectedClientsIds)
        {
            ServerPlayerDataManager.GetPlayerData(playerID).coins += Shop.RoundEarnings;
        }
        // Set GamePhase
        GamePhase = GamePhases.ShopPhase;

        BroadCastShopPhaseStartRPC();


        // Refresh Everyones shop
        foreach (ulong clientID in NetworkManager.ConnectedClientsIds)
        {
            shop.ShopRefreshServerRPC(clientID);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BroadCastShopPhaseStartRPC()
    {
        // Set GamePhase
        GamePhase = GamePhases.ShopPhase;

        // Get Money
        shop.coins += Shop.RoundEarnings;
    }

}
