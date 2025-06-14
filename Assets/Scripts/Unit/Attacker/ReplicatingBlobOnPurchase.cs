using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplicatingBlobOnPurchase : OnPurchaseEffect
{
    // Summons A Copy of this unit
    public override void OnPurchase()
    {
        // Find Board
        Unit unitScript = GetComponent<Unit>();
        int boardID = unitScript.GetBoard();
        PlayerBoard board = FindObjectOfType<PlayerBoardsManager>().GetBoardByBoardID(boardID);
        // Summon a copy
        GameObject copyPrefab = FindObjectOfType<UnitDex>().Dex[unitScript.UnitID];
        GameObject copy = board.SideBoard.AddUnitToSideBoard(copyPrefab);
        // Copy Over Stats
        Attacker.CopyOverBonusStats(GetComponent<Attacker>(), copy.GetComponent<Attacker>());
        // Flag to not add to shop pool
        copy.GetComponent<Unit>().quantityToShopPool = 0;

    }
}
