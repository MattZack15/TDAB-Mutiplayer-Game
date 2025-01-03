using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GamePhaseManager : NetworkBehaviour
{
    // Define Game Phase Enum
    public enum GamePhases { ShopPhase, BattlePhase }
    public GamePhases GamePhase = GamePhases.ShopPhase;

    [SerializeField] float ShopPhaseLength = 25f;
    [HideInInspector] public NetworkVariable<float> turnTimer = new NetworkVariable<float>();
    [HideInInspector] public NetworkVariable<int> roundNumber = new NetworkVariable<int>();
    private bool forceStart = false;

    [SerializeField] PlayerWarband playerWarband;
    [SerializeField] PlayerBoardsManager playerBoardsManager;
    [SerializeField] UnitDex unitDex;
    [SerializeField] Shop shop;
    [SerializeField] ServerPlayerDataManager ServerPlayerDataManager;
    [SerializeField] RoundMatchMaking RoundMatchMaking;

    List<GameObject> deactivateOnBattleEnd = new List<GameObject>();
    
    List<AttackerSpawner> AttackerSpawners = new List<AttackerSpawner>();


    public void StartGame()
    {
        roundNumber.Value = 0;
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
        List<(ulong, ulong)> matches = RoundMatchMaking.MakeMatches(NetworkManager.ConnectedClientsIds.ToList());


        while (matches.Count > 0)
        {
            MakeMatch(matches[0].Item1, matches[0].Item2);

            matches.RemoveAt(0);
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

        // increment round counter
        roundNumber.Value += 1;

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

        // Define how much money a player is given this round
        int RoundEarnings = 3 + (roundNumber.Value - 1);
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

            // Refresh Everyones shop
            shop.ShopRefresh(playerID);
        }
        
        // Set GamePhase
        GamePhase = GamePhases.ShopPhase;
        
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

        // Set GamePhase
        GamePhase = GamePhases.ShopPhase;

    }

}
