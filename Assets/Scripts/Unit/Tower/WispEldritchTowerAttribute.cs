using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispEldritchTowerAttribute : EldritchTowerAttribute
{
    [SerializeField] WispTower TowerScript;
    protected override void TriggerBloodlust(GameObject KillTarget)
    {
        TowerScript.GainBloodLustEffect();
    }
}
