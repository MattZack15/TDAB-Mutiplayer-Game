using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttribute : MonoBehaviour
{
    protected Tower Tower;

    // Start is called before the first frame update
    void Start()
    {
        Tower = GetComponent<Tower>();

        Tower.TowerAttributes.Add(this);

        if (GetComponent<Unit>().tribes.Count == 0)
        {
            print("This Unit is not of a Tribe");
        }
    }

    protected int GetLevel(string tribe)
    {
        // Returns the level of the tribe based on the board its on
        PlayerBoard board = FindObjectOfType<PlayerBoardsManager>().GetBoardByBoardID(GetComponent<Unit>().GetBoard());
        TribeSynergy TribeSynergy = board.gameObject.GetComponent<TribeSynergy>();

        if (tribe == "Arcane")
        {
            return TribeSynergy.ArcaneCount.Value;
        }
        else if (tribe == "Eldritch")
        {
            return TribeSynergy.EldritchCount.Value;
        }
        print(tribe + " Not found");
        return 0;
    }

    public virtual void OnAttack()
    {

    }

    public virtual void OnReciveKillCredit(GameObject KillTarget) 
    { 

    }


}
