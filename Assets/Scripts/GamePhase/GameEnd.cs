using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameEnd : NetworkBehaviour
{
    // Keep track of the order which players lose
    // So we can say who is the winner (and what place everyone got)

    // Each time a player is eliminated their id is added to the stack
    List<ulong> deadPlayerStack = new List<ulong>();

    [SerializeField] ServerPlayerDataManager ServerPlayerDataManager;
    [SerializeField] EndGameRankings EndGameRankings;

    public bool gameHasEnded = false;

    // Function gets called by gamephase manager every round
    public void TrackPlayerPlacements()
    {
        if (gameHasEnded) return;

        // Search all players and check their health
        List<ulong> newDeadPlayers = new List<ulong>();
        List<ServerPlayerData> ServerPlayerDatas = ServerPlayerDataManager.GetAllPlayerData();
        foreach (ServerPlayerData playerData in ServerPlayerDatas)
        {
            // Check if player is dead
            if (playerData.health.Value <= 0)
            {
                // If we have already tracked this player then we ignore
                if (deadPlayerStack.Contains(playerData.clientID.Value)) { continue; }
                // Otherwise keep note of them
                newDeadPlayers.Add(playerData.clientID.Value);

            }
        }

        // Now we have a list of players that died this round
        // Add them to deadPlayerStack in order of how low their hp is
        while (newDeadPlayers.Count > 0)
        {
            int lowestHP = 100;
            ulong lowestHPPlayer = newDeadPlayers[0];
            foreach (ulong playerID in newDeadPlayers)
            {
                ServerPlayerData playerData = ServerPlayerDataManager.GetPlayerData(playerID);
                if (playerData.health.Value < lowestHP)
                {
                    lowestHP = playerData.health.Value;
                    lowestHPPlayer = playerID;
                }
            }
            // Remove Player
            newDeadPlayers.Remove(lowestHPPlayer);
            deadPlayerStack.Add(lowestHPPlayer);
        }

        CheckForGameEnd();
    }

    private void CheckForGameEnd()
    {
        // The game ends when there is only one player left
        int alivePlayerCount = 0;
        ulong alivePlayer = 0;
        List<ServerPlayerData> ServerPlayerDatas = ServerPlayerDataManager.GetAllPlayerData();
        foreach (ServerPlayerData playerData in ServerPlayerDatas)
        {
            // Check if player is dead
            if (playerData.health.Value > 0)
            {
                alivePlayerCount += 1;
                alivePlayer = playerData.clientID.Value;
            }
        }

        if (alivePlayerCount > 1)
        {
            return;
        }

        // Otherwise the game is over here
        gameHasEnded = true;
        // Add final player to the stack
        deadPlayerStack.Add(alivePlayer);

        // Send out endscreen signal
        // Create leaderboard of names
        FixedString128Bytes[] leaderboard = new FixedString128Bytes[deadPlayerStack.Count];
        int i = 0;
        foreach (ulong playerID in deadPlayerStack)
        {
            ServerPlayerData playerData = ServerPlayerDataManager.GetPlayerData(playerID);
            leaderboard[i] = playerData.username.Value;
            i++;
        }

        EndScreenRPC(leaderboard);

    }

    [Rpc(SendTo.ClientsAndHost)]
    public void EndScreenRPC(FixedString128Bytes[] playerNames)
    {
        EndGameRankings.DisplayRankings(playerNames);
    }
}
