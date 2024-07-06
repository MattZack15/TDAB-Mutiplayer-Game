using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Necromancer : Attacker
{
    public override void OnEntry()
    {
        
        // Give Reborn To Next Attacker

        PlayerBoardsManager boardsManager = FindObjectOfType<PlayerBoardsManager>();

        PlayerBoard board = boardsManager.GetBoardByBoardID(GetComponent<Unit>().GetBoard());


        GameObject nextAttacker = board.AttackerSpawner.PeekNextAttacker();

        if (nextAttacker == null)
        {
            return;
        }

        if (nextAttacker.GetComponent<Necromancer>() == null)
        {
            
            Reborn RebornEffect = nextAttacker.AddComponent<Reborn>();

            nextAttacker.GetComponent<Attacker>().OnDeathEffects.Add(RebornEffect);
        }
        else
        {
            print("Cant Reborn Necromancer");
        }

        
    }
}
