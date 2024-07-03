using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Necromancer : Attacker
{
    public override void OnEntry()
    {
        return;
        
        // Give Reborn To Next Attacker

        PlayerBoardsManager boardsManager = FindObjectOfType<PlayerBoardsManager>();

        PlayerBoard board = boardsManager.GetBoardByBoardID(GetComponent<Unit>().GetBoard());

        if (board.AttackerSpawner.attackerQueue.Count == 0)
        {
            return;
        }
        GameObject nextAttacker = board.AttackerSpawner.attackerQueue[0];

        if (nextAttacker.GetComponent<Necromancer>() == null)
        {
            // CANNOT EDIT THE ATTACKER NOW
            
            Reborn RebornEffect = nextAttacker.AddComponent<Reborn>();

            nextAttacker.GetComponent<Attacker>();
        }
        else
        {
            print("Cant Reborn Necromancer");
        }

        
    }
}
