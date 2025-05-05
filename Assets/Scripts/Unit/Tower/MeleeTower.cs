using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MeleeTower : Tower
{
    [SerializeField] private Animator animator;

    protected bool attackAnimCallback = false;
    
    public override IEnumerator Attack()
    {

        // Start Animation
        FindObjectOfType<VFXManager>().PlayUnitAnimRPC(GetComponent<NetworkObject>().NetworkObjectId, "attack");


        // Wait for apex of attack animation
        while (!attackAnimCallback) 
        {
            yield return null;
        }
        attackAnimCallback = false;
        
        //AudioManager.Instance.PlayForBoardRPC(attackSoundName, GetComponent<Unit>().GetBoard(), true);
        
        // Deal Damage
        if (currentTarget != null)
        {
            currentTarget.gameObject.GetComponent<Attacker>().TakeHit(damage, this);
        }

        // Trigger On Attack
        TriggerOnAttackEffects();

        // Wait Cooldown
        yield return new WaitForSeconds(attackSpeed);
    }

    public void AttackAnimationCallback()
    {
        if (!IsServer) { return; }
        attackAnimCallback = true;
    }



}
