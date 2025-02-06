using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyTempestHandler : MonoBehaviour
{
    [SerializeField] PlayerBoardsManager PlayerBoardsManager;
    [SerializeField] ServerPlayerDataManager ServerPlayerDataManager;
    [SerializeField] GameObject GreedyTempestPrefab;

    public void HandleBuyUnit(GameObject spawnedPurchsedUnit, ulong playerID)
    {
        // Called whenever a unit is purchased to check how much it gains from GreedyTempest

        Unit unit = spawnedPurchsedUnit.GetComponent<Unit>();
        // Must be attacker
        if (!unit.isAttacker()) { return ; }
        // Must be elemental
        if (!unit.isElemental()) { return; }

        // Find players number of GreedyTempest Stacks
        ServerPlayerData playerData = ServerPlayerDataManager.GetPlayerData(playerID);
        int stacks = playerData.greedyTempestStacks.Value;

        // Add stats to attacker unit
        GreedyTempest GreedyTempest = GreedyTempestPrefab.GetComponent<GreedyTempest>();
        Attacker attacker = unit.GetComponent<Attacker>();

        attacker.AddMaxHp(GreedyTempest.GetBonusHealth(stacks));
        attacker.AddFlatMoveSpeed(GreedyTempest.GetBonusMoveSpeed(stacks));

        //print($"Bought Stacks {stacks}");

    }

    public void HandleSellUnit(GameObject unit, int boardID, ulong playerID)
    {
        // Called whenever a unit is sold to see if the player gets future buffs from Greedy Tempest

        // If unit is an elemental
        if (!unit.GetComponent<Unit>().isElemental()) { return; }

        // If Greedy Tempest is on board
        List<GameObject> attackers = PlayerBoardsManager.GetBoardByBoardID(boardID).SideBoard.GetAllAttackersOnSideBoard();
        string tempestName = GreedyTempestPrefab.GetComponent<Unit>().UnitName;
        foreach (GameObject attacker in attackers) 
        {
            if (tempestName == attacker.GetComponent<Unit>().UnitName)
            {
                // Tempest is on board
                int stacksPerEleSold = attacker.GetComponent<GreedyTempest>().stacksPerEleSold;
                ServerPlayerData playerData = ServerPlayerDataManager.GetPlayerData(playerID);
                playerData.greedyTempestStacks.Value += stacksPerEleSold;

                // Play proc particles
                Vector3 VFXPos = attacker.transform.position + new Vector3(0f, 1f, 0f);
                FindObjectOfType<VFXManager>().PlayGreedyTempestProcParticlesRPC(VFXPos);

                //print($"Sold Stacks {playerData.greedyTempestStacks.Value}");
            }
        }
        // Let the loop run so it works with multiple of them
    }
}
