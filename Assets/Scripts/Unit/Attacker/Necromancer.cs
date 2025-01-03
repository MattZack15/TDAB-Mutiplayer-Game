using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : Attacker
{
    public int level = 1;

    public override void OnEntry()
    {
        
        // Give Reborn To Next Attacker

        PlayerBoardsManager boardsManager = FindObjectOfType<PlayerBoardsManager>();

        PlayerBoard board = boardsManager.GetBoardByBoardID(GetComponent<Unit>().GetBoard());

        GameObject nextAttacker = board.AttackerSpawner.PeekNextAttacker(0);
        GiveRebornToAttacker(nextAttacker);


        if (level > 1)
        {
            GameObject nexterAttacker = board.AttackerSpawner.PeekNextAttacker(1);
            GiveRebornToAttacker(nexterAttacker);
        }

    }

    private void GiveRebornToAttacker(GameObject Attacker)
    {
        if (Attacker == null)
        {
            return;
        }

        if (Attacker.GetComponent<Reborn>())
        {
            return;
        }

        Reborn RebornEffect = Attacker.AddComponent<Reborn>();
        Attacker.GetComponent<Attacker>().OnDeathEffects.Add(RebornEffect);
    }
}
