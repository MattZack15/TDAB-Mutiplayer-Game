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

    public IEnumerator StartBattles(List<(ulong, ulong)> matches)
    {
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
    }

    IEnumerator WaitForBattlesToEnd(List<AttackerSpawner> attackerSpawners)
    {
        
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

            yield return null;
        }

        yield return new WaitForSeconds(.5f);
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
                return;
            }
        }

        // We are not in the attackers list so now look through defenders list to make sure we are there
        // In this case we just look at our own board because we are defending
        foreach (ulong clientID in defenders)
        {
            if (clientID == localPlayerID)
            {
                CameraMovement.LookAtPlayersBoard(localPlayerID);
                return;
            }
        }

        // If we are here then we must not be in battle so we just look at the first defender board
        print("Picking Default Cam Pos");
        CameraMovement.LookAtPlayersBoard(defenders[0]);
    }
}
