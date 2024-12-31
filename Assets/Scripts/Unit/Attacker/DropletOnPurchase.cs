using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DropletOnPurchase : OnPurchaseEffect
{
    [SerializeField] int numOfFreeRefreshes = 1;
    // Used for Droplet to give free shop refreshes
    public override void OnPurchase()
    {
        int boardID = GetComponent<Unit>().GetBoard();
        PlayerBoard board = FindObjectOfType<PlayerBoardsManager>().GetBoardByBoardID(boardID);
        ServerPlayerData playerData = FindObjectOfType<ServerPlayerDataManager>().GetPlayerData(board.owner.Value);
        playerData.freeRefreshes.Value += numOfFreeRefreshes;
    }

}
