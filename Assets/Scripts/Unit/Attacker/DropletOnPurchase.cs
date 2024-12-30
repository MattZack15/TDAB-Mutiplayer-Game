using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DropletOnPurchase : NetworkBehaviour
{
    [SerializeField] int numOfFreeRefreshes = 1;
    // Used for Droplet to give a free shop refresh
    void Start()
    {
        if (!IsServer) return;
        // This is the way we do it
        int boardID = GetComponent<Unit>().GetBoard();
        
        // Ignore because this was spawned during a battle
        if (boardID > 0) { return; }
        
        PlayerBoard board = FindObjectOfType<PlayerBoardsManager>().GetBoardByBoardID(boardID);
        ServerPlayerData playerData = FindObjectOfType<ServerPlayerDataManager>().GetPlayerData(board.owner.Value);
        playerData.freeRefreshes.Value += numOfFreeRefreshes;
    }

}
