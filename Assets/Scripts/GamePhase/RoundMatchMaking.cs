using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RoundMatchMaking : MonoBehaviour
{
    private Dictionary<ulong, ulong> MatchHistory = new Dictionary<ulong, ulong>();


    public List<(ulong, ulong)> MakeMatches(List<ulong> clientIDs)
    {
        List<(ulong, ulong)> matches = new List<(ulong, ulong)> ();
        Dictionary<ulong, ulong> newMatchHistory = new Dictionary<ulong, ulong>();

        // Create List of Players
        List<ulong> playerIDs = new List<ulong>();
        foreach (ulong clientID in clientIDs)
        {
            playerIDs.Add(clientID);
        }

        while(playerIDs.Count >= 2)
        {
            // Randomly pick from the list of players
            ulong targetPlayer = playerIDs[Random.Range(0, playerIDs.Count)];
            playerIDs.Remove(targetPlayer);
            ulong chosenOpponent = 0;

            // If only one option pick that option
            if (playerIDs.Count == 1)
            {
                chosenOpponent = playerIDs[0];
            }
            else
            {
                // Get list of options
                List<ulong> options = new List<ulong> ();
                
                if (MatchHistory.ContainsKey(targetPlayer))
                {
                    ulong lastMatch = MatchHistory[targetPlayer];
                    foreach (ulong option in playerIDs)
                    {
                        // Don't match with your last Opponent
                        if (option != lastMatch)
                        {
                            options.Add(option);
                        }
                    }
                }
                else
                {
                    foreach (ulong option in playerIDs)
                    {
                        options.Add(option);
                    }
                }

                // Randomly Pick from the options
                chosenOpponent = options[Random.Range(0, options.Count)];
            }

            matches.Add((targetPlayer, chosenOpponent));
            playerIDs.Remove(chosenOpponent);

            newMatchHistory.Add(targetPlayer, chosenOpponent);
            newMatchHistory.Add(chosenOpponent, targetPlayer);
        }
        
        MatchHistory = newMatchHistory;
        return matches;
    }
}
