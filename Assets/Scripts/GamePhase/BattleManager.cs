using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleManager : NetworkBehaviour
{
    // This script is meant to manage battles
    // It will take a match between p1 and p2
    // Play the attack of p1 on p2 - Speeding up if nessary
    // note p1's damage on p2
    // Play the attack of p2 on p1 - Speeding up if nessary
    // note p2's damage on p1
    // Subtract the values to find the overall damage
    // cap the damage and apply it to losing players health

    [SerializeField] PlayerBoardsManager PlayerBoardsManager;
    [SerializeField] CameraMovement CameraMovement;
    [SerializeField] BattleIndicatorUI BattleIndicatorUI;
    [SerializeField] ServerPlayerDataManager ServerPlayerDataManager;
    [SerializeField] BattleDamageSummary BattleDamageSummary;

    [SerializeField] int minHealthDamage = 5;
    [SerializeField] int damageCap = 35;

    public IEnumerator StartBattles(List<(ulong, ulong)> matches)
    {
        if (matches.Count == 0) { yield break; }
        // Track Total Battle Damage (Number of units that made it thorugh for each player)
        // player, EndZone, player EndZone

        // Start Phase 1
        List<AttackerSpawner> attackerSpawners = new List<AttackerSpawner>();
        ulong[] attackersIDs = new ulong[matches.Count];
        ulong[] defendersIDs = new ulong[matches.Count];
        int i = 0;
        foreach ((ulong, ulong) match in matches) 
        {
            ulong attackerPlayerID = match.Item1;
            ulong defenderPlayerID = match.Item2;

            AttackerSpawner attackerSpawner = PlayerBoardsManager.PlayerBoardTable[defenderPlayerID].AttackerSpawner;
            // Used for being able to check when battle is over
            attackerSpawners.Add(attackerSpawner);

            attackerSpawner.StartSpawner();

            // For camera movement
            attackersIDs[i] = attackerPlayerID;
            defendersIDs[i] = defenderPlayerID;
            i++;
        }

        // Move camera
        MoveCamerasRPC(attackersIDs, defendersIDs);

        // Wait for battles to end
        yield return WaitForBattlesToEnd(attackerSpawners);

        // Start Phase 2
        attackerSpawners = new List<AttackerSpawner>();
        attackersIDs = new ulong[matches.Count];
        defendersIDs = new ulong[matches.Count];
        i = 0;
        foreach ((ulong, ulong) match in matches)
        {
            ulong attackerPlayerID = match.Item2;
            ulong defenderPlayerID = match.Item1;

            AttackerSpawner attackerSpawner = PlayerBoardsManager.PlayerBoardTable[defenderPlayerID].AttackerSpawner;
            // Used for being able to check when battle is over
            attackerSpawners.Add(attackerSpawner);

            attackerSpawner.StartSpawner();

            // For camera movement
            attackersIDs[i] = attackerPlayerID;
            defendersIDs[i] = defenderPlayerID;
            i++;
        }

        // Move camera
        MoveCamerasRPC(attackersIDs, defendersIDs);

        // Wait for battles to end
        yield return WaitForBattlesToEnd(attackerSpawners);

        // Deal Damage to Players
        foreach ((ulong, ulong) match in matches)
        {
            ulong p1ID = match.Item1;
            ulong p2ID = match.Item2;

            EndZone p1EndZone = PlayerBoardsManager.PlayerBoardTable[p1ID].EndZone.GetComponent<EndZone>();
            EndZone p2EndZone = PlayerBoardsManager.PlayerBoardTable[p2ID].EndZone.GetComponent<EndZone>();

            // We look at the opposite players endzone to see how well we did
            int p1Score = p2EndZone.unitsPassedThroughThisRound;
            int p2Score = p1EndZone.unitsPassedThroughThisRound;

            // Reset for next round
            p1EndZone.unitsPassedThroughThisRound = 0;
            p2EndZone.unitsPassedThroughThisRound = 0;

            // p1 Win
            if (p1Score >= p2Score)
            {
                DealDamage(p1ID, p2ID, p1Score, p2Score);                
            }
            // p2 Win
            else if (p2Score > p1Score)
            {
                DealDamage(p2ID, p1ID, p2Score, p1Score);
            }
        }

        yield return new WaitForSeconds(3f);
    }

    private void DealDamage(ulong winnerID, ulong loserID, int winnerScore, int loserScore)
    {
        int scoreDifference = winnerScore - loserScore;
        int totalDamage = minHealthDamage + scoreDifference;
        totalDamage = Mathf.Min(totalDamage, damageCap);
        ServerPlayerData loserData = ServerPlayerDataManager.GetPlayerData(loserID);
        loserData.health.Value -= totalDamage;

        //print($"{loserID} takes {totalDamage} damage");
        // Show Summary on both clients
        BattleDamageSummary.PlayOnClient(winnerID, loserID, winnerScore, loserScore, minHealthDamage, loserData.health.Value + totalDamage, loserData.health.Value, winnerID);
        BattleDamageSummary.PlayOnClient(winnerID, loserID, winnerScore, loserScore, minHealthDamage, loserData.health.Value + totalDamage, loserData.health.Value, loserID);

    }

    IEnumerator WaitForBattlesToEnd(List<AttackerSpawner> attackerSpawners)
    {
        float battleTimer = 0f;
        bool battleIsOver = false;
        while (!battleIsOver)
        {
            // Assume all battle have ended
            battleIsOver = true;
            // Check every board
            foreach (AttackerSpawner spawner in attackerSpawners)
            {
                // If any match has not ended then we know there are still battles going
                if (spawner.activeAtttack)
                {
                    battleIsOver = false;
                    break;
                }
            }

            // Check for speed up / fast forward
            HandleBattleSpeedUp(battleTimer);

            battleTimer += Time.deltaTime;
            yield return null;
        }

        ChangeTimeScaleRPC(1f);
        yield return new WaitForSeconds(.5f);
    }

    private void HandleBattleSpeedUp(float battleTimer)
    {
        // Check for speed up / fast forward
        float nextTimeScale = 1f;
        if (battleTimer > 60f)
        {
            nextTimeScale = 5f;
        }
        else if (battleTimer > 20f)
        {
            nextTimeScale = 2f;
        }

        if (nextTimeScale != Time.timeScale)
        {
            ChangeTimeScaleRPC(nextTimeScale);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeTimeScaleRPC(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void MoveCamerasRPC(ulong[] attackers, ulong[] defenders)
    {
        ulong localPlayerID = NetworkManager.Singleton.LocalClientId;
        // Search Attackers list
        // If we find our ID then set our camera to the defender board
        int i = 0;
        foreach (ulong clientID in attackers)
        {
            if (clientID == localPlayerID)
            {
                CameraMovement.LookAtPlayersBoard(defenders[i]);
                BattleIndicatorUI.DisplayBattle(localPlayerID, defenders[i]);
                return;
            }
            i++;
        }

        // We are not in the attackers list so now look through defenders list to make sure we are there
        // In this case we just look at our own board because we are defending
        i = 0;
        foreach (ulong clientID in defenders)
        {
            if (clientID == localPlayerID)
            {
                CameraMovement.LookAtPlayersBoard(localPlayerID);
                BattleIndicatorUI.DisplayBattle(attackers[i], localPlayerID);
                return;
            }
            i++;
        }

        // If we are here then we must not be in battle so we just look at the first defender board
        CameraMovement.LookAtPlayersBoard(defenders[0]);
        BattleIndicatorUI.DisplayBattle(attackers[0], defenders[0]);
    }
}
