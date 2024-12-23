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

    public virtual void OnAttack()
    {

    }

    public virtual void OnReciveKillCredit(GameObject KillTarget) 
    { 

    }


}
