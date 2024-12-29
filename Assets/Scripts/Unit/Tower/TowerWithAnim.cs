using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TowerWithAnim : Tower
{
    // Dear god why would i code it with inheritenche

    [SerializeField] private Animator animator;

    private bool attackAnimCallback = false;
    [SerializeField] float maxWaitTime = 1f;

    public override IEnumerator Attack()
    {

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

    public void AttackAnimationCallback()
    {
        if (!IsServer) { return; }
        attackAnimCallback = true;
    }
}
