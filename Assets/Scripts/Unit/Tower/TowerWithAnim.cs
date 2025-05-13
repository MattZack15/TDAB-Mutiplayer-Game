using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TowerWithAnim : Tower
{
    // Dear god why would i code it with inheritenche

    [SerializeField] private Animator animator;

    protected bool attackAnimCallback = false;
    [SerializeField] protected float maxWaitTime = 1f;

    public override IEnumerator Attack()
    {
        //print("TowerWithAnim Attack");

        // Start Animation
        FindObjectOfType<VFXManager>().PlayUnitAnimRPC(GetComponent<NetworkObject>().NetworkObjectId, "attack");

        // Wait for apex of attack animation
        float timer = 0f;
        while (!attackAnimCallback && timer < maxWaitTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        attackAnimCallback = false;

        // Attack
        if (currentTarget != null)
        {
            StartCoroutine(base.Attack());
        }
        
        // Wait Cooldown
        yield return new WaitForSeconds(attackSpeed);
    }

    protected void BaseAttackMethod()
    {
        // Because IDK how to do base.base.Attack()
        StartCoroutine(base.Attack());
    }

    public override void InstantBonusAttack()
    {
        StartCoroutine(base.Attack());
    }

    public void AttackAnimationCallback()
    {
        if (!IsServer) { return; }
        attackAnimCallback = true;
    }
}
