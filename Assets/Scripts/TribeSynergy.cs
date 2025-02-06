using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TribeSynergy : NetworkBehaviour
{

    // Counts the number of unqiue members of a tribe that is on a Player Board
    [SerializeField] PlayerBoard PlayerBoard;
    public NetworkVariable<int> ArcaneCount = new NetworkVariable<int>();
    public NetworkVariable<int> EldritchCount = new NetworkVariable<int>();

    private void Update()
    {
        if (!IsServer) return;

        // May be ineffcient
        CountTribeNumber();
    }
    
    private void CountTribeNumber()
    {
        
        int ArcaneCount = 0;
        int EldritchCount = 0;

        List<GameObject> towers = PlayerBoard.GetTowers();
        List<string> alreadyCounted = new List<string>();

        foreach (GameObject tower in towers)
        {
            string towerName = tower.GetComponent<Unit>().UnitName;
            if (alreadyCounted.Contains(towerName)) { continue; }

            List<Tribe> tribes = tower.GetComponent<Unit>().tribes;
            foreach (Tribe tribe in tribes)
            {
                if (tribe.tribeName == "Arcane")
                {
                    ArcaneCount += 1;
                }
                else if (tribe.tribeName == "Eldritch")
                {
                    EldritchCount += 1;
                }
            }
            
            alreadyCounted.Add(towerName);

        }

        this.ArcaneCount.Value = ArcaneCount;
        this.EldritchCount.Value = EldritchCount;
    }
}
