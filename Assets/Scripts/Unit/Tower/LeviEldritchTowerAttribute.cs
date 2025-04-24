using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeviEldritchTowerAttribute : EldritchTowerAttribute
{
    [SerializeField] float attackSpeedGain = .1f;
    [SerializeField] LeviTower TowerScript;
    protected override void TriggerBloodlust(GameObject KillTarget)
    {
        base.TriggerBloodlust(KillTarget);
        float attackCooldownReduction = TowerScript.attackSpeed * attackSpeedGain;
        TowerScript.GainBloodlustEffect(attackCooldownReduction);
    }
}
