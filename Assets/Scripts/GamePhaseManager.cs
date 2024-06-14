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

    Dictionary<ulong, List<GameObject>> attackerList = new Dictionary<ulong, List<GameObject>>();
    
    List<AttackerSpawner> AttackerSpawners = new List<AttackerSpawner>();

    public void StartBattlePhase()
    {
        if (!IsServer) { return; }
        
        StartCoroutine(EnterBattlePhase());
    }

    IEnumerator EnterBattlePhase()
    {
        yield return GetAttackers();

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
        // Get attackers 

        AttackerSpawner attackerSpawner = playerBoardsManager.PlayerBoardTable[defenderID].AttackerSpawner;

        if (!AttackerSpawners.Contains(attackerSpawner))
        {
            AttackerSpawners.Add(attackerSpawner);
        }
        
        attackerSpawner.UpdateAttackerQueue(attackerList[attackerID]);

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RequestAttackersClientRPC()
    {
        // Send my Attackers to Server
        int[] AttackerIDs = playerWarband.GetOwnedUnitIDs().ToArray();
        ReciveAttackersServerRPC(NetworkManager.Singleton.LocalClientId, AttackerIDs);
    }

    [Rpc(SendTo.Server)]
    private void ReciveAttackersServerRPC(ulong playerID, int[] attackerIDs) 
    {
        List<GameObject> attackers = new List<GameObject>();
        foreach (int attackerID in attackerIDs) 
        {
            attackers.Add(unitDex.Dex[attackerID]);
        }
        
        
        attackerList.Add(playerID, attackers);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BroadCastBattlePhaseStartRPC()
    {
        GamePhase = GamePhases.BattlePhase;
    }

    IEnumerator GetAttackers()
    {
        // Reset old List
        attackerList = new Dictionary<ulong, List<GameObject>>();

        // Request Attacker Info
        RequestAttackersClientRPC();

        // Wait For answers
        while (attackerList.Keys.Count != NetworkManager.Singleton.ConnectedClientsIds.Count)
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
        
        BroadCastShopPhaseStartRPC();

        GamePhase = GamePhases.ShopPhase;

        // Refresh Everyones shop
        foreach (ulong clientID in NetworkManager.ConnectedClientsIds)
        {
            shop.ShopRefreshServerRPC(clientID);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BroadCastShopPhaseStartRPC()
    {
        GamePhase = GamePhases.ShopPhase;
    }

}
