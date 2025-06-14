using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GamePhaseManager : NetworkBehaviour
{
    // Define Game Phase Enum
    public enum GamePhases { ShopPhase, BattlePhase }

    [SerializeField] float baseShopPhaseLength = 25f;
    [SerializeField] float incrementShopPhaseLength = 5f;
    [HideInInspector] public NetworkVariable<int> GamePhase = new NetworkVariable<int>();
    [HideInInspector] public NetworkVariable<float> turnTimer = new NetworkVariable<float>();
    [HideInInspector] public NetworkVariable<int> roundNumber = new NetworkVariable<int>();
    private bool forceStart = false;

    [SerializeField] PlayerBoardsManager playerBoardsManager;
    [SerializeField] UnitDex unitDex;
    [SerializeField] Shop shop;
    [SerializeField] ServerPlayerDataManager ServerPlayerDataManager;
    [SerializeField] RoundMatchMaking RoundMatchMaking;
    [SerializeField] BattleManager BattleManager;
    [SerializeField] CameraMovement CameraMovement;
    [SerializeField] GameEnd GameEnd;

    List<GameObject> deactivateOnBattleEnd = new List<GameObject>();
    

    public void StartGame()
    {
        roundNumber.Value = 0;
        turnTimer.Value = 0;
        StartShopPhase();
    }

    public void StartBattlePhase()
    {
        if (!IsServer) { return; }

        StartCoroutine(EnterBattlePhase());
    }

    private IEnumerator EnterBattlePhase()
    {
        // Reset Previous Data
        deactivateOnBattleEnd = new List<GameObject>();

        // Match making
        List<(ulong, ulong)> matches = RoundMatchMaking.MakeMatches(NetworkManager.ConnectedClientsIds.ToList());

        // Prepare all matches
        foreach ((ulong, ulong) match in matches) 
        {
            MakeMatch(match.Item1, match.Item2);
        }

        ulong[] towerIds = new ulong[deactivateOnBattleEnd.Count];
        int i = 0;
        foreach(GameObject tower in deactivateOnBattleEnd)
        {
            towerIds[i] = tower.GetComponent<NetworkObject>().NetworkObjectId;
            i++;
        }

        GamePhase.Value = (int)GamePhases.BattlePhase;
        BroadCastBattlePhaseStartRPC(towerIds);

        yield return BattleManager.StartBattles(matches);

        // Battles are over
        StartShopPhase();
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

        // Get List of attackers from sideboard
        List<GameObject> attackers = playerBoardsManager.PlayerBoardTable[attackerID].GetComponent<SideBoard>().GetAttackersForBattle();

        List<(GameObject, GameObject)> attackersInstancesAndTemplates = new List<(GameObject, GameObject)>();

        // Spawn them all disabled
        foreach (GameObject attacker in attackers) 
        {
            GameObject newAttacker = Instantiate(attacker, attackerSpawner.transform.position, Quaternion.identity);

            newAttacker.SetActive(false);
            attackersInstancesAndTemplates.Add((newAttacker, attacker));
        }

        attackerSpawner.UpdateAttackerQueue(attackersInstancesAndTemplates);
    }

    private void PrepareTowers(ulong defenderID)
    {
        // Loop Through all Boards and Enable their Towers

        PlayerBoard DefenderBoard = playerBoardsManager.PlayerBoardTable[defenderID];
        List<GameObject> towers = DefenderBoard.GetTowers();

        foreach (GameObject tower in towers)
        {
            tower.GetComponent<Unit>().SetActive();
            tower.GetComponent<Tower>().Init(DefenderBoard.BoardID);

            deactivateOnBattleEnd.Add(tower);
        }
    }



    [Rpc(SendTo.ClientsAndHost)]
    private void BroadCastBattlePhaseStartRPC(ulong[] towersIds)
    {
        // IDK why this is
        foreach (ulong towerId in towersIds)
        {
            NetworkObject Unit;
            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(towerId, out Unit);
            Unit.gameObject.GetComponent<Unit>().SetActive();
        }
    }

    public void ForceStart()
    {
        if (!IsServer) { return; }
        if (GamePhase.Value != (int)GamePhases.ShopPhase) { return; }

        forceStart = true;
    }

    IEnumerator WaitForShopEnd()
    {
        float timer = baseShopPhaseLength + (roundNumber.Value-1)*incrementShopPhaseLength;
        if (timer > 60f)
        {
            timer = 60f;
        }
        
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

        GameEnd.TrackPlayerPlacements();

        // increment round counter
        roundNumber.Value += 1;

        // Clean up Objects
        ulong[] towerIds = new ulong[deactivateOnBattleEnd.Count];
        int i = 0;
        foreach (GameObject tower in deactivateOnBattleEnd)
        {
            if (tower == null) {  continue; }
            // Store Network id
            towerIds[i] = tower.GetComponent<NetworkObject>().NetworkObjectId;
            i++;

            // Call on round end effects
            tower.GetComponent<Tower>().OnRoundEnd();

            // Set tower inactive
            tower.GetComponent<Unit>().SetInactive();
        }

        // Define how much money a player is given this round
        int RoundEarnings = 5 + (roundNumber.Value - 1);
        if (roundNumber.Value == 1) { RoundEarnings = Shop.StartingCoins; }
        
        foreach (ulong playerID in NetworkManager.ConnectedClientsIds)
        {
            ServerPlayerData playerData = ServerPlayerDataManager.GetPlayerData(playerID);
            // Set Everyones Money
            playerData.coins.Value = RoundEarnings;

            // Reduce Level Cost
            playerData.levelCost.Value -= 1;
            if (playerData.levelCost.Value < 0)
            {
                playerData.levelCost.Value = 0;
            }

            // Refresh Everyones shop (unless its frozen)
            if (playerData.shopIsFrozen.Value)
            {
                // Skip and unfreeze
                playerData.shopIsFrozen.Value = false;
            }
            else
            {
                shop.ShopRefresh(playerID);
            }
        }
        
        // Set GamePhase
        GamePhase.Value = (int)GamePhases.ShopPhase;
        
        BroadCastShopPhaseStartRPC(towerIds);

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

        // Move Camera back to own board
        CameraMovement.LookAtPlayersBoard(NetworkManager.Singleton.LocalClientId);
    }

}
