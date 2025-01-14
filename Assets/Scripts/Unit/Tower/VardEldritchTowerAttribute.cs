using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VardEldritchTowerAttribute : EldritchTowerAttribute
{
    [SerializeField] VardTower TowerScript;
    protected override void TriggerBloodlust(GameObject KillTarget)
    {
        TowerScript.GainBloodLustEffect();
    }
}
