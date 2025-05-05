using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class VardTower : MeleeTower
{
    public int sweepingStrikeDamage;
    public int bleedDamage;
    public float bleedDuration;
    public float slowPercent;

    // A melee tower than can also do a sweeping strike attack which applies a bleed and slow
    private int sweepingStrikes;

    public void GainBloodLustEffect()
    {
        sweepingStrikes += 1;
    }

    public override IEnumerator Attack()
    {
        if (sweepingStrikes <= 0)
        {
            yield return base.Attack();
            yield break;
        }

        // Perform Sweeping Strike
        // Start Animation
        FindObjectOfType<VFXManager>().PlayUnitAnimRPC(GetComponent<NetworkObject>().NetworkObjectId, "bloodlust");

        sweepingStrikes -= 1;

        // Wait for apex of attack animation
        while (!attackAnimCallback)
        {
            yield return null;
        }
        attackAnimCallback = false;


        // Deal Damage
        // Get all units in range
        List<Transform> attackers = AttackersInRange();
        foreach (Transform attackerTrans in attackers)
        {
            // Deal damage to them
            Attacker attacker = attackerTrans.gameObject.GetComponent<Attacker>();
            attacker.TakeHit(sweepingStrikeDamage, this);
            // Apply Bleed
            DamageOverTime bleed = attacker.AddComponent<DamageOverTime>();
            bleed.Init(bleedDamage, bleedDuration, this);
            // Apply slow
            TempMoveSpeedBuff slow = attacker.AddComponent<TempMoveSpeedBuff>();
            slow.Init(slowPercent, bleedDuration);
        }

        // Trigger On Attack
        TriggerOnAttackEffects();

        // Wait Cooldown
        yield return new WaitForSeconds(attackSpeed);
    }
}
