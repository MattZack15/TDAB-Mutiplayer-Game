using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerUnitData : MonoBehaviour
{
    // Eternal Knights dead count
    public int ekdCount;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RegisterDeath(GameObject Attacker)
    {
        // Called by Attackers when they die

        HandleReanimatedGiant(Attacker);
    }
        
    private void HandleReanimatedGiant(GameObject deadAttacker)
    {
        Unit deadUnit = deadAttacker.GetComponent<Unit>();

        if (!isUndead(deadUnit)) { return; }

        int boardID = deadUnit.GetBoard();

        List<GameObject> attackers = GameObject.FindGameObjectsWithTag("Attacker").ToList();

        List<GameObject> ReanimatedGiants = new List<GameObject>();

        // Find all active Reanimated Giants on this board
        foreach (GameObject attacker in attackers)
        {
            Attacker attackerComp = attacker.GetComponent<Attacker>();
            if (!attackerComp.enabled) { continue; }
            if (attackerComp.GetComponent<Unit>().UnitName != "Reanimated Giant") { continue; }
            if (attackerComp.GetComponent<Unit>().GetBoard() != boardID) { continue; }

            ReanimatedGiants.Add(attacker);
        }
        // Signal To them
        foreach (GameObject reanimatedGiant in ReanimatedGiants)
        {
            reanimatedGiant.GetComponent<ReanimatedGiant>().UndeadDied();
        }
    }

    private bool isUndead(Unit unit)
    {
        foreach (Tribe tribe in unit.tribes) 
        {
            if (tribe.tribeName == "Undead")
            {
                return true;
            }
        }

        return false;
    }
}
