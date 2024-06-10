using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GamePhaseManager : NetworkBehaviour
{
    [SerializeField] PlayerWarband playerWarband;
    [SerializeField] PlayerBoardsManager playerBoardsManager;
    [SerializeField] UnitDex unitDex;

    Dictionary<ulong, List<GameObject>> attackerList = new Dictionary<ulong, List<GameObject>>();
    
    public void StartBattlePhase()
    {
        if (!IsServer) { return; }
        
        StartCoroutine(EnterBattlePhase());
    }

    IEnumerator EnterBattlePhase()
    {
        yield return GetAttackers();
        
        
        
        // Match making
        List<ulong> playerIDs = (List<ulong>)NetworkManager.ConnectedClientsIds;

        while (playerIDs.Count > 0)
        {
            MakeMatch(playerIDs[0], playerIDs[1]);

            playerIDs.RemoveAt(0);
            playerIDs.RemoveAt(0);
        }

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

    IEnumerator GetAttackers()
    {
        // Reset old List
        attackerList = new Dictionary<ulong, List<GameObject>>();

        // Request Attacker Info
        RequestAttackersClientRPC();

        // Wait For answers
        while (attackerList.Keys.Count != NetworkManager.Singleton.ConnectedClientsIds.Count)
        {
            print("Waiting For Attackers from Clients...");
            yield return null;
        }
    }

}
