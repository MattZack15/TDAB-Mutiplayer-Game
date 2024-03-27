using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBoards : NetworkBehaviour
{
    [SerializeField] private HexagonGrid HexagonGrid;
    [SerializeField] private Vector2 spacing;

    [SerializeField] GameObject PlayerBoard;

    public void SpawnPlayerBoards(int playerCount)
    {
        int i = 0;
        while (i < playerCount)
        {
            NetworkObject board = Instantiate(PlayerBoard).GetComponent<NetworkObject>();
            board.Spawn();
            i++;
        }
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void SpawnAllGridsRPC(int playerCount)
    {
        int i = 0;
        while(i < playerCount)
        {
            Vector2 GridOffset = new Vector2 (spacing.x * (i%3), (int)(i/3) * spacing.y);
            
            HexagonGrid.SpawnHexagonGrid(i, GridOffset);
            i++;
    
        }
        
    }

}
