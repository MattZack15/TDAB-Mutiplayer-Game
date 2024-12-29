using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthElementalAttacker : Attacker
{
    [SerializeField] private int flatDamageReduction;

    public override void TakeHit(int damage, Tower damageSource)
    {
        if (!IsServer) return;
        if (!this.enabled) return;

        damage = Mathf.Max(damage - flatDamageReduction, 0);

        base.TakeHit(damage, damageSource);
    }
}
